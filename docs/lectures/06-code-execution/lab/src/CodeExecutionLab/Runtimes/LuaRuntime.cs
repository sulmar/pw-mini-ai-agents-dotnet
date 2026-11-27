using System.Text;
using Lua;
using Lua.IO;
using Lua.Platforms;
using Lua.Standard;

namespace CodeExecutionLab.Runtimes;

/// <summary>
/// Runtime Lua 5.2 oparty o LuaCSharp.
///
/// <para>Sandbox:</para>
/// <list type="bullet">
///   <item>Otwierane są tylko biblioteki base, string, math, table, coroutine i bitwise.</item>
///   <item>Nie otwieramy <c>io</c>, <c>os</c>, <c>package</c>/<c>require</c> ani <c>debug</c>.</item>
///   <item>Z biblioteki base usuwamy <c>dofile</c> i <c>loadfile</c>.</item>
///   <item>Platforma (system plików, zmienne środowiskowe, <c>os.exit</c>) jest zaślepiona —
///         nawet gdyby ktoś dodał bibliotekę, nie dostanie dostępu do hosta.</item>
///   <item>Standardowe wyjście (<c>print</c>) trafia do bufora w pamięci.</item>
/// </list>
///
/// <para>Anulowanie: LuaCSharp nie przerywa pętli sam z siebie, więc hook instrukcji
/// (<c>SetHook</c>, licznik instrukcji) sprawdza <see cref="CancellationToken"/> co
/// <see cref="HookInstructionInterval"/> instrukcji i rzuca <see cref="OperationCanceledException"/>.</para>
/// </summary>
public sealed class LuaRuntime : ICodeRuntime
{
    private const int HookInstructionInterval = 1_000;

    public Task<ExecutionResult> ExecuteAsync(ExecutionRequest request, CancellationToken cancellationToken)
    {
        if (request.Language != CodeLanguage.Lua)
        {
            return Task.FromResult(
                ExecutionResult.Rejected(
                    $"Nieobsługiwany język: {request.Language}."));
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult(ExecutionResult.Cancelled());
        }

        return Task.Run(
            () => ExecuteCoreAsync(request, cancellationToken),
            CancellationToken.None);
    }

    private static async Task<ExecutionResult> ExecuteCoreAsync(
        ExecutionRequest request,
        CancellationToken cancellationToken)
    {
        var stdout = new MemoryStream();
        var stderr = new MemoryStream();

        var platform = new LuaPlatform(
            new DeniedFileSystem(),
            new DeniedOsEnvironment(),
            new CapturedStandardIO(stdout, stderr),
            TimeProvider.System);

        var state = LuaState.Create(platform);

        try
        {
            OpenSandboxedLibraries(state);
            InstallCancellationHook(state, cancellationToken);

            await state.DoStringAsync(request.Code, "execution", cancellationToken);

            await platform.StandardIO.Output.FlushAsync(CancellationToken.None);

            return new ExecutionResult(
                Status: ExecutionStatus.Succeeded,
                ExitCode: 0,
                Stdout: Decode(stdout),
                Stderr: Decode(stderr));
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return ExecutionResult.Cancelled();
        }
        catch (LuaParseException exception)
        {
            return Failed(stdout, stderr, exception.Message);
        }
        catch (LuaCompileException exception)
        {
            return Failed(stdout, stderr, exception.Message);
        }
        catch (LuaRuntimeException exception)
        {
            return Failed(stdout, stderr, exception.Message);
        }
    }

    private static void OpenSandboxedLibraries(LuaState state)
    {
        // Celowo NIE wywołujemy OpenStandardLibraries(): zawiera io, os, package i debug.
        state.OpenBasicLibrary();
        state.OpenStringLibrary();
        state.OpenMathLibrary();
        state.OpenTableLibrary();
        state.OpenCoroutineLibrary();
        state.OpenBitwiseLibrary();

        // Biblioteka base dokłada funkcje czytające pliki — usuwamy je z globalnego środowiska.
        state.Environment["dofile"] = LuaValue.Nil;
        state.Environment["loadfile"] = LuaValue.Nil;
    }

    private static void InstallCancellationHook(LuaState state, CancellationToken cancellationToken)
    {
        var hook = new LuaFunction("cancellation_check", (context, _) =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return new ValueTask<int>(context.Return());
        });

        // Pusta maska + licznik: zdarzenie co N instrukcji Lua.
        state.SetHook(hook, string.Empty, HookInstructionInterval);
    }

    private static ExecutionResult Failed(MemoryStream stdout, MemoryStream stderr, string message)
    {
        var errorText = Decode(stderr);
        return new ExecutionResult(
            Status: ExecutionStatus.Failed,
            ExitCode: 1,
            Stdout: Decode(stdout),
            Stderr: string.IsNullOrEmpty(errorText)
                ? message
                : string.Join(Environment.NewLine, errorText, message).Trim());
    }

    private static string Decode(MemoryStream stream) =>
        Encoding.UTF8.GetString(stream.ToArray());

    /// <summary>Wyjście standardowe i błędów kierowane do buforów w pamięci; wejście puste.</summary>
    private sealed class CapturedStandardIO(MemoryStream stdout, MemoryStream stderr) : ILuaStandardIO
    {
        private readonly ILuaStream _input = ILuaStream.CreateFromStream(new MemoryStream(), LuaFileOpenMode.Read);
        private readonly ILuaStream _output = ILuaStream.CreateFromStream(stdout, LuaFileOpenMode.Write);
        private readonly ILuaStream _error = ILuaStream.CreateFromStream(stderr, LuaFileOpenMode.Write);

        public ILuaStream Input => _input;
        public ILuaStream Output => _output;
        public ILuaStream Error => _error;
    }

    /// <summary>Brak dostępu do systemu plików hosta.</summary>
    private sealed class DeniedFileSystem : ILuaFileSystem
    {
        public string DirectorySeparator => "/";

        public bool IsReadable(string path) => false;

        public string GetTempFileName() => throw new NotSupportedException("Dostęp do plików jest wyłączony.");

        public ValueTask<ILuaStream> Open(string path, LuaFileOpenMode mode, CancellationToken cancellationToken) =>
            ValueTask.FromException<ILuaStream>(new NotSupportedException("Dostęp do plików jest wyłączony."));

        public ValueTask Rename(string oldPath, string newPath, CancellationToken cancellationToken) =>
            ValueTask.FromException(new NotSupportedException("Dostęp do plików jest wyłączony."));

        public ValueTask Remove(string path, CancellationToken cancellationToken) =>
            ValueTask.FromException(new NotSupportedException("Dostęp do plików jest wyłączony."));

        public ValueTask<ILuaStream> OpenTempFileStream(CancellationToken cancellationToken) =>
            ValueTask.FromException<ILuaStream>(new NotSupportedException("Dostęp do plików jest wyłączony."));
    }

    /// <summary>Brak dostępu do środowiska procesu (zmienne środowiskowe, os.exit).</summary>
    private sealed class DeniedOsEnvironment : ILuaOsEnvironment
    {
        public string GetEnvironmentVariable(string name) => string.Empty;

        public double GetTotalProcessorTime() => 0;

        public ValueTask Exit(int exitCode, CancellationToken cancellationToken) =>
            ValueTask.FromException(new NotSupportedException("os.exit jest wyłączone."));
    }
}
