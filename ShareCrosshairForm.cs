using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Celowniczek
{
    public class ShareCrosshairForm : Form
    {
        private readonly ConfigData config;
        private readonly TextBox authorTextBox = new TextBox();
        private readonly TextBox titleTextBox = new TextBox();
        private readonly Button sendButton = new Button();
        private readonly Button cancelButton = new Button();
        private readonly Label statusLabel = new Label();
        private readonly Label authorLabel = new Label();
        private readonly Label titleLabel = new Label();

        public ShareCrosshairForm(ConfigData config)
        {
            this.config = config;
            bool polish = config.Language == "pl";
            Text = polish ? "Udostępnij celownik" : "Share crosshair";
            ClientSize = new Size(390, 220);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            authorLabel.Location = new Point(18, 18);
            authorLabel.Size = new Size(350, 20);
            authorLabel.Text = polish ? "Autor" : "Author";

            authorTextBox.Location = new Point(18, 40);
            authorTextBox.Size = new Size(350, 26);
            authorTextBox.MaxLength = 40;

            titleLabel.Location = new Point(18, 76);
            titleLabel.Size = new Size(350, 20);
            titleLabel.Text = polish ? "Nazwa celownika" : "Crosshair name";

            titleTextBox.Location = new Point(18, 98);
            titleTextBox.Size = new Size(350, 26);
            titleTextBox.MaxLength = 64;

            statusLabel.Location = new Point(18, 132);
            statusLabel.Size = new Size(350, 42);

            sendButton.Location = new Point(178, 177);
            sendButton.Size = new Size(90, 30);
            sendButton.Text = polish ? "Wyślij" : "Send";
            sendButton.Click += async (s, e) => await SendAsync();

            cancelButton.Location = new Point(278, 177);
            cancelButton.Size = new Size(90, 30);
            cancelButton.Text = polish ? "Anuluj" : "Cancel";
            cancelButton.DialogResult = DialogResult.Cancel;

            Controls.Add(authorLabel);
            Controls.Add(authorTextBox);
            Controls.Add(titleLabel);
            Controls.Add(titleTextBox);
            Controls.Add(statusLabel);
            Controls.Add(sendButton);
            Controls.Add(cancelButton);
            AcceptButton = sendButton;
            CancelButton = cancelButton;
        }

        private async Task SendAsync()
        {
            string author = authorTextBox.Text.Trim();
            string title = titleTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(author) || string.IsNullOrWhiteSpace(title))
            {
                statusLabel.Text = config.Language == "pl"
                    ? "Wpisz autora i nazwę celownika."
                    : "Enter both an author and a crosshair name.";
                return;
            }

            CommunityCrosshair submission = new CommunityCrosshair
            {
                Title = title,
                Author = author,
                Size = config.Size,
                Thickness = config.Thickness,
                Gap = config.Gap,
                DotRadius = config.DotRadius,
                ColorHex = config.ColorHex,
                EnableOutline = config.EnableOutline,
                Style = config.Style
            };

            sendButton.Enabled = false;
            statusLabel.Text = config.Language == "pl" ? "Wysyłanie zgłoszenia..." : "Sending submission...";
            try
            {
                await CommunityCrosshairService.SubmitCrosshairAsync(submission);
                statusLabel.Text = config.Language == "pl" ? "Wysłano zgłoszenie." : "Submission sent.";
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                if (ex is InvalidOperationException)
                {
                    statusLabel.Text = config.Language == "pl"
                        ? "Webhook Discord nie jest skonfigurowany. Właściciel aplikacji musi ustawić DiscordWebhookUrl przed kompilacją."
                        : "The Discord webhook is not configured. The app owner must set DiscordWebhookUrl before building.";
                }
                else
                {
                    statusLabel.Text = config.Language == "pl"
                        ? $"Nie udało się wysłać: {ex.Message}"
                        : $"Could not send: {ex.Message}";
                }
            }
            finally
            {
                if (!IsDisposed)
                {
                    sendButton.Enabled = true;
                }
            }
        }
    }
}
