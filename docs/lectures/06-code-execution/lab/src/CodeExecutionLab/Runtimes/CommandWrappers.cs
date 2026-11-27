namespace CodeExecutionLab.Runtimes;

/// <summary>
/// Uruchomienie bezpośrednio na hoście. Jeśli komenda wymaga pliku źródłowego,
/// tworzy katalog tymczasowy, zapisuje kod i usuwa katalog po wykonaniu.
/// NIE jest izolacją bezpieczeństwa.
/// </summary>
public sealed class HostWrapper : ICommandWrapper
{
    public PreparedCommand Prepare(ProcessCommand command, string image)
    {
        if (command.SourceFileName is null)
        {
            return new PreparedCommand(
                command.FileName, command.Arguments, command.Stdin,
                command.Environment, WorkingDirectory: null, OnCancelled: null, Cleanup: () => { });
        }

        var workDirectory = Directory.CreateTempSubdirectory("codeexec-host-");
        File.WriteAllText(Path.Combine(workDirectory.FullName, command.SourceFileName), command.Stdin);

        return new PreparedCommand(
            command.FileName, command.Arguments, Input: string.Empty,
            command.Environment, WorkingDirectory: workDirectory.FullName,
            OnCancelled: null, Cleanup: () => workDirectory.Delete(recursive: true));
    }
}

/// <summary>
/// Uruchomienie w jednorazowym kontenerze Docker: bez sieci, z limitem pamięci i CPU,
/// z zapisywalnym <c>/w</c> (tmpfs), bez montowania katalogów hosta.
/// Przy anulowaniu kontener jest zabijany przez <c>docker kill</c>.
/// </summary>
public sealed class DockerWrapper : ICommandWrapper
{
    private const string WorkDirectory = "/w";

    public PreparedCommand Prepare(ProcessCommand command, string image)
    {
        var containerName = $"codelab-{Guid.NewGuid():N}";

        var arguments = new List<string>
        {
            "run", "--rm", "-i",
            "--name", containerName,
            "--network", "none",
            "--memory", command.MemoryLimit,
            "--cpus", "0.5",
            "--pull", "never",
            "--tmpfs", WorkDirectory,
            "-w", WorkDirectory,
        };

        foreach (var (name, value) in command.Environment)
        {
            arguments.Add("-e");
            arguments.Add($"{name}={value}");
        }

        arguments.Add(image);

        if (command.SourceFileName is null)
        {
            arguments.Add(command.FileName);
            arguments.AddRange(command.Arguments);
        }
        else
        {
            // Kod ze stdin zapisujemy do pliku, potem uruchamiamy program. Wszystkie
            // fragmenty komendy pochodzą z profilu języka (stałe), nie od użytkownika.
            var program = string.Join(' ', new[] { command.FileName }.Concat(command.Arguments));
            arguments.Add("sh");
            arguments.Add("-c");
            arguments.Add($"cat > {WorkDirectory}/{command.SourceFileName} && exec {program}");
        }

        return new PreparedCommand(
            "docker", arguments, command.Stdin,
            new Dictionary<string, string>(),
            WorkingDirectory: null,
            OnCancelled: () => ProcessExecutor.RunAsync(
                "docker", ["kill", containerName], string.Empty, CancellationToken.None),
            Cleanup: () => { });
    }
}
