using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using OpenAI;

// Jeden model, dwie role. Klucz ustawiamy w środowisku, nie w kodzie.
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.Error.WriteLine("Ustaw zmienną OPENAI_API_KEY przed uruchomieniem przykładu.");
    return 1;
}

var model = Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-4.1-mini";
using IChatClient client = new OpenAIClient(apiKey).GetChatClient(model).AsIChatClient();

// 1. Każdy agent ma własną nazwę i instrukcję.
AIAgent author = new ChatClientAgent(client,
    name: "Autor",
    instructions: """
        Jesteś autorem powitania na wykładzie. Napisz po polsku krótkie powitanie
        na zadany temat: najwyżej dwa zdania. Nie dodawaj komentarzy ani nagłówków.
        """);

AIAgent reviewer = new ChatClientAgent(client,
    name: "Recenzent",
    instructions: """
        Jesteś redaktorem. Popraw powitanie napisane przez Autora.
        Zachowaj temat, użyj prostego języka i skróć tekst do jednego zdania.
        Zwróć tylko ostateczne powitanie po polsku.
        """);

// 2. MAF przekazuje rozmowę i odpowiedź Autora do Recenzenta.
var workflow = AgentWorkflowBuilder.BuildSequential([author, reviewer]);
var request = args.Length > 0
    ? string.Join(" ", args)
    : "Przywitaj studentów na wykładzie o współpracy agentów AI.";

Console.WriteLine($"Zadanie: {request}");
Console.WriteLine("Przebieg: Autor -> Recenzent");

// 3. Pokazujemy odpowiedzi obu agentów na żywo.
List<ChatMessage> messages = [new(ChatRole.User, request)];
await using StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, messages);
await run.TrySendMessageAsync(new TurnToken(emitEvents: true));

string? lastAgent = null;
await foreach (WorkflowEvent evt in run.WatchStreamAsync())
{
    if (evt is AgentResponseUpdateEvent update)
    {
        if (lastAgent != update.ExecutorId)
        {
            lastAgent = update.ExecutorId;
            Console.WriteLine($"\n\n[{lastAgent}]");
        }

        Console.Write(update.Update.Text);
    }
}

Console.WriteLine();
return 0;
