using System.Net.Http.Json;

using var http = new HttpClient
{
    BaseAddress = new Uri("http://localhost:11434")
};

var situation = """
    Student chce kawę.
    Automat nie działa.
    Kawiarnia jest otwarta.
    Student ma 5 minut do zajęć.
    """;

// 1. Obserwacja
Console.WriteLine($"Środowisko:\n{situation}");

// 2. Decyzja podejmowana przez LLM
var response = await http.PostAsJsonAsync("/api/generate", new
{
    model = "qwen2.5:3b",
    stream = false,
    prompt = $"""
        Jesteś agentem na kampusie uczelni.

        Sytuacja:
        {situation}

        Wybierz jedno działanie:
        AUTOMAT, KAWIARNIA albo REZYGNUJ.

        Odpowiedz wyłącznie nazwą działania.
        """
});

response.EnsureSuccessStatusCode();

var result = await response.Content
    .ReadFromJsonAsync<OllamaResponse>();

var decision = result?.Response.Trim().ToUpperInvariant();

// 3. Działanie wykonywane przez aplikację
switch (decision)
{
    case "AUTOMAT":
        Console.WriteLine("Agent idzie do automatu.");
        break;

    case "KAWIARNIA":
        Console.WriteLine("Agent kieruje studenta do kawiarni.");
        break;

    case "REZYGNUJ":
        Console.WriteLine("Agent proponuje zrezygnować z kawy.");
        break;

    default:
        Console.WriteLine("Nie rozpoznano decyzji modelu.");
        break;
}

record OllamaResponse(string Response);
