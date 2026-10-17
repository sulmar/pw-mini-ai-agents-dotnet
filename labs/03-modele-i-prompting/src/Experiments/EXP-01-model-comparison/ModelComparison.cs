// EXP-01: porównanie dwóch modeli w ekstrakcji piętra i windy
public static class ModelComparison
{
    private static readonly string[] Models =
    [
        "gemini-3.5-flash-lite",
        "gemini-3.8-flash"
    ];

    public static async Task RunAsync(GeminiClient client)
    {
        foreach (var model in Models)
        {
            Console.WriteLine($"\nModel: {model}");

            var metrics = await Extraction.EvaluateAsync(
                client,
                model,
                TestData.Examples,
                note => Extraction.Prompt(note),
                format: Extraction.JsonMode);

            ResultsCsv.Append("EXP-01", model, "zero-shot", 0, metrics);
        }
    }
}
