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

            Label heading = new Label
            {
                Location = new Point(12, 12),
                Size = new Size(365, 34),
                Font = new Font("Segoe UI", 14, FontStyle.Bold)
            };

            crosshairList.Location = new Point(12, 54);
            crosshairList.Size = new Size(365, 300);
            crosshairList.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
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

            detailsLabel.Location = new Point(12, 365);
            detailsLabel.Size = new Size(365, 96);
            detailsLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            detailsLabel.BorderStyle = BorderStyle.FixedSingle;

            refreshButton.Location = new Point(12, 472);
            refreshButton.Size = new Size(114, 34);
            refreshButton.Click += async (s, e) => await RefreshCrosshairsAsync();

            applyButton.Location = new Point(137, 472);
            applyButton.Size = new Size(114, 34);
            applyButton.Click += (s, e) => ApplySelectedCrosshair();

            shareButton.Location = new Point(263, 472);
            shareButton.Size = new Size(114, 34);
            shareButton.Click += (s, e) => OpenShareForm();

            statusLabel.Location = new Point(12, 520);
            statusLabel.Size = new Size(365, 52);
            statusLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            Controls.Add(heading);
            Controls.Add(crosshairList);
            Controls.Add(detailsLabel);
            Controls.Add(refreshButton);
            Controls.Add(applyButton);
            Controls.Add(shareButton);
            Controls.Add(statusLabel);

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
            if (Controls.Count > 0)
            {
                Controls[0].Text = polish ? "Celowniki społeczności" : "Community crosshairs";
            }

            refreshButton.Text = polish ? "Odśwież" : "Refresh";
            applyButton.Text = polish ? "Zastosuj" : "Apply";
            shareButton.Text = polish ? "Udostępnij" : "Share";
            statusLabel.Text = polish ? "Lista nie została jeszcze pobrana." : "The list has not been loaded yet.";
            UpdateSelectionDetails();
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
