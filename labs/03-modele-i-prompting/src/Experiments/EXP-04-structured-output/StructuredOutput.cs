// EXP-04: ekstrakcja danych do JSON i walidacja
public static class StructuredOutput
{
    private const string Model = "gemini-3.5-flash-lite";

    public static async Task RunAsync(GeminiClient client)
    {
        // Format JSON opisany tylko w prompcie vs natywny tryb JSON Schema w Ollama
        var variants = new (string Name, object? Format)[]
{
            ("prompt", null),
            ("json-mode", Extraction.JsonMode),
            ("json-schema", Extraction.Schema)
};

        foreach (var (name, format) in variants)
        {
            Console.WriteLine($"\nWariant: {name}");

            var metrics = await Extraction.EvaluateAsync(
                client,
                Model,
                TestData.Examples,
                note => Extraction.Prompt(note),
                format: format);

            ResultsCsv.Append("EXP-04", Model, name, 0, metrics);
        }
    }
}
