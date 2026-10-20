using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Celowniczek
{
    public class CommunityCrosshairsControl : UserControl
    {
        private readonly ConfigData config;
        private readonly CrosshairForm crosshairForm;
        private readonly TableLayoutPanel layout = new TableLayoutPanel();
        private readonly ThemePanel backgroundPanel = new ThemePanel();
        private readonly Label headingLabel = new Label();
        private readonly ListBox crosshairList = new ListBox();
        private readonly Label detailsLabel = new Label();
        private readonly Label statusLabel = new Label();
        private readonly Button refreshButton = new Button();
        private readonly Button applyButton = new Button();
        private readonly Button shareButton = new Button();
        private readonly List<CommunityCrosshair> crosshairs = new List<CommunityCrosshair>();
        private bool hasLoaded;

        public CommunityCrosshairsControl(ConfigData config, CrosshairForm crosshairForm)
        {
            this.config = config;
            this.crosshairForm = crosshairForm;
            Dock = DockStyle.Fill;
            Padding = new Padding(12);

            backgroundPanel.Dock = DockStyle.Fill;
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 1;
            layout.RowCount = 5;
            layout.Margin = new Padding(0);
            layout.Padding = new Padding(0);
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));

            headingLabel.Dock = DockStyle.Fill;
            headingLabel.Margin = new Padding(0, 0, 0, 8);
            headingLabel.Font = new Font("Segoe UI", 14, FontStyle.Bold);

            crosshairList.Dock = DockStyle.Fill;
            crosshairList.Margin = new Padding(0, 0, 0, 8);
            crosshairList.IntegralHeight = false;
            crosshairList.SelectionMode = SelectionMode.One;
            crosshairList.Enabled = true;
            crosshairList.TabStop = true;
            crosshairList.SelectedIndexChanged += (s, e) => UpdateSelectionDetails();
            crosshairList.DoubleClick += (s, e) => ApplySelectedCrosshair();
            crosshairList.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ApplySelectedCrosshair();
                    e.Handled = true;
                }
            };

            detailsLabel.Dock = DockStyle.Fill;
            detailsLabel.Margin = new Padding(0, 0, 0, 8);
            detailsLabel.BorderStyle = BorderStyle.FixedSingle;
            detailsLabel.Tag = "surface";

            FlowLayoutPanel buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            refreshButton.Size = new Size(114, 34);
            refreshButton.Tag = "secondary";
            refreshButton.Margin = new Padding(0, 0, 8, 0);
            refreshButton.Click += async (s, e) => await RefreshCrosshairsAsync();

            applyButton.Size = new Size(114, 34);
            applyButton.Margin = new Padding(0, 0, 8, 0);
            applyButton.Click += (s, e) => ApplySelectedCrosshair();

            shareButton.Size = new Size(114, 34);
            shareButton.Tag = "primary";
            shareButton.Margin = new Padding(0);
            shareButton.Click += (s, e) => OpenShareForm();

            buttonPanel.Controls.Add(refreshButton);
            buttonPanel.Controls.Add(applyButton);
            buttonPanel.Controls.Add(shareButton);

            statusLabel.Dock = DockStyle.Fill;
            statusLabel.Margin = new Padding(0);
            statusLabel.AutoEllipsis = true;

            layout.Controls.Add(headingLabel, 0, 0);
            layout.Controls.Add(crosshairList, 0, 1);
            layout.Controls.Add(detailsLabel, 0, 2);
            layout.Controls.Add(buttonPanel, 0, 3);
            layout.Controls.Add(statusLabel, 0, 4);
            backgroundPanel.Controls.Add(layout);
            Controls.Add(backgroundPanel);

            UpdateTexts();
        }

        public async Task EnsureLoadedAsync()
        {
            if (!hasLoaded)
            {
                await RefreshCrosshairsAsync();
            }
        }

        public void UpdateTexts()
        {
            bool polish = config.Language == "pl";
            headingLabel.Text = polish ? "Celowniki społeczności" : "Community crosshairs";

            refreshButton.Text = polish ? "Odśwież" : "Refresh";
            applyButton.Text = polish ? "Zastosuj" : "Apply";
            shareButton.Text = polish ? "Udostępnij" : "Share";
            statusLabel.Text = polish ? "Lista nie została jeszcze pobrana." : "The list has not been loaded yet.";
            UpdateSelectionDetails();
        }

        public void ApplyTheme(UiThemePalette theme)
        {
            UiThemeManager.Apply(backgroundPanel, theme);
            crosshairList.Invalidate();
        }

        private async Task RefreshCrosshairsAsync()
        {
            refreshButton.Enabled = false;
            statusLabel.Text = config.Language == "pl" ? "Pobieranie listy..." : "Loading community list...";
            try
            {
                IReadOnlyList<CommunityCrosshair> entries = await CommunityCrosshairService.GetCrosshairsAsync();
                crosshairs.Clear();
                crosshairs.AddRange(entries);
                crosshairList.BeginUpdate();
                crosshairList.Items.Clear();
                foreach (CommunityCrosshair crosshair in crosshairs)
                {
                    crosshairList.Items.Add(crosshair);
                }
                crosshairList.EndUpdate();
                hasLoaded = true;

                statusLabel.Text = crosshairs.Count == 0
                    ? config.Language == "pl" ? "Brak celowników w publicznym pliku JSON." : "No crosshairs are listed in the public JSON file."
                    : config.Language == "pl" ? $"Pobrano celowników: {crosshairs.Count}." : $"Loaded crosshairs: {crosshairs.Count}.";
                if (crosshairList.Items.Count > 0)
                {
                    crosshairList.SelectedIndex = 0;
                }
                else
                {
                    UpdateSelectionDetails();
                }
            }
            catch (Exception ex)
            {
                hasLoaded = false;
                statusLabel.Text = config.Language == "pl"
                    ? $"Nie udało się pobrać listy: {ex.Message}"
                    : $"Could not load the list: {ex.Message}";
            }
            finally
            {
                refreshButton.Enabled = true;
            }
        }

        private void UpdateSelectionDetails()
        {
            CommunityCrosshair? selected = crosshairList.SelectedItem as CommunityCrosshair;
            applyButton.Enabled = selected != null;
            if (selected == null)
            {
                detailsLabel.Text = config.Language == "pl" ? "Wybierz celownik z listy." : "Select a crosshair from the list.";
                return;
            }

            detailsLabel.Text = config.Language == "pl"
                ? $"Autor: {selected.Author}\r\nRozmiar: {selected.Size}   Grubość: {selected.Thickness}   Przerwa: {selected.Gap}\r\nKropka: {selected.DotRadius}   Kolor: {selected.ColorHex}   Styl: {GetLocalizedStyle(selected.Style)}"
                : $"Author: {selected.Author}\r\nSize: {selected.Size}   Thickness: {selected.Thickness}   Gap: {selected.Gap}\r\nDot: {selected.DotRadius}   Color: {selected.ColorHex}   Style: {selected.Style}";
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            ApplyTheme(UiThemeManager.GetTheme(config.Theme));
        }

        private string GetLocalizedStyle(CrosshairStyle style)
        {
            if (config.Language != "pl")
            {
                return style.ToString();
            }

            return style switch
            {
                CrosshairStyle.Cross => "Krzyż",
                CrosshairStyle.DotOnly => "Sama kropka",
                CrosshairStyle.Circle => "Okrąg",
                _ => style.ToString()
            };
        }

        private void ApplySelectedCrosshair()
        {
            if (crosshairList.SelectedItem is not CommunityCrosshair selected)
            {
                return;
            }

            config.Size = selected.Size;
            config.Thickness = selected.Thickness;
            config.Gap = selected.Gap;
            config.DotRadius = selected.DotRadius;
            config.ColorHex = selected.ColorHex;
            config.EnableOutline = selected.EnableOutline;
            config.Style = selected.Style;
            ConfigManager.ApplyToForm(config, crosshairForm);
            ConfigManager.SaveConfig(config);

            statusLabel.Text = config.Language == "pl"
                ? $"Zastosowano: {selected.Title}. Ustawienia zostały zapisane."
                : $"Applied: {selected.Title}. Settings have been saved.";
        }

        private void OpenShareForm()
        {
            using ShareCrosshairForm shareForm = new ShareCrosshairForm(config);
            shareForm.ShowDialog(FindForm());
        }
    }
}
