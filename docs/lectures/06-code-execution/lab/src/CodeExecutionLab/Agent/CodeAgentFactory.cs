using CodeExecutionLab.Tools;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OllamaSharp;

namespace CodeExecutionLab.Agent;

public static class CodeAgentFactory
{
    public const string Instructions = """
        Jesteś asystentem, który rozwiązuje zadania obliczeniowe, pisząc i wykonując kod.
        Zasady:
        1. Napisz program w Pythonie (language = 'python') lub JavaScript (language = 'javascript').
        2. Program musi wypisać wynik na standardowe wyjście.
        3. Wykonaj kod narzędziem execute_code i użyj wyniku w odpowiedzi.
        4. Jeśli wynik ma Status Failed, Rejected lub TimedOut, przeanalizuj stderr,
           popraw program i wywołaj narzędzie ponownie. Masz maksymalnie trzy ponowienia.
        5. Jeśli odpowiedź zaczyna się od LIMIT_PROB lub RUNTIME_NIEZAIMPLEMENTOWANY, nie ponawiaj;
           napisz użytkownikowi, co poszło nie tak.
        6. Nie używaj sieci, nie czytaj plików ani zmiennych środowiskowych.
        Odpowiadaj po polsku.
        """;

    public static AIAgent Create(string ollamaUrl, string model, ExecuteCodeTool codeTool)
    {
        // OllamaSharp implementuje IChatClient z Microsoft.Extensions.AI,
        // więc agent MAF nie zależy od dostawcy modelu.
        IChatClient chatClient = new OllamaApiClient(new Uri(ollamaUrl), model);

        return new ChatClientAgent(
            chatClient,
            instructions: Instructions,
            name: "CodeAgent",
            tools: [AIFunctionFactory.Create(codeTool.ExecuteCodeAsync, name: "execute_code")]);
    }
}
