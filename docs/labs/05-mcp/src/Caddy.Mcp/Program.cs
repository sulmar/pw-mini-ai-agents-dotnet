using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Caddy.Mcp;

var builder = Host.CreateApplicationBuilder(args);
// stdout jest przeznaczone wyłącznie na komunikaty protokołu MCP.
builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Trace);
builder.Services.AddHttpClient<CaddyAdminClient>(client =>
{
    client.BaseAddress = new Uri("http://127.0.0.1:2019/");
    client.Timeout = TimeSpan.FromSeconds(5);
});
builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();
await builder.Build().RunAsync();
