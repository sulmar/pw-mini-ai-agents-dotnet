using System.Text;
using CodeExecutionLab.Agent;
using CodeExecutionLab.Configuration;
using CodeExecutionLab.Runtimes;
using CodeExecutionLab.Tools;
using Microsoft.Extensions.AI;

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

    // Wyświetlenie końcowej odpowiedzi
    // var response = await agent.RunAsync(input, session);
    // Console.WriteLine(response.Text);

    // Wyświetlanie na bieżąco komunikatów
    var response = new StringBuilder();

    await foreach (var update in agent.RunStreamingAsync(input, session))
    {
        Console.ForegroundColor =  ConsoleColor.Gray;
        Console.Write(update.Text);
        response.Append(update.Text);
    }

    Console.ResetColor();
    Console.WriteLine();

    // Wyświetlenie końcowej odpowiedzi
    string finalResponse = response.ToString();
    Console.WriteLine(finalResponse);

}
