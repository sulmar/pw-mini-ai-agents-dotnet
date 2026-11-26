namespace CodeExecutionLab.Runtimes;

/// <summary>
/// Runtime uruchamiający kod w izolowanym kontenerze (wariant zaawansowany, opcjonalny).
/// </summary>
public sealed class DockerRuntime : ICodeRuntime
{
    // Wskazówki:
    //  - Proces: docker run --rm -i --network none --memory 128m --cpus 0.5 <obraz> python -
    //  - Kontener kończysz przy anulowaniu: docker kill <nazwa> (ustaw --name).
    //  - Nie montuj katalogów hosta; użyj obrazu bez kluczy i sekretów.
    //  - Obsługuj brak działającego Dockera jako ExecutionResult.Failed, nie wyjątek.

    public Task<ExecutionResult> ExecuteAsync(ExecutionRequest request, CancellationToken cancellationToken)
    {
        // TODO: zaimplementuj wykonanie kodu zgodnie z kontraktem ICodeRuntime:
        //  - odrzuć język inny niż Python (ExecutionResult.Rejected),
        //  - uruchom kod poza procesem aplikacji (lub przerwij interpreter),
        //  - po anulowaniu zatrzymaj wykonanie i zwróć ExecutionStatus.Cancelled,
        //  - błędy kodu zwróć jako Failed z stderr (bez wyjątków),
        //  - nie przekazuj kluczy API do środowiska wykonania.
        throw new NotImplementedException("TODO: DockerRuntime — implementacja ExecuteAsync.");
    }
}
