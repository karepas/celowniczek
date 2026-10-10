using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Celowniczek
{
    public class GitHubRelease
    {
        public string tag_name { get; set; } = "";
        public string html_url { get; set; } = "";
        public string body { get; set; } = "";
    }

    public static class UpdateChecker
    {
        public static readonly string CurrentVersion = "1.0.3";

        // Ustawiony Twój nick: karepas
        private static readonly string GitHubApiUrl = "https://api.github.com/repos/karepas/celowniczek/releases/latest";

        public static async Task CheckForUpdatesAsync(bool silent = true, string language = "en")
        {
            bool polish = language == "pl";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    client.DefaultRequestHeaders.Add("User-Agent", "Celowniczek-App");

                    string json = await client.GetStringAsync(GitHubApiUrl);
                    GitHubRelease? release = JsonSerializer.Deserialize<GitHubRelease>(json);

                    if (release != null && !string.IsNullOrEmpty(release.tag_name))
                    {
                        string cleanTag = release.tag_name.TrimStart('v', 'V');

                        Version current = new Version(CurrentVersion);
                        Version latest = new Version(cleanTag);

                        if (latest > current)
                        {
                            DialogResult result = MessageBox.Show(
                                polish
                                    ? $"Dostępna jest nowa wersja Celowniczka ({cleanTag})!\n\nLista zmian:\n{release.body}\n\nCzy chcesz otworzyć stronę pobierania na GitHubie?"
                                    : $"A new version of Celowniczek is available ({cleanTag})!\n\nWhat's new:\n{release.body}\n\nWould you like to open the GitHub download page?",
                                polish ? "Aktualizacja Celowniczka" : "Celowniczek update",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information
                            );

                            if (result == DialogResult.Yes && !string.IsNullOrEmpty(release.html_url))
                            {
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = release.html_url,
                                    UseShellExecute = true
                                });
                            }
                        }
                        else if (!silent)
                        {
                            MessageBox.Show(
                                polish ? "Używasz najnowszej wersji Celowniczka!" : "You are using the latest version of Celowniczek!",
                                polish ? "Brak aktualizacji" : "No updates available",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (!silent)
                {
                    MessageBox.Show(
                        polish ? $"Nie udało się sprawdzić aktualizacji: {ex.Message}" : $"Could not check for updates: {ex.Message}",
                        polish ? "Błąd" : "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }
    }
}