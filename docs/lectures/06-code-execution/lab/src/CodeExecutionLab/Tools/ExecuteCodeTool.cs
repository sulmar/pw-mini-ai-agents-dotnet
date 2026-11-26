using System.ComponentModel;
using CodeExecutionLab.Runtimes;

namespace CodeExecutionLab.Tools;

/// <summary>
/// Narzędzie udostępniane agentowi. Egzekwuje ograniczenia niezależne od runtime'u:
/// długość kodu, limit prób na jedno zadanie i długość wyniku zwracanego do modelu.
/// </summary>
public sealed class ExecuteCodeTool(ICodeRuntime runtime, ExecutionLimits? limits = null)
{
    private readonly ExecutionLimits _limits = limits ?? new ExecutionLimits();
    private int _attempts;

    /// <summary>Wywołaj przed każdą nową prośbą użytkownika — resetuje licznik prób.</summary>
    public void StartNewTask() => _attempts = 0;

    [Description("Wykonuje kod w wybranym języku i zwraca stdout, stderr oraz kod wyjścia. " +
                 "Program musi wypisać wynik na standardowe wyjście (print / console.log).")]
    public async Task<string> ExecuteCodeAsync(
        [Description("Język kodu: 'python' albo 'javascript'.")] string language,
        [Description("Kod źródłowy do wykonania. Bez dostępu do sieci i plików.")] string code,
        CancellationToken cancellationToken = default)
    {
        if (_attempts >= _limits.MaxAttemptsPerTask)
        {
            return $"LIMIT_PROB: wykorzystano {_limits.MaxAttemptsPerTask} próby dla tego zadania. " +
                   "Nie wywołuj narzędzia ponownie; poinformuj użytkownika o problemie.";
        }

        _attempts++;

        if (!Enum.TryParse<CodeLanguage>(language, ignoreCase: true, out var parsed)
            && !(language.Equals("js", StringComparison.OrdinalIgnoreCase)
                 && (parsed = CodeLanguage.JavaScript) == CodeLanguage.JavaScript))
        {
            return Format(ExecutionResult.Rejected($"Nieobsługiwany język: '{language}'."));
        }

        if (code.Length > _limits.MaxCodeChars)
        {
            return Format(ExecutionResult.Rejected(
                $"Kod za długi ({code.Length} znaków, limit {_limits.MaxCodeChars})."));
        }

        var request = new ExecutionRequest(parsed, code, _limits.Timeout);

        try
        {
            var result = await runtime.ExecuteAsync(request, cancellationToken);
            return Format(result);
        }
        catch (NotImplementedException)
        {
            // Szkielet runtime'u jeszcze nieuzupełniony — nie ma sensu ponawiać.
            return "RUNTIME_NIEZAIMPLEMENTOWANY: wybrany runtime nie ma implementacji ExecuteAsync. " +
                   "Nie ponawiaj wykonania; poinformuj użytkownika.";
        }
    }

    private string Format(ExecutionResult result)
    {
        return $"""
            Status: {result.Status}
            ExitCode: {result.ExitCode?.ToString() ?? "-"}
            CzasMs: {result.DurationMs}
            Stdout:
            {Truncate(result.Stdout)}
            Stderr:
            {Truncate(result.Stderr)}
            """;
    }

    private string Truncate(string text)
    {
        if (text.Length <= _limits.MaxOutputChars)
        {
            return text;
        }

        return text[.._limits.MaxOutputChars] + $"\n...[obcięto, pozostało {text.Length - _limits.MaxOutputChars} znaków]";
    }
}
