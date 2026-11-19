var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://127.0.0.1:5101");
var app = builder.Build();

app.MapGet("/", () => new { service = "api-a", message = "Hello from api-a!" });
app.MapGet("/health", () => new { service = "api-a", status = "ok" });
app.MapGet("/api/products", () => new
{
    service = "api-a",
    products = new[] { new { id = 1, name = "Keyboard" } }
});

app.Run();
