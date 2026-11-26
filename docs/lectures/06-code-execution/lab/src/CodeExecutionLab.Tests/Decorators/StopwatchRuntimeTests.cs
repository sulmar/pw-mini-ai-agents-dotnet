using CodeExecutionLab.Runtimes;
using CodeExecutionLab.Tests.Fakes;
using Xunit;

namespace CodeExecutionLab.Tests.Decorators;

public class StopwatchRuntimeTests
{
    private static readonly ExecutionRequest Request = new(CodeLanguage.Python, "print(1)", TimeSpan.FromSeconds(5));

    [Fact]
    public async Task FillsDurationFromInnerOperation()
    {
        var inner = new FakeRuntime(async (_, _) =>
        {
            await Task.Delay(120);
            return new ExecutionResult(ExecutionStatus.Succeeded, 0, "1", "");
        });
        var runtime = new StopwatchRuntime(inner);

        var result = await runtime.ExecuteAsync(Request, CancellationToken.None);

        Assert.True(result.DurationMs >= 100, $"DurationMs={result.DurationMs}");
        Assert.Equal("1", result.Stdout);
    }

    [Fact]
    public async Task PassesThroughStatusAndExitCode()
    {
        var expected = new ExecutionResult(ExecutionStatus.Failed, 1, "", "SyntaxError");
        var runtime = new StopwatchRuntime(FakeRuntime.Returning(expected));

        var result = await runtime.ExecuteAsync(Request, CancellationToken.None);

        Assert.Equal(ExecutionStatus.Failed, result.Status);
        Assert.Equal(1, result.ExitCode);
        Assert.Equal("SyntaxError", result.Stderr);
    }
}
