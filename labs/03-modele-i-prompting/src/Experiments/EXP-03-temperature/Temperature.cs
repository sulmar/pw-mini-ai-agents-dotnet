using System.Diagnostics;

// EXP-03: wpływ temperatury na poprawność i powtarzalność
public static class Temperature
{
    // Ten sam model i prompt - zmieniamy tylko temperaturę
    private const string Model = "gemma3:4b";

    private const int Repetitions = 5;

    private static readonly double[] Temperatures = [0.0, 0.2, 1.0];

    public static async Task RunAsync(OllamaClient client)
    {
        var notes = TestData.Examples;

        foreach (var temperature in Temperatures)
        {
            Console.WriteLine($"\nTemperature: {temperature}");

            var metrics = new Metrics();
            var stableNotes = 0;

            foreach (var note in notes)
            {
                var answers = new List<string>();

                for (var i = 0; i < Repetitions; i++)
                {
                    var watch = Stopwatch.StartNew();

                    var response = await client.GenerateAsync(
                        Model,
                        Extraction.Prompt(note.Note),
                        temperature);

                    watch.Stop();

                    var result = Extraction.Parse(response, out var error);

                    metrics.Add(note, result, watch.ElapsedMilliseconds);
                    answers.Add(result?.ToString() ?? error!);
                }

                var distinct = answers.Distinct().Count();

                if (distinct == 1)
                    stableNotes++;

                Console.WriteLine(
                    $"{note.Id} \"{note.Note}\" => różnych odpowiedzi: {distinct}" +
                    $" (oczekiwano: {new ExtractionResult(note.Floor, note.Elevator)})");

                foreach (var answer in answers.Distinct())
                    Console.WriteLine($"    {answers.Count(a => a == answer)}x {answer}");
            }

            Console.WriteLine(metrics);
            Console.WriteLine($"Powtarzalne notatki: {stableNotes}/{notes.Count}");

            ResultsCsv.Append("EXP-03", Model, $"x{Repetitions}", temperature, metrics);
        }
    }
}
