namespace CodeExecutionLab.Runtimes;

/// <summary>
/// Runtime uruchamiający Python w jednorazowym kontenerze Docker (wariant zaawansowany, opcjonalny).
/// Kontener bez sieci, z limitem pamięci i CPU, bez montowania katalogów hosta.
/// Przy anulowaniu klient <c>docker</c> jest zabijany, a kontener kończony przez <c>docker kill</c>.
/// </summary>
public sealed class DockerRuntime : ICodeRuntime
{
    public const string Image = "python:3.12-slim-bookworm";

    public Task<ExecutionResult> ExecuteAsync(ExecutionRequest request, CancellationToken cancellationToken)
    {
        if (request.Language != CodeLanguage.Python)
        {
            return Task.FromResult(ExecutionResult.Rejected($"Nieobsługiwany język: {request.Language}."));
        }

        var containerName = $"codelab-{Guid.NewGuid():N}";

        return ProcessExecutor.RunAsync(
            "docker",
            [
                "run", "--rm", "-i",
                "--name", containerName,
                "--network", "none",
                "--memory", "128m",
                "--cpus", "0.5",
                "--pull", "never",
                Image,
                "python", "-I", "-",
            ],
            request.Code,
            cancellationToken,
            onCancelled: () => ProcessExecutor.RunAsync(
                "docker", ["kill", containerName], string.Empty, CancellationToken.None));
    }
}
