using System;
using System.Diagnostics;
using System.Drawing;
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
            sendButton.Tag = "primary";
            sendButton.Text = polish ? "Wyślij" : "Send";
            sendButton.Click += (s, e) => SubmitToGitHub();

            cancelButton.Location = new Point(278, 177);
            cancelButton.Size = new Size(90, 30);
            cancelButton.Tag = "secondary";
            cancelButton.Text = polish ? "Anuluj" : "Cancel";
            cancelButton.DialogResult = DialogResult.Cancel;
            sendButton.Enabled = true;
            authorTextBox.TabIndex = 0;
            titleTextBox.TabIndex = 1;
            sendButton.TabIndex = 2;
            cancelButton.TabIndex = 3;

            Controls.Add(authorLabel);
            Controls.Add(authorTextBox);
            Controls.Add(titleLabel);
            Controls.Add(titleTextBox);
            Controls.Add(statusLabel);
            Controls.Add(sendButton);
            Controls.Add(cancelButton);
            AcceptButton = sendButton;
            CancelButton = cancelButton;
            UiThemeManager.Apply(this, UiThemeManager.GetTheme(config.Theme));
        }

        private void SubmitToGitHub()
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

            try
            {
                Uri issueUri = CommunityCrosshairService.CreateSubmissionIssueUri(submission);
                Process.Start(new ProcessStartInfo
                {
                    FileName = issueUri.AbsoluteUri,
                    UseShellExecute = true
                });
                statusLabel.ForeColor = Color.DarkGreen;
                statusLabel.Text = config.Language == "pl"
                    ? "Otworzyłem GitHub. Zaloguj się i kliknij „Submit new issue”. Po zatwierdzeniu zgłoszenie pojawi się na liście społeczności."
                    : "GitHub is open. Sign in and click “Submit new issue”. The crosshair appears in the community list after approval.";
            }
            catch (Exception ex)
            {
                statusLabel.ForeColor = Color.DarkRed;
                statusLabel.Text = config.Language == "pl"
                    ? $"Nie udało się otworzyć zgłoszenia GitHub: {ex.Message}"
                    : $"Could not open the GitHub submission: {ex.Message}";
            }
        }
    }
}
