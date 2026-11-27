namespace CodeExecutionLab.Runtimes;

public sealed class LuaRuntime : ICodeRuntime
{
    public Task<ExecutionResult> ExecuteAsync(ExecutionRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException("TODO: LuaRuntime — implementacja ExecuteAsync.");

    }
}
