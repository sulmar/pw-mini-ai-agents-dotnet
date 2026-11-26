namespace CodeExecutionLab.Runtimes;

/// <summary>
/// Runtime JavaScript w zewnętrznym procesie Node.js.
/// </summary>
public sealed class NodeRuntime : ICodeRuntime
{
    // Wskazówki:
    //  - Proces: node (przekaż kod przez stdin lub plik tymczasowy).
    //  - Process.Start + RedirectStandardOutput/Error; czyść Environment.
    //  - Po anulowaniu: process.Kill(entireProcessTree: true).
    //  - Node w procesie nadal ma dostęp do systemu plików i sieci hosta — to nie jest izolacja.

    public Task<ExecutionResult> ExecuteAsync(ExecutionRequest request, CancellationToken cancellationToken)
    {
        // TODO: zaimplementuj wykonanie kodu zgodnie z kontraktem ICodeRuntime:
        //  - odrzuć język inny niż JavaScript (ExecutionResult.Rejected),
        //  - uruchom kod poza procesem aplikacji (lub przerwij interpreter),
        //  - po anulowaniu zatrzymaj wykonanie i zwróć ExecutionStatus.Cancelled,
        //  - błędy kodu zwróć jako Failed z stderr (bez wyjątków),
        //  - nie przekazuj kluczy API do środowiska wykonania.
        throw new NotImplementedException("TODO: NodeRuntime — implementacja ExecuteAsync.");
    }
}
