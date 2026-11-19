var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://127.0.0.1:5102");
var app = builder.Build();

app.MapGet("/", () => new { service = "api-b", message = "Hello from api-b!" });
app.MapGet("/health", () => new { service = "api-b", status = "ok" });
app.MapGet("/api/products", () => new
{
    service = "api-b",
    products = new[] { new { id = 1, name = "Monitor" } }
});

app.Run();
