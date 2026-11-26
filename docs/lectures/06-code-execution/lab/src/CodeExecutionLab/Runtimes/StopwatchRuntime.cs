using System.Diagnostics;

namespace CodeExecutionLab.Runtimes;

/// <summary>
/// Dekorator mierzący czas całej operacji i uzupełniający <see cref="ExecutionResult.DurationMs"/>.
/// Nie zawiera żadnej logiki specyficznej dla języka.
/// </summary>
public sealed class StopwatchRuntime(ICodeRuntime inner) : ICodeRuntime
{
    public async Task<ExecutionResult> ExecuteAsync(ExecutionRequest request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await inner.ExecuteAsync(request, cancellationToken);
        stopwatch.Stop();

        return result with { DurationMs = stopwatch.ElapsedMilliseconds };
    }
}
