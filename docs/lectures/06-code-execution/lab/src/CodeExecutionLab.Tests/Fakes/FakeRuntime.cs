using CodeExecutionLab.Runtimes;

namespace CodeExecutionLab.Tests.Fakes;

/// <summary>Fikcyjny runtime do testów dekoratorów — bez interpretera Python/Node.</summary>
public sealed class FakeRuntime(Func<ExecutionRequest, CancellationToken, Task<ExecutionResult>> behaviour) : ICodeRuntime
{
    public int Calls { get; private set; }

    public Task<ExecutionResult> ExecuteAsync(ExecutionRequest request, CancellationToken cancellationToken)
    {
        Calls++;
        return behaviour(request, cancellationToken);
    }

    public static FakeRuntime Returning(ExecutionResult result) =>
        new((_, _) => Task.FromResult(result));

    /// <summary>Symuluje runtime, który honoruje anulowanie i kończy się sam.</summary>
    public static FakeRuntime RespectingCancellation() => new(async (_, ct) =>
    {
        try
        {
            await Task.Delay(Timeout.Infinite, ct);
            return new ExecutionResult(ExecutionStatus.Succeeded, 0, "", "");
        }
        catch (OperationCanceledException)
        {
            return ExecutionResult.Cancelled();
        }
    });

    /// <summary>Symuluje runtime, który ignoruje anulowanie (naruszenie kontraktu).</summary>
    public static FakeRuntime IgnoringCancellation() => new(async (_, _) =>
    {
        await Task.Delay(TimeSpan.FromSeconds(30));
        return new ExecutionResult(ExecutionStatus.Succeeded, 0, "", "");
    });
}
