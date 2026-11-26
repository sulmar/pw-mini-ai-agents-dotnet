using CodeExecutionLab.Agent;
using CodeExecutionLab.Configuration;
using CodeExecutionLab.Runtimes;
using CodeExecutionLab.Tools;

var settings = LabSettings.FromEnvironment();
var runtime = RuntimeFactory.Create(settings.RuntimeName);
var codeTool = new ExecuteCodeTool(runtime);
var agent = CodeAgentFactory.Create(settings.OllamaUrl, settings.Model, codeTool);

Console.WriteLine($"Runtime: {settings.RuntimeName} | Model: {settings.Model}");
Console.WriteLine("Wpisz polecenie (pusty wiersz lub 'exit' kończy).");

var session = await agent.CreateSessionAsync();

while (true)
{
    Console.Write("\n> ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    codeTool.StartNewTask();
    var response = await agent.RunAsync(input, session);
    Console.WriteLine(response.Text);
}
