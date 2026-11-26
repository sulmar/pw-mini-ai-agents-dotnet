namespace CodeExecutionLab.Runtimes;

/// <summary>
/// Runtime JavaScript w procesie .NET (interpreter Jint).
/// </summary>
public sealed class JintRuntime : ICodeRuntime
{
    // Wskazówki:
    //  - Pakiet NuGet: dotnet add package Jint (src/CodeExecutionLab).
    //  - new Engine(options => options.TimeoutInterval(...) / options.CancellationToken(...)).
    //  - Wyjście console.log: podmień konsolę (engine.SetValue) i zbieraj do StringBuilder.
    //  - Jint działa w procesie: NIE jest izolacją bezpieczeństwa. Ogranicz dostęp do CLR
    //    (np. nie wystawiaj Environment, File, Process).

    public Task<ExecutionResult> ExecuteAsync(ExecutionRequest request, CancellationToken cancellationToken)
    {
        // TODO: zaimplementuj wykonanie kodu zgodnie z kontraktem ICodeRuntime:
        //  - odrzuć język inny niż JavaScript (ExecutionResult.Rejected),
        //  - uruchom kod poza procesem aplikacji (lub przerwij interpreter),
        //  - po anulowaniu zatrzymaj wykonanie i zwróć ExecutionStatus.Cancelled,
        //  - błędy kodu zwróć jako Failed z stderr (bez wyjątków),
        //  - nie przekazuj kluczy API do środowiska wykonania.
        throw new NotImplementedException("TODO: JintRuntime — implementacja ExecuteAsync.");
    }
}
