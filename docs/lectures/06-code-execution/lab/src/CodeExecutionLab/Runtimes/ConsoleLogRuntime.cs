namespace CodeExecutionLab.Runtimes;

public sealed class ConsoleLogRuntime(ICodeRuntime inner) : ICodeRuntime
{
    public Task<ExecutionResult> ExecuteAsync(ExecutionRequest request, CancellationToken cancellationToken)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Language: {request.Language} Code: {request.Code}");
        Console.ResetColor();
        return inner.ExecuteAsync(request, cancellationToken);
    }
}
