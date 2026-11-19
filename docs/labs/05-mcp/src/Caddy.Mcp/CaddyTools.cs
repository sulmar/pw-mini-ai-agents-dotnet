using System.ComponentModel;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace Caddy.Mcp;

[McpServerToolType]
public sealed class CaddyTools
{
    private readonly CaddyAdminClient caddy;

    public CaddyTools(CaddyAdminClient caddy) => this.caddy = caddy;

    // Działający przykład: sprawdź odkrywanie i wywołanie narzędzia w Inspectorze.
    [McpServerTool(Name = "get_lab_info"), Description("Zwraca adresy usług lokalnego laboratorium Caddy.")]
    public string GetLabInfo() =>
        "Proxy: http://127.0.0.1:8080; admin: http://127.0.0.1:2019; " +
        "api-a: http://127.0.0.1:5101; api-b: http://127.0.0.1:5102.";

    [McpServerTool(Name = "get_configuration"), Description("Odczytuje bieżącą konfigurację Caddy w formacie JSON.")]
    public Task<string> GetConfiguration(CancellationToken cancellationToken)
    {
        // TODO 1: wywołaj odpowiednią metodę caddy, przekaż cancellationToken.
        throw new McpException("TODO 1: zaimplementuj get_configuration.");
    }

    [McpServerTool(Name = "get_active_backend"), Description("Zwraca upstreamy reverse proxy lab-proxy w formacie JSON.")]
    public Task<string> GetActiveBackend(CancellationToken cancellationToken)
    {
        // TODO 2: odczytaj upstreamy przez gotowego klienta HTTP.
        throw new McpException("TODO 2: zaimplementuj get_active_backend.");
    }

    [McpServerTool(Name = "switch_backend"), Description("Przełącza reverse proxy na lokalne api-a lub api-b. Zmienia konfigurację Caddy.")]
    public Task<string> SwitchBackend(
        [Description("Nazwa backendu: api-a albo api-b.")] string backend,
        CancellationToken cancellationToken)
    {
        // TODO 3: zaakceptuj wyłącznie api-a lub api-b; inne wartości -> McpException.
        // Zmapuj nazwę na 127.0.0.1:5101 lub 127.0.0.1:5102.
        // Użyj SetUpstreamsAsync. Nie przyjmuj dowolnego URL od użytkownika.
        throw new McpException("TODO 3: zaimplementuj switch_backend.");
    }
}
