namespace CodeExecutionLab.Runtimes;

/// <summary>
/// Runtime uruchamiający Python w zewnętrznym procesie (python3).
/// </summary>
public sealed class PythonRuntime : ICodeRuntime
{
    // Wskazówki:
    //  - System.Diagnostics.Process + ProcessStartInfo (RedirectStandardOutput/Error = true).
    //  - Kod przekaż przez stdin (python3 -) albo plik tymczasowy w osobnym katalogu.
    //  - Czyść ProcessStartInfo.Environment (nie przekazuj zmiennych ANTHROPIC_*).
    //  - Po anulowaniu: process.Kill(entireProcessTree: true) i WaitForExitAsync.
    //  - Wynik: ExitCode, ReadToEndAsync dla stdout/stderr (z limitem rozmiaru).

    public Task<ExecutionResult> ExecuteAsync(ExecutionRequest request, CancellationToken cancellationToken)
    {
        // TODO: zaimplementuj wykonanie kodu zgodnie z kontraktem ICodeRuntime:
        //  - odrzuć język inny niż Python (ExecutionResult.Rejected),
        //  - uruchom kod poza procesem aplikacji (lub przerwij interpreter),
        //  - po anulowaniu zatrzymaj wykonanie i zwróć ExecutionStatus.Cancelled,
        //  - błędy kodu zwróć jako Failed z stderr (bez wyjątków),
        //  - nie przekazuj kluczy API do środowiska wykonania.
        throw new NotImplementedException("TODO: PythonRuntime — implementacja ExecuteAsync.");
    }
}
