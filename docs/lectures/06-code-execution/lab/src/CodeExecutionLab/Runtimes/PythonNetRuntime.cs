using Python.Runtime;

namespace CodeExecutionLab.Runtimes;

/// <summary>
/// Runtime Pythona w tym samym procesie .NET przez Python.NET (pakiet <c>pythonnet</c>).
///
/// <para>Wymagania: zainstalowany Python 3 z biblioteką współdzieloną (libpython). Ścieżkę do niej
/// podajemy przez zmienną środowiskową <c>PYTHONNET_PYDLL</c>, np. na macOS
/// <c>/opt/homebrew/opt/python@3.12/Frameworks/Python.framework/Versions/3.12/Python</c>.</para>
///
/// <para>Izolacja: Python działa W PROCESIE aplikacji — NIE jest izolacją bezpieczeństwa.
/// Kod ma pełny dostęp do modułów Pythona i (przez nie) do systemu plików i sieci hosta.
/// Używaj tego runtime'u wyłącznie do celów dydaktycznych lub gdy kod jest zaufany.</para>
///
/// <para>Mechanika:</para>
/// <list type="bullet">
///   <item>Interpreter inicjalizowany jest raz na proces; GIL wątku głównego jest zwalniany
///         (<c>BeginAllowThreads</c>), a każde wykonanie zdobywa GIL przez <c>Py.GIL()</c>.</item>
///   <item>Każdy program działa we własnym scope; <c>sys.stdout</c>/<c>sys.stderr</c> są przekierowane
///         do buforów w pamięci.</item>
///   <item>Anulowanie: wykonanie chodzi pod <c>sys.settrace</c>, a funkcja śledząca przy każdej linii
///         kodu użytkownika sprawdza flagę <c>_stop</c>. Flagę ustawia wywołanie z .NET pod GIL.</item>
/// </list>
/// </summary>
public sealed class PythonNetRuntime : ICodeRuntime
{
    // Kod harnessu wykonywany w każdym scope. Funkcja _run przechwytuje wyjście, ustawia śledzenie
    // anulowania i zwraca status: 0 = sukces, 1 = błąd, 2 = zatrzymane przez anulowanie.
    private const string Harness = """
        import io as _io
        import sys as _sys

        _stop = [False]
        _result = ("", "")

        class _Stop(BaseException):
            pass

        def _tracer(frame, event, arg):
            if frame.f_code.co_filename != "execution":
                return None
            if _stop[0]:
                raise _Stop()
            return _tracer

        def _run(src):
            global _result
            out = _io.StringIO()
            err = _io.StringIO()
            saved = _sys.stdout, _sys.stderr
            _sys.stdout, _sys.stderr = out, err
            status = 0
            _sys.settrace(_tracer)
            try:
                exec(compile(src, "execution", "exec"), {"__name__": "__main__"})
            except _Stop:
                status = 2
            except BaseException as e:
                status = 1
                err.write(f"{type(e).__name__}: {e}\n")
            finally:
                _sys.settrace(None)
                _sys.stdout, _sys.stderr = saved
                _result = (out.getvalue(), err.getvalue())
            return status
        """;

    private static readonly SemaphoreSlim Gate = new(1, 1);

    private static readonly Lazy<bool> EngineReady = new(() =>
    {
        if (!PythonEngine.IsInitialized)
        {
            PythonEngine.Initialize();
        }

        // Wątek główny nie może trzymać GIL, inaczej wykonania w innych wątkach czekałyby w nieskończoność.
        PythonEngine.BeginAllowThreads();
        return true;
    });

    public async Task<ExecutionResult> ExecuteAsync(ExecutionRequest request, CancellationToken cancellationToken)
    {
        if (request.Language != CodeLanguage.Python)
        {
            return ExecutionResult.Rejected($"Nieobsługiwany język: {request.Language}.");
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return ExecutionResult.Cancelled();
        }

        // Wszystkie wykonania współdzielą jeden interpreter i sys.stdout — uruchamiamy je pojedynczo.
        try
        {
            await Gate.WaitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return ExecutionResult.Cancelled();
        }

        try
        {
            return await Task.Run(() => ExecuteCore(request.Code, cancellationToken), CancellationToken.None);
        }
        finally
        {
            Gate.Release();
        }
    }

    private static ExecutionResult ExecuteCore(string code, CancellationToken cancellationToken)
    {
        _ = EngineReady.Value;

        var state = new RunState();

        try
        {
            using (Py.GIL())
            {
                using var scope = Py.CreateScope();
                scope.Exec(Harness);

                state.Flag = scope.Get("_stop");
                using var registration = cancellationToken.Register(() => Task.Run(() => RequestStop(state)));

                scope.Set("_user_code", code.ToPython());
                var status = scope.Eval("_run(_user_code)").As<int>();

                var stdout = scope.Eval("_result[0]").As<string>();
                var stderr = scope.Eval("_result[1]").As<string>();

                if (status == 2 || cancellationToken.IsCancellationRequested)
                {
                    return ExecutionResult.Cancelled();
                }

                return new ExecutionResult(
                    Status: status == 0 ? ExecutionStatus.Succeeded : ExecutionStatus.Failed,
                    ExitCode: status,
                    Stdout: stdout,
                    Stderr: stderr);
            }
        }
        catch (PythonException exception)
        {
            return cancellationToken.IsCancellationRequested
                ? ExecutionResult.Cancelled()
                : new ExecutionResult(ExecutionStatus.Failed, 1, string.Empty, exception.Message);
        }
        finally
        {
            lock (state.Sync)
            {
                state.Finished = true;
            }
        }
    }

    /// <summary>Ustawia flagę zatrzymania, jeśli wykonanie jeszcze trwa.</summary>
    private static void RequestStop(RunState state)
    {
        lock (state.Sync)
        {
            if (state.Finished || state.Flag is null)
            {
                return;
            }

            using (Py.GIL())
            {
                state.Flag.SetItem(0.ToPython(), true.ToPython());
            }
        }
    }

    /// <summary>
    /// Stan jednego wykonania. Pola są ustawiane/czytane pod <see cref="Sync"/>, a blokada nigdy
    /// nie jest trzymana, gdy wątek trzyma GIL, a druga strona już na nią czeka (brak zakleszczenia).
    /// </summary>
    private sealed class RunState
    {
        public readonly object Sync = new();
        public PyObject? Flag;
        public bool Finished;
    }
}
