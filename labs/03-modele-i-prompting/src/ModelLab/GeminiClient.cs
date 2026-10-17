using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

public sealed class GeminiClient(HttpClient http, string apiKey)
{
    private static readonly JsonSerializerOptions Options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private static readonly TimeSpan MinInterval = TimeSpan.FromMilliseconds(
        int.TryParse(Environment.GetEnvironmentVariable("GEMINI_DELAY_MS"), out var ms) ? ms : 6000);

    private const int MaxRetries = 5;

    private DateTime _lastRequest = DateTime.MinValue;

    public long LastLatencyMs { get; private set; }

    // format: null (zwykły tekst), "json" (tryb JSON bez schematu) albo obiekt JSON Schema
    public async Task<string> GenerateAsync(
        string model,
        string prompt,
        double temperature = 0,
        object? format = null)
    {
        var request = new
        {
            contents = new[]
            {
                new { role = "user", parts = new[] { new { text = prompt } } }
            },
            generationConfig = new
            {
                temperature,
                responseMimeType = format is null ? null : "application/json",
                responseJsonSchema = format is string ? null : format
            }
        };

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent";

        for (var attempt = 1; ; attempt++)
        {
            await ThrottleAsync();

            using var message = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(request, options: Options)
            };
            message.Headers.Add("x-goog-api-key", apiKey);

            var watch = Stopwatch.StartNew();
            using var response = await http.SendAsync(message);
            watch.Stop();

            if (response.StatusCode is HttpStatusCode.TooManyRequests
                    or HttpStatusCode.ServiceUnavailable
                    or HttpStatusCode.InternalServerError
                && attempt < MaxRetries)
            {
                var wait = TimeSpan.FromSeconds(10 * attempt);
                Console.WriteLine($"    [{(int)response.StatusCode}] czekam {wait.TotalSeconds:F0} s i ponawiam...");
                await Task.Delay(wait);
                continue;
            }

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(
                    $"Gemini API {(int)response.StatusCode}: {await response.Content.ReadAsStringAsync()}");

            LastLatencyMs = watch.ElapsedMilliseconds;

            using var json = await response.Content.ReadFromJsonAsync<JsonDocument>();

            return ExtractText(json!.RootElement);
        }
    }

    // Odpowiedź: candidates[0].content.parts[]
    private static string ExtractText(JsonElement root)
    {
        if (!root.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
            return "";

        var candidate = candidates[0];

        if (!candidate.TryGetProperty("content", out var content) ||
            !content.TryGetProperty("parts", out var parts))
            return "";

        return string.Concat(parts.EnumerateArray()
            .Where(p => !(p.TryGetProperty("thought", out var t) && t.ValueKind == JsonValueKind.True))
            .Where(p => p.TryGetProperty("text", out _))
            .Select(p => p.GetProperty("text").GetString()))
            .Trim();
    }

    private async Task ThrottleAsync()
    {
        var wait = _lastRequest + MinInterval - DateTime.UtcNow;

        if (wait > TimeSpan.Zero)
            await Task.Delay(wait);

        _lastRequest = DateTime.UtcNow;
    }
}