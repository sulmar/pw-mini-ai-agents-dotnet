using CodeExecutionLab.Runtimes;
using Xunit;

namespace CodeExecutionLab.Tests.Contracts;

/// <summary>
/// Testy kontraktowe dla runtime'u wybranego przez CODE_RUNTIME (domyślnie: python).
/// Dopóki runtime nie ma implementacji (NotImplementedException), testy są POMIJANE, nie błędne.
/// Uruchomienie: dotnet test --filter Contracts (z katalogu src).
/// </summary>
public class RuntimeContractTests
{
    private static readonly string RuntimeName = Environment.GetEnvironmentVariable("CODE_RUNTIME") ?? "python";
    private static readonly CodeLanguage Language = RuntimeFactory.LanguageFor(RuntimeName);

    // Moduł WAT wypisujący "42\n" przez WASI fd_write (iovec pod adresem 8, ptr=0, len=3).
    private const string WasmPrints42 = """
        (module
          (import "wasi_snapshot_preview1" "fd_write" (func $fd_write (param i32 i32 i32 i32) (result i32)))
          (memory (export "memory") 1)
          (data (i32.const 0) "42\n")
          (data (i32.const 8) "\00\00\00\00\03\00\00\00")
          (func (export "_start")
            (drop (call $fd_write (i32.const 1) (i32.const 8) (i32.const 1) (i32.const 16)))))
        """;

    private static string Code(string python, string javascript, string lua, string wasm) => Language switch
    {
        CodeLanguage.Lua => lua,
        CodeLanguage.JavaScript => javascript,
        CodeLanguage.WebAssembly => wasm,
        _ => python,
    };

    private static async Task<ExecutionResult> RunAsync(ICodeRuntime runtime, string code, TimeSpan? timeout = null, CancellationToken ct = default)
    {
        var request = new ExecutionRequest(Language, code, timeout ?? TimeSpan.FromSeconds(5));
        try
        {
            return await runtime.ExecuteAsync(request, ct);
        }
        catch (NotImplementedException)
        {
            throw new SkipException($"Runtime '{RuntimeName}' nie jest jeszcze zaimplementowany (TODO).");
        }
    }

    [SkippableFact]
    public async Task ExecutesValidProgram()
    {
        var result = await RunAsync(
            RuntimeFactory.CreateBase(RuntimeName),
            Code("print('ok')", "console.log('ok')", "print('ok')", WasmPrints42));

        Assert.Equal(ExecutionStatus.Succeeded, result.Status);
        Assert.Equal(0, result.ExitCode);
    }

    [SkippableFact]
    public async Task ReadsResultFromStdout()
    {
        var result = await RunAsync(
            RuntimeFactory.CreateBase(RuntimeName),
            Code("print(6 * 7)", "console.log(6 * 7)", "print(6 * 7)", WasmPrints42));

        Assert.Contains("42", result.Stdout);
    }

    [SkippableFact]
    public async Task ReportsSyntaxError()
    {
        var result = await RunAsync(
            RuntimeFactory.CreateBase(RuntimeName),
            Code("def (:", "function (", "print(", "(module (func"));

        Assert.Equal(ExecutionStatus.Failed, result.Status);
        Assert.False(string.IsNullOrWhiteSpace(result.Stderr));
    }

    [SkippableFact]
    public async Task ReportsRuntimeError()
    {
        var result = await RunAsync(
            RuntimeFactory.CreateBase(RuntimeName),
            Code("raise ValueError('x')", "throw new Error('x')", "error('x')",
                "(module (func (export \"_start\") unreachable))"));

        Assert.Equal(ExecutionStatus.Failed, result.Status);
        Assert.False(string.IsNullOrWhiteSpace(result.Stderr));
    }

    [SkippableFact]
    public async Task EnforcesFiveSecondTimeout()
    {
        var runtime = new TimeoutRuntime(RuntimeFactory.CreateBase(RuntimeName));
        var result = await RunAsync(
            runtime,
            Code("import time\ntime.sleep(60)", "while (true) {}", "while true do end",
                "(module (func (export \"_start\") (loop $l (br $l))))"),
            TimeSpan.FromSeconds(5));

        Assert.Equal(ExecutionStatus.TimedOut, result.Status);
    }

    [SkippableFact]
    public async Task StopsCleanlyAfterCancellation()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
        var result = await RunAsync(
            RuntimeFactory.CreateBase(RuntimeName),
            Code("while True:\n    pass", "while (true) {}", "while true do end",
                "(module (func (export \"_start\") (loop $l (br $l))))"),
            TimeSpan.FromSeconds(30),
            cts.Token);

        Assert.Equal(ExecutionStatus.Cancelled, result.Status);
    }
}
