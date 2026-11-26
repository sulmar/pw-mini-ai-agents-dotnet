namespace CodeExecutionLab.Runtimes;

/// <summary>
/// Wspólny kontrakt wykonywania kodu. Wszystkie runtime'y (Python, Jint, Node, Docker)
/// implementują ten interfejs. Agent nigdy nie wywołuje ich bezpośrednio — zawsze przez
/// <c>ExecuteCodeTool → StopwatchRuntime → TimeoutRuntime → ICodeRuntime</c>.
///
/// <para>ODPOWIEDZIALNOŚĆ RUNTIME'U (ważne — dekoratory tego nie wymuszą):</para>
/// <list type="bullet">
///   <item>Kod uruchamiany jest poza procesem aplikacji (proces systemowy / kontener),
///         a Jint — jako interpreter w procesie — musi sam przerwać pętlę po anulowaniu.</item>
///   <item>Po anulowaniu <paramref name="cancellationToken"/> runtime MUSI zatrzymać wykonanie
///         (zabić proces, przerwać interpreter) i zwrócić <see cref="ExecutionStatus.Cancelled"/>.
///         Samo anulowanie <c>Task</c> NIE kończy procesu systemowego.</item>
///   <item>Błędy kodu użytkownika (składnia, wyjątek w trakcie działania) to NIE wyjątki —
///         zwracamy <see cref="ExecutionStatus.Failed"/> z treścią z stderr.</item>
///   <item>Runtime nie może otrzymać kluczy API ani innych sekretów z konfiguracji aplikacji.</item>
///   <item>Runtime nie obsługuje języków, których nie deklaruje — zwraca <see cref="ExecutionStatus.Rejected"/>.</item>
/// </list>
/// </summary>
public interface ICodeRuntime
{
    Task<ExecutionResult> ExecuteAsync(ExecutionRequest request, CancellationToken cancellationToken);
}
