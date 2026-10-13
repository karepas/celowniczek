using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Celowniczek
{
    public static class CommunityCrosshairService
    {
        public const string RawCrosshairsUrl = "https://raw.githubusercontent.com/karepas/celowniczek/main/community/crosshairs.json";
        public const string DiscordWebhookUrl = "YOUR_DISCORD_WEBHOOK_URL";

        private static readonly HttpClient HttpClient = CreateHttpClient();
        private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

        public static async Task<IReadOnlyList<CommunityCrosshair>> GetCrosshairsAsync(CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, RawCrosshairsUrl);
            request.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };

            using HttpResponseMessage response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();

            await using System.IO.Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            List<CommunityCrosshair>? entries = await JsonSerializer.DeserializeAsync<List<CommunityCrosshair>>(stream, JsonOptions, cancellationToken);
            if (entries == null)
            {
                throw new JsonException("The community crosshair file does not contain a JSON array.");
            }

            return entries
                .Where(entry => entry != null && entry.IsValid())
                .OrderBy(entry => entry.Title, StringComparer.CurrentCultureIgnoreCase)
                .ToArray();
        }

        public static async Task SubmitCrosshairAsync(CommunityCrosshair crosshair, CancellationToken cancellationToken = default)
        {
            if (crosshair == null || !crosshair.IsValid())
            {
                throw new ArgumentException("The crosshair submission contains invalid values.", nameof(crosshair));
            }

            if (!Uri.TryCreate(DiscordWebhookUrl, UriKind.Absolute, out Uri? webhookUri)
                || !string.Equals(webhookUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)
                || !(string.Equals(webhookUri.Host, "discord.com", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(webhookUri.Host, "discordapp.com", StringComparison.OrdinalIgnoreCase))
                || !webhookUri.AbsolutePath.StartsWith("/api/webhooks/", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Discord webhook is not configured. Replace YOUR_DISCORD_WEBHOOK_URL in CommunityCrosshairService.cs with a Discord webhook URL before enabling submissions.");
            }

            string crosshairJson = JsonSerializer.Serialize(crosshair, JsonOptions);
            DiscordWebhookPayload payload = new DiscordWebhookPayload
            {
                Content = $"New community crosshair submission:\n```json\n{crosshairJson}\n```"
            };
            string payloadJson = JsonSerializer.Serialize(payload);

            using StringContent content = new StringContent(payloadJson, System.Text.Encoding.UTF8, "application/json");
            using HttpResponseMessage response = await HttpClient.PostAsync(webhookUri, content, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        private static HttpClient CreateHttpClient()
        {
            HttpClient client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(15)
            };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Celowniczek-Community/1.0");
            return client;
        }

        private static JsonSerializerOptions CreateJsonOptions()
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }

        private sealed class DiscordWebhookPayload
        {
            public string Content { get; set; } = "";
        }
    }
}
