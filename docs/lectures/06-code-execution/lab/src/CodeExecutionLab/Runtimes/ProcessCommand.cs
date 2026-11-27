namespace CodeExecutionLab.Runtimes;

/// <summary>
/// Komenda do uruchomienia, niezależna od miejsca wykonania (host albo kontener).
/// Kod użytkownika zawsze trafia do <see cref="Stdin"/>. Jeśli <see cref="SourceFileName"/> jest ustawione,
/// owijacz zapisze kod do pliku o tej nazwie przed uruchomieniem (np. Go nie czyta ze stdin).
/// </summary>
public sealed record ProcessCommand(
    string FileName,
    IReadOnlyList<string> Arguments,
    string Stdin,
    string? SourceFileName,
    IReadOnlyDictionary<string, string> Environment,
    string MemoryLimit);

/// <summary>
/// Komenda gotowa do uruchomienia przez <see cref="ProcessExecutor"/>. Po wykonaniu wywołaj <see cref="Cleanup"/>.
/// </summary>
public sealed record PreparedCommand(
    string FileName,
    IReadOnlyList<string> Arguments,
    string Input,
    IReadOnlyDictionary<string, string> Environment,
    string? WorkingDirectory,
    Func<Task>? OnCancelled,
    Action Cleanup);

/// <summary>
/// Owija komendę w sposób izolujący (lub nie). Decyduje o miejscu wykonania, nie o języku.
/// </summary>
public interface ICommandWrapper
{
    PreparedCommand Prepare(ProcessCommand command, string image);
}
