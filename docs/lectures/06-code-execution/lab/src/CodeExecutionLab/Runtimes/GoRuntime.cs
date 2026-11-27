namespace CodeExecutionLab.Runtimes;

/// <summary>
/// Runtime Go w zewnętrznym procesie: kod zapisywany jest do pliku tymczasowego
/// i uruchamiany przez <c>go run</c> (Go nie czyta programu ze stdin).
///
/// <para>Program musi być pakietem <c>main</c> z funkcją <c>main</c>. Wynik wypisujemy przez <c>fmt.Println</c>.</para>
///
/// <para>Izolacja: <c>go run</c> działa w procesie systemowym hosta — NIE jest izolacją bezpieczeństwa.
/// Ograniczamy jedynie środowisko: brak sieci (<c>GOPROXY=off</c>), brak pobierania toolchaina
/// (<c>GOTOOLCHAIN=local</c>) i brak cgo. Kod ma dostęp do systemu plików hosta.</para>
///
/// <para>Anulowanie: drzewo procesów (<c>go</c> i skompilowany program) jest zabijane przez <see cref="ProcessExecutor"/>.</para>
/// </summary>
public sealed class GoRuntime : ICodeRuntime
{
    private const string SourceFileName = "main.go";

    private static readonly IReadOnlyDictionary<string, string> GoEnvironment = new Dictionary<string, string>
    {
        ["GOPROXY"] = "off",
        ["GOTOOLCHAIN"] = "local",
        ["GOFLAGS"] = "-mod=readonly",
        ["CGO_ENABLED"] = "0",
        ["GOTELEMETRY"] = "off",
    };

    public async Task<ExecutionResult> ExecuteAsync(ExecutionRequest request, CancellationToken cancellationToken)
    {
        if (request.Language != CodeLanguage.Go)
        {
            return ExecutionResult.Rejected($"Nieobsługiwany język: {request.Language}.");
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return ExecutionResult.Cancelled();
        }

        // Każde wykonanie w osobnym katalogu — kompilacja nie widzi plików innych wykonań.
        var workDirectory = Directory.CreateTempSubdirectory("codeexec-go-");

        try
        {
            await File.WriteAllTextAsync(
                Path.Combine(workDirectory.FullName, SourceFileName),
                request.Code,
                cancellationToken);

            return await ProcessExecutor.RunAsync(
                "go",
                ["run", SourceFileName],
                string.Empty,
                cancellationToken,
                workingDirectory: workDirectory.FullName,
                extraEnvironment: GoEnvironment);
        }
        catch (OperationCanceledException)
        {
            return ExecutionResult.Cancelled();
        }
        finally
        {
            workDirectory.Delete(recursive: true);
        }
    }
}
