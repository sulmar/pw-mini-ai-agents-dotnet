using System.Net.Http.Json;
using ModelContextProtocol;

namespace Caddy.Mcp;

// Gotowa warstwa HTTP. Zadaniem studentów jest udostępnienie jej przez MCP.
public sealed class CaddyAdminClient(HttpClient http)
{
    public Task<string> GetConfigurationAsync(CancellationToken cancellationToken) =>
        SendAsync(HttpMethod.Get, "config/", null, cancellationToken);

    public Task<string> GetUpstreamsAsync(CancellationToken cancellationToken) =>
        SendAsync(HttpMethod.Get, "id/lab-proxy/upstreams", null, cancellationToken);

    public Task<string> SetUpstreamsAsync(string dial, CancellationToken cancellationToken) =>
        SendAsync(HttpMethod.Patch, "id/lab-proxy/upstreams",
            JsonContent.Create(new[] { new { dial } }), cancellationToken);

    private async Task<string> SendAsync(HttpMethod method, string path,
        HttpContent? content, CancellationToken cancellationToken)
    {
        try
        {
            using var request = new HttpRequestMessage(method, path) { Content = content };
            using var response = await http.SendAsync(request, cancellationToken);
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!response.IsSuccessStatusCode)
                throw new McpException($"Caddy zwrócił HTTP {(int)response.StatusCode}: {body}");
            return string.IsNullOrWhiteSpace(body) ? "OK" : body;
        }
        catch (HttpRequestException)
        {
            throw new McpException("Brak połączenia z Caddy. Uruchom Caddy na 127.0.0.1:2019.");
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new McpException("Przekroczono czas oczekiwania na Caddy (5 s).");
        }
    }
}
