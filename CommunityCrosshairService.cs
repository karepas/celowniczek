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
        public const string NewSubmissionIssueUrl = "https://github.com/karepas/celowniczek/issues/new";
        public const string ApprovalLabel = "community-approved";

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

        public static Uri CreateSubmissionIssueUri(CommunityCrosshair crosshair)
        {
            if (crosshair == null || !crosshair.IsValid())
            {
                throw new ArgumentException("The crosshair submission contains invalid values.", nameof(crosshair));
            }

            string crosshairJson = JsonSerializer.Serialize(crosshair, JsonOptions);
            string title = $"Crosshair submission: {crosshair.Title.Trim()}";
            string body = string.Join("\n", new[]
            {
                "Community crosshair submission",
                "",
                "Please review this crosshair. A repository maintainer can add the `community-approved` label to publish it in the in-app list.",
                "",
                "```json",
                crosshairJson,
                "```"
            });
            string issueUrl = $"{NewSubmissionIssueUrl}?title={Uri.EscapeDataString(title)}&body={Uri.EscapeDataString(body)}";
            return new Uri(issueUrl, UriKind.Absolute);
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
    }
}
