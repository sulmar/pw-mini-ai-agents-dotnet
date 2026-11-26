using System.Diagnostics;
using CodeExecutionLab.Runtimes;
using CodeExecutionLab.Tests.Fakes;
using Xunit;

namespace CodeExecutionLab.Tests.Decorators;

public class TimeoutRuntimeTests
{
    [Fact]
    public async Task ReturnsInnerResultWhenFinishedInTime()
    {
        var expected = new ExecutionResult(ExecutionStatus.Succeeded, 0, "ok", "");
        var runtime = new TimeoutRuntime(FakeRuntime.Returning(expected));

        var result = await runtime.ExecuteAsync(new(CodeLanguage.Python, "x", TimeSpan.FromSeconds(5)), CancellationToken.None);

        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task ReturnsTimedOutWhenRuntimeRespectsCancellation()
    {
        var runtime = new TimeoutRuntime(FakeRuntime.RespectingCancellation());
        var sw = Stopwatch.StartNew();

        var result = await runtime.ExecuteAsync(new(CodeLanguage.Python, "x", TimeSpan.FromMilliseconds(300)), CancellationToken.None);

        Assert.Equal(ExecutionStatus.TimedOut, result.Status);
        Assert.True(sw.Elapsed < TimeSpan.FromSeconds(3));
    }

    [Fact]
    public async Task ReturnsTimedOutEvenWhenRuntimeIgnoresCancellation()
    {
        var runtime = new TimeoutRuntime(FakeRuntime.IgnoringCancellation(), grace: TimeSpan.FromMilliseconds(200));
        var sw = Stopwatch.StartNew();

        var result = await runtime.ExecuteAsync(new(CodeLanguage.Python, "x", TimeSpan.FromMilliseconds(200)), CancellationToken.None);

        Assert.Equal(ExecutionStatus.TimedOut, result.Status);
        Assert.True(sw.Elapsed < TimeSpan.FromSeconds(5), "Dekorator nie może czekać bez końca na runtime.");
    }

    [Fact]
    public async Task ExternalCancellationReturnsCancelled()
    {
        var runtime = new TimeoutRuntime(FakeRuntime.RespectingCancellation());
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(200));

        var result = await runtime.ExecuteAsync(new(CodeLanguage.Python, "x", TimeSpan.FromSeconds(30)), cts.Token);

        Assert.Equal(ExecutionStatus.Cancelled, result.Status);
    }
}
