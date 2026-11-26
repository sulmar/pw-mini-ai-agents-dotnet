namespace CodeExecutionLab.Runtimes;

/// <summary>
/// Ograniczenia bezpieczeństwa i kosztów. Egzekwowane w <c>ExecuteCodeTool</c>,
/// niezależnie od tego, który runtime został wybrany.
/// </summary>
/// <param name="MaxCodeChars">Maksymalna długość kodu wejściowego.</param>
/// <param name="MaxOutputChars">Maksymalna długość stdout/stderr zwracanego do modelu.</param>
/// <param name="MaxAttemptsPerTask">1 próba + maksymalnie 3 ponowienia po błędzie.</param>
/// <param name="TimeoutSeconds">Limit czasu jednego wykonania.</param>
public sealed record ExecutionLimits(
    int MaxCodeChars = 4_000,
    int MaxOutputChars = 4_000,
    int MaxAttemptsPerTask = 4,
    int TimeoutSeconds = 5)
{
    public TimeSpan Timeout => TimeSpan.FromSeconds(TimeoutSeconds);
}
