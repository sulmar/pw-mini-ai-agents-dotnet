// EXP-02: wpływ zero-shot, one-shot i few-shot na jakość ekstrakcji
public static class Prompting
{
    private const string Model = "gemini-3.5-flash-lite";

    public static async Task RunAsync(GeminiClient client)
    {
        var examples = TestData.PromptExamples;

        var variants = new (string Name, Func<string, string> Prompt)[]
        {
            ("zero-shot", note => Extraction.Prompt(note)),
            ("one-shot", note => Extraction.Prompt(note, examples.Take(1))),
            ("few-shot", note => Extraction.Prompt(note, examples))
        };

        foreach (var (name, buildPrompt) in variants)
        {
            Console.WriteLine($"\nWariant: {name}");

            var metrics = await Extraction.EvaluateAsync(
    client,
    Model,
    TestData.Examples,
    buildPrompt,
    format: Extraction.JsonMode);

            ResultsCsv.Append("EXP-02", Model, name, 0, metrics);
        }
    }
}
