using System.Text;
using Jint;
using Jint.Runtime;

namespace CodeExecutionLab.Runtimes;

/// <summary>
/// Runtime JavaScript w procesie .NET (interpreter Jint).
///
/// <para>Jint działa w procesie aplikacji — NIE jest izolacją bezpieczeństwa.
/// Do silnika nie wystawiamy żadnych obiektów CLR poza funkcją wyjścia (<c>__emit</c>);
/// Environment, File ani Process są niedostępne.</para>
///
/// <para>Anulowanie: token przekazany do <see cref="Engine"/> przerywa pętle i rzuca
/// <see cref="OperationCanceledException"/>.</para>
/// </summary>
public sealed class JintRuntime : ICodeRuntime
{
    private const string ConsolePrelude =
        "var console = { log: function () { __emit(Array.prototype.map.call(arguments, String).join(' ')); } };";

    public Task<ExecutionResult> ExecuteAsync(ExecutionRequest request, CancellationToken cancellationToken)
    {
        if (request.Language != CodeLanguage.JavaScript)
        {
            return Task.FromResult(ExecutionResult.Rejected($"Nieobsługiwany język: {request.Language}."));
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromResult(ExecutionResult.Cancelled());
        }

        return Task.Run(() => ExecuteCore(request.Code, cancellationToken), CancellationToken.None);
    }

    private static ExecutionResult ExecuteCore(string code, CancellationToken cancellationToken)
    {
        var stdout = new StringBuilder();

        try
        {
            var engine = new Engine(options => options.CancellationToken(cancellationToken));
            engine.SetValue("__emit", new Action<string>(line => stdout.Append(line).Append('\n')));
            engine.Execute(ConsolePrelude);
            engine.Execute(code);

            return new ExecutionResult(ExecutionStatus.Succeeded, 0, stdout.ToString(), string.Empty);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return ExecutionResult.Cancelled();
        }
        catch (JintException exception)
        {
            // Jint może opakować anulowanie we własny wyjątek — rozpoznajemy je po tokenie.
            if (cancellationToken.IsCancellationRequested)
            {
                return ExecutionResult.Cancelled();
            }

            return new ExecutionResult(ExecutionStatus.Failed, 1, stdout.ToString(), exception.Message);
        }
    }
}
