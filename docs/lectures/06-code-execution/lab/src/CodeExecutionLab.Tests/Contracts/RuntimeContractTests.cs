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
    private static readonly bool IsJs = Language == CodeLanguage.JavaScript;

    private static string Code(string python, string javascript) => IsJs ? javascript : python;

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
        var result = await RunAsync(RuntimeFactory.CreateBase(RuntimeName), Code("print('ok')", "console.log('ok')"));

        Assert.Equal(ExecutionStatus.Succeeded, result.Status);
        Assert.Equal(0, result.ExitCode);
    }

    [SkippableFact]
    public async Task ReadsResultFromStdout()
    {
        var result = await RunAsync(RuntimeFactory.CreateBase(RuntimeName), Code("print(6 * 7)", "console.log(6 * 7)"));

        Assert.Contains("42", result.Stdout);
    }

    [SkippableFact]
    public async Task ReportsSyntaxError()
    {
        var result = await RunAsync(RuntimeFactory.CreateBase(RuntimeName), Code("def (:", "function ("));

        Assert.Equal(ExecutionStatus.Failed, result.Status);
        Assert.False(string.IsNullOrWhiteSpace(result.Stderr));
    }

    [SkippableFact]
    public async Task ReportsRuntimeError()
    {
        var result = await RunAsync(RuntimeFactory.CreateBase(RuntimeName), Code("raise ValueError('x')", "throw new Error('x')"));

        Assert.Equal(ExecutionStatus.Failed, result.Status);
        Assert.False(string.IsNullOrWhiteSpace(result.Stderr));
    }

    [SkippableFact]
    public async Task EnforcesFiveSecondTimeout()
    {
        var runtime = new TimeoutRuntime(RuntimeFactory.CreateBase(RuntimeName));
        var result = await RunAsync(runtime, Code("import time\ntime.sleep(60)", "while (true) {}"), TimeSpan.FromSeconds(5));

        Assert.Equal(ExecutionStatus.TimedOut, result.Status);
    }

    [SkippableFact]
    public async Task StopsCleanlyAfterCancellation()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
        var result = await RunAsync(
            RuntimeFactory.CreateBase(RuntimeName),
            Code("while True:\n    pass", "while (true) {}"),
            TimeSpan.FromSeconds(30),
            cts.Token);

        Assert.Equal(ExecutionStatus.Cancelled, result.Status);
    }
}
