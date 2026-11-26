namespace CodeExecutionLab.Runtimes;

/// <summary>
/// Dekorator wymuszający limit czasu z <see cref="ExecutionRequest.Timeout"/>.
///
/// <para>Zachowanie:</para>
/// <list type="bullet">
///   <item>Limit czasu i zewnętrzne anulowanie są łączone w jeden <c>CancellationToken</c>
///         przekazywany do runtime'u.</item>
///   <item>Po przekroczeniu limitu zwracamy jednoznaczne <see cref="ExecutionStatus.TimedOut"/>.</item>
///   <item>Po anulowaniu czekamy krótko (<c>grace</c>) na zakończenie runtime'u. Jeśli runtime
///         nie zareagował, i tak zwracamy wynik — ale proces/interpreter może nadal działać.
///         Zatrzymanie wykonania jest odpowiedzialnością runtime'u (patrz <see cref="ICodeRuntime"/>).</item>
/// </list>
/// </summary>
public sealed class TimeoutRuntime(ICodeRuntime inner, TimeSpan? grace = null) : ICodeRuntime
{
    private readonly TimeSpan _grace = grace ?? TimeSpan.FromSeconds(2);

    public async Task<ExecutionResult> ExecuteAsync(ExecutionRequest request, CancellationToken cancellationToken)
    {
        using var timeoutCts = new CancellationTokenSource(request.Timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        // Sygnał „zatrzymaj” niezależny od tego, czy runtime reaguje na token.
        var stopSignal = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var registration = linkedCts.Token.Register(() => stopSignal.TrySetResult());

        var work = inner.ExecuteAsync(request, linkedCts.Token);
        var finished = await Task.WhenAny(work, stopSignal.Task);

        if (finished == work)
        {
            return await work;
        }

        // Limit czasu albo anulowanie z zewnątrz. Dajemy runtime'owi chwilę na sprzątanie.
        var cleanup = await Task.WhenAny(work, Task.Delay(_grace));
        if (cleanup != work)
        {
            // Runtime nie zakończył się w czasie grace — to naruszenie kontraktu ICodeRuntime.
            // Nie czekamy w nieskończoność; obserwujemy wyjątek, żeby nie zgłosić go jako nieobsłużony.
            _ = work.ContinueWith(t => _ = t.Exception, TaskContinuationOptions.OnlyOnFaulted);
        }

        return timeoutCts.IsCancellationRequested
            ? ExecutionResult.TimedOut(request.Timeout)
            : ExecutionResult.Cancelled();
    }
}
