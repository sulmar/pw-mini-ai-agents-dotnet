using Wasmtime;

namespace CodeExecutionLab.Runtimes;

public sealed class WebAssemblyRuntime : ICodeRuntime
{
    // dotnet add package Wasmtime
    private const long MaxMemoryBytes = 64 * 1024 * 1024;

    public Task<ExecutionResult> ExecuteAsync(
        ExecutionRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Language != CodeLanguage.WebAssembly)
        {
            return Task.FromResult(
                ExecutionResult.Rejected(
                    $"Nieobsługiwany język: {request.Language}."));
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult(
                ExecutionResult.Cancelled());
        }

        return Task.Run(
            () => Execute(request, cancellationToken),
            CancellationToken.None);
    }

    private static ExecutionResult Execute(
        ExecutionRequest request,
        CancellationToken cancellationToken)
    {
        var directory = Path.Combine(
            Path.GetTempPath(),
            $"wasm-{Guid.NewGuid():N}");

        var stdoutPath = Path.Combine(directory, "stdout");
        var stderrPath = Path.Combine(directory, "stderr");

        try
        {
            Directory.CreateDirectory(directory);

            // Czas wykonania ogranicza przerwanie epoką (timeout / anulowanie), nie budżet paliwa:
            // budżet paliwa zużywał się w kilkadziesiąt ms i przerywał pętle przed limitem czasu.
            using var config = new Config()
                .WithEpochInterruption(true);

            using var engine = new Engine(config);
            using var store = new Store(engine);

            store.SetLimits(memorySize: MaxMemoryBytes);
            store.SetEpochDeadline(1);

            using var registration = cancellationToken.Register(
                () => engine.IncrementEpoch());

            cancellationToken.ThrowIfCancellationRequested();

            using var module = Module.FromText(
                engine,
                "execution",
                request.Code);

            cancellationToken.ThrowIfCancellationRequested();

            using var linker = new Linker(engine);

            linker.DefineWasi();

            var wasi = new WasiConfiguration()
                .WithStandardOutput(stdoutPath)
                .WithStandardError(stderrPath);

            store.SetWasiConfiguration(wasi);

            var instance = linker.Instantiate(store, module);

            var start = instance.GetAction("_start");

            if (start is null)
            {
                return ExecutionResult.Rejected(
                    "Moduł musi eksportować funkcję _start " +
                    "o sygnaturze () -> ().");
            }

            start();

            cancellationToken.ThrowIfCancellationRequested();

            return new ExecutionResult(
                Status: ExecutionStatus.Succeeded,
                ExitCode: 0,
                Stdout: ReadOutput(stdoutPath),
                Stderr: ReadOutput(stderrPath));
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            return ExecutionResult.Cancelled();
        }
        catch (WasmtimeException)
            when (cancellationToken.IsCancellationRequested)
        {
            return ExecutionResult.Cancelled();
        }
        catch (WasmtimeException exception)
        {
            return new ExecutionResult(
                Status: ExecutionStatus.Failed,
                ExitCode: 1,
                Stdout: ReadOutput(stdoutPath),
                Stderr: string.Join(
                    Environment.NewLine,
                    ReadOutput(stderrPath),
                    exception.Message).Trim());
        }
        finally
        {
            DeleteDirectory(directory);
        }
    }

    private static string ReadOutput(string path)
    {
        return File.Exists(path)
            ? File.ReadAllText(path)
            : string.Empty;
    }

    private static void DeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }
        catch (IOException)
        {
            // Sprzątanie nie powinno przesłaniać wyniku wykonania.
        }
        catch (UnauthorizedAccessException)
        {
            // Analogicznie w przypadku braku uprawnień.
        }
    }
}
