using CodeExecutionLab.Runtimes;
using CodeExecutionLab.Tests.Fakes;
using CodeExecutionLab.Tools;
using Xunit;

namespace CodeExecutionLab.Tests.Decorators;

public class ExecuteCodeToolTests
{
    private static ExecutionResult Ok(string stdout = "42") =>
        new(ExecutionStatus.Succeeded, 0, stdout, "");

    [Fact]
    public async Task RejectsUnknownLanguageWithoutCallingRuntime()
    {
        var fake = FakeRuntime.Returning(Ok());
        var tool = new ExecuteCodeTool(fake);

        var output = await tool.ExecuteCodeAsync("cobol", "print 1");

        Assert.Contains("Rejected", output);
        Assert.Equal(0, fake.Calls);
    }

    [Fact]
    public async Task RejectsTooLongCode()
    {
        var fake = FakeRuntime.Returning(Ok());
        var tool = new ExecuteCodeTool(fake, new ExecutionLimits(MaxCodeChars: 10));

        var output = await tool.ExecuteCodeAsync("python", new string('a', 11));

        Assert.Contains("za długi", output);
        Assert.Equal(0, fake.Calls);
    }

    [Fact]
    public async Task TruncatesLongOutput()
    {
        var tool = new ExecuteCodeTool(FakeRuntime.Returning(Ok(new string('x', 500))), new ExecutionLimits(MaxOutputChars: 100));

        var output = await tool.ExecuteCodeAsync("python", "print('x')");

        Assert.Contains("obcięto", output);
        Assert.True(output.Length < 500);
    }

    [Fact]
    public async Task LimitsAttemptsPerTaskAndResetsOnNewTask()
    {
        var fake = FakeRuntime.Returning(new(ExecutionStatus.Failed, 1, "", "err"));
        var tool = new ExecuteCodeTool(fake, new ExecutionLimits(MaxAttemptsPerTask: 4));

        for (var i = 0; i < 4; i++)
        {
            await tool.ExecuteCodeAsync("python", "raise x");
        }

        var blocked = await tool.ExecuteCodeAsync("python", "raise x");
        Assert.Contains("LIMIT_PROB", blocked);
        Assert.Equal(4, fake.Calls);

        tool.StartNewTask();
        await tool.ExecuteCodeAsync("python", "print(1)");
        Assert.Equal(5, fake.Calls);
    }

    [Fact]
    public async Task ReportsNotImplementedRuntimeWithoutRetryHint()
    {
        var tool = new ExecuteCodeTool(new FakeRuntime((_, _) => throw new NotImplementedException()));

        var output = await tool.ExecuteCodeAsync("python", "print(1)");

        Assert.Contains("RUNTIME_NIEZAIMPLEMENTOWANY", output);
    }
}
