using System.Diagnostics;
using System.Text;

namespace CodeExecutionLab.Runtimes;

/// <summary>
/// Wspólne uruchamianie zewnętrznego procesu dla runtime'ów Python, Node i Docker.
///
/// <para>Zasady:</para>
/// <list type="bullet">
///   <item>Kod przekazywany jest przez stdin — bez plików tymczasowych.</item>
///   <item>Środowisko jest czyszczone; przekazujemy tylko PATH i HOME (Docker potrzebuje HOME do konfiguracji).</item>
///   <item>Po anulowaniu drzewo procesów jest zabijane, a wywołujący może wykonać dodatkowe sprzątanie (np. <c>docker kill</c>).</item>
///   <item>Wyjście jest ograniczane do <see cref="MaxCapturedChars"/> znaków na strumień.</item>
///   <item>Kod wyjścia 0 → Succeeded, inny → Failed (bez wyjątków).</item>
/// </list>
/// </summary>
internal static class ProcessExecutor
{
    public const int MaxCapturedChars = 64 * 1024;

    private static readonly string[] PassedEnvironmentVariables = ["PATH", "HOME"];

    public static async Task<ExecutionResult> RunAsync(
        string fileName,
        IReadOnlyList<string> arguments,
        string input,
        CancellationToken cancellationToken,
        Func<Task>? onCancelled = null,
        string? workingDirectory = null,
        IReadOnlyDictionary<string, string>? extraEnvironment = null)
    {
        var startInfo = new ProcessStartInfo(fileName)
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardInputEncoding = Encoding.UTF8,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8,
            WorkingDirectory = workingDirectory ?? string.Empty,
        };

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        startInfo.Environment.Clear();
        foreach (var name in PassedEnvironmentVariables)
        {
            var value = Environment.GetEnvironmentVariable(name);
            if (value is not null)
            {
                startInfo.Environment[name] = value;
            }
        }

        if (extraEnvironment is not null)
        {
            foreach (var (name, value) in extraEnvironment)
            {
                startInfo.Environment[name] = value;
            }
        }

        using var process = new Process { StartInfo = startInfo };

        try
        {
            process.Start();
        }
        catch (System.ComponentModel.Win32Exception exception)
        {
            return new ExecutionResult(ExecutionStatus.Failed, 1, string.Empty,
                $"Nie udało się uruchomić '{fileName}': {exception.Message}");
        }

        var stdoutTask = ReadLimitedAsync(process.StandardOutput);
        var stderrTask = ReadLimitedAsync(process.StandardError);

        try
        {
            await process.StandardInput.WriteAsync(input.AsMemory(), cancellationToken);
            await process.StandardInput.FlushAsync(cancellationToken);
            process.StandardInput.Close();

            await process.WaitForExitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            KillTree(process);
            await process.WaitForExitAsync(CancellationToken.None);

            if (onCancelled is not null)
            {
                await onCancelled();
            }

            return ExecutionResult.Cancelled();
        }
        catch (IOException)
        {
            // Proces zakończył się, zanim zdążyliśmy wpisać stdin (np. błąd uruchomienia).
            await process.WaitForExitAsync(CancellationToken.None);
        }

        var stdout = await stdoutTask;
        var stderr = await stderrTask;

        return new ExecutionResult(
            Status: process.ExitCode == 0 ? ExecutionStatus.Succeeded : ExecutionStatus.Failed,
            ExitCode: process.ExitCode,
            Stdout: stdout,
            Stderr: stderr);
    }

    private static void KillTree(Process process)
    {
        try
        {
            process.Kill(entireProcessTree: true);
        }
        catch (InvalidOperationException)
        {
            // Proces już się zakończył.
        }
    }

    private static async Task<string> ReadLimitedAsync(StreamReader reader)
    {
        var builder = new StringBuilder();
        var buffer = new char[4096];
        int read;

        while ((read = await reader.ReadAsync(buffer)) > 0)
        {
            var remaining = MaxCapturedChars - builder.Length;
            if (remaining > 0)
            {
                builder.Append(buffer, 0, Math.Min(read, remaining));
            }
        }

        return builder.ToString();
    }
}
