namespace CodeExecutionLab.Runtimes;

/// <summary>
/// Wspólny runtime procesowy: profil języka (co uruchomić) + owijacz (gdzie uruchomić).
/// Dzięki temu ten sam język działa zarówno na hoście, jak i w kontenerze Docker.
/// </summary>
public sealed class ProcessRuntime(LanguageProfile profile, ICommandWrapper wrapper) : ICodeRuntime
{
    public async Task<ExecutionResult> ExecuteAsync(ExecutionRequest request, CancellationToken cancellationToken)
    {
        if (request.Language != profile.Language)
        {
            return ExecutionResult.Rejected($"Nieobsługiwany język: {request.Language}.");
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return ExecutionResult.Cancelled();
        }

        var prepared = wrapper.Prepare(profile.ToCommand(request.Code), profile.Image);

        try
        {
            return await ProcessExecutor.RunAsync(
                prepared.FileName,
                prepared.Arguments,
                prepared.Input,
                cancellationToken,
                prepared.OnCancelled,
                prepared.WorkingDirectory,
                prepared.Environment);
        }
        finally
        {
            prepared.Cleanup();
        }
    }
}
