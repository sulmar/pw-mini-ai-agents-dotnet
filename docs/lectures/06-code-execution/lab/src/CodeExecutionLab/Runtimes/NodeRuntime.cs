namespace CodeExecutionLab.Runtimes;

/// <summary>
/// Runtime JavaScript w zewnętrznym procesie Node.js.
/// Node bez argumentów wykonuje skrypt podany na stdin.
/// Uwaga: Node w procesie ma dostęp do systemu plików i sieci hosta — to nie jest izolacja.
/// </summary>
public sealed class NodeRuntime : ICodeRuntime
{
    public Task<ExecutionResult> ExecuteAsync(ExecutionRequest request, CancellationToken cancellationToken)
    {
        if (request.Language != CodeLanguage.JavaScript)
        {
            return Task.FromResult(ExecutionResult.Rejected($"Nieobsługiwany język: {request.Language}."));
        }

        return ProcessExecutor.RunAsync("node", [], request.Code, cancellationToken);
    }
}
