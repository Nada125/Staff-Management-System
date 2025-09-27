using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StaffManagementSystem.Application.DTOs.Auth;
using StaffManagementSystem.Application.Interfaces;
using System.Text;
using System.Text.Json;


namespace StaffManagementSystem.Application.Services
{
    public class ReportAiService : IReportAiService
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly ILogger<ReportAiService> _logger;
        private readonly GoogleAiOptions _opts;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        public ReportAiService(IHttpClientFactory httpFactory, IOptions<GoogleAiOptions> opts, ILogger<ReportAiService> logger)
        {
            _httpFactory = httpFactory;
            _logger = logger;
            _opts = opts.Value;
        }

        public async Task<string> SummarizeAsync(string reportText, CancellationToken cancellationToken = default)
        {
            var chunks = SplitIntoChunks(reportText, 2500);
            var partialSummaries = new List<string>();

            foreach (var chunk in chunks)
            {
                var prompt = $"{chunk}";
                var summary = await CallGenerateAsync(prompt, cancellationToken);
                partialSummaries.Add(summary.Trim());
            }

            if (partialSummaries.Count == 1)
                return partialSummaries[0];

            var combinedPrompt = string.Join(" ", partialSummaries);
            return await CallGenerateAsync(combinedPrompt, cancellationToken);
        }

        private async Task<string> CallGenerateAsync(string prompt, CancellationToken cancellationToken)
        {
            var client = _httpFactory.CreateClient("google-ai");
            var url = $"{_opts.BaseUrl}/v1beta/models/{_opts.Model}:generateContent?key={_opts.ApiKey}";

            var body = new
            {
                contents = new[]
                {
            new
            {
                role = "user",
                parts = new[]
                {
                    new { text = prompt }
                }
            }
        },
                generationConfig = new
                {
                    temperature = _opts.Temperature,
                    maxOutputTokens = _opts.MaxOutputTokens
                }
            };

            var req = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonSerializer.Serialize(body, _jsonOptions), Encoding.UTF8, "application/json")
            };
            var jsonBody = JsonSerializer.Serialize(body, _jsonOptions);

            var res = await client.SendAsync(req, cancellationToken);
            var payload = await res.Content.ReadAsStringAsync(cancellationToken);

            if (!res.IsSuccessStatusCode)
            {
                _logger.LogWarning("Google AI returned {Status}: {Payload}", res.StatusCode, payload);
                throw new HttpRequestException($"Google AI API failed: {payload}");
            }

            using var doc = JsonDocument.Parse(payload);
            return TryExtractGeneratedText(doc, out var text) ? text : payload;
        }

        private bool TryExtractGeneratedText(JsonDocument doc, out string text)
        {
            text = "";

            if (doc.RootElement.TryGetProperty("candidates", out var candidates) && candidates.ValueKind == JsonValueKind.Array && candidates.GetArrayLength() > 0)
            {
                var contentObj = candidates[0].GetProperty("content");

                if (contentObj.TryGetProperty("parts", out var parts) && parts.ValueKind == JsonValueKind.Array && parts.GetArrayLength() > 0)
                {
                    var firstPart = parts[0];
                    if (firstPart.TryGetProperty("text", out var t))
                    {
                        text = t.GetString() ?? "";
                        return true;
                    }
                }
            }

            return false;
        }

        private List<string> SplitIntoChunks(string text, int approxCharsPerChunk)
        {
            if (string.IsNullOrWhiteSpace(text)) return new List<string> { "" };
            var sentences = text.Split(new[] { '.', '-' }, StringSplitOptions.RemoveEmptyEntries);
            var chunks = new List<string>();
            var sb = new StringBuilder();
            foreach (var s in sentences)
            {
                if (sb.Length + s.Length + 1 > approxCharsPerChunk)
                {
                    chunks.Add(sb.ToString());
                    sb.Clear();
                }
                sb.Append(s.Trim()).Append(".");
            }
            if (sb.Length > 0) chunks.Add(sb.ToString());
            return chunks;
        }
    }
}
