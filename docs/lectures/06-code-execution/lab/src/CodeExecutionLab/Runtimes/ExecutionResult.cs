namespace CodeExecutionLab.Runtimes;

public enum ExecutionStatus
{
    /// <summary>Kod zakończył się kodem 0.</summary>
    Succeeded,
    /// <summary>Błąd składni lub błąd w trakcie wykonania (niezerowy kod wyjścia).</summary>
    Failed,
    /// <summary>Przekroczono limit czasu (ustawia <c>TimeoutRuntime</c>).</summary>
    TimedOut,
    /// <summary>Wykonanie anulowane przez wywołującego.</summary>
    Cancelled,
    /// <summary>Żądanie odrzucone przed wykonaniem (np. nieobsługiwany język, za długi kod).</summary>
    Rejected
}

/// <summary>Wynik wykonania kodu.</summary>
/// <param name="DurationMs">Uzupełniany przez <c>StopwatchRuntime</c>; runtime zwraca 0.</param>
public sealed record ExecutionResult(
    ExecutionStatus Status,
    int? ExitCode,
    string Stdout,
    string Stderr,
    long DurationMs = 0)
{
    public bool Success => Status == ExecutionStatus.Succeeded;

    public static ExecutionResult Rejected(string reason) =>
        new(ExecutionStatus.Rejected, null, string.Empty, reason);

    public static ExecutionResult TimedOut(TimeSpan limit) =>
        new(ExecutionStatus.TimedOut, null, string.Empty, $"Przekroczono limit czasu ({limit.TotalSeconds:0.#} s).");

    public static ExecutionResult Cancelled() =>
        new(ExecutionStatus.Cancelled, null, string.Empty, "Wykonanie anulowane.");
}
