namespace CodeExecutionLab.Runtimes;

/// <summary>
/// Runtime uruchamiający Python w zewnętrznym procesie (python3).
/// Kod przekazywany przez stdin; <c>-I</c> ignoruje zmienne PYTHON* i katalog użytkownika.
/// </summary>
public sealed class PythonRuntime : ICodeRuntime
{
    public Task<ExecutionResult> ExecuteAsync(ExecutionRequest request, CancellationToken cancellationToken)
    {
        if (request.Language != CodeLanguage.Python)
        {
            return Task.FromResult(ExecutionResult.Rejected($"Nieobsługiwany język: {request.Language}."));
        }

        return ProcessExecutor.RunAsync("python3", ["-I", "-"], request.Code, cancellationToken);
    }
}
