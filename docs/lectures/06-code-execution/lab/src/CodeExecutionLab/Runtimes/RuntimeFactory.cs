namespace CodeExecutionLab.Runtimes;

/// <summary>
/// Buduje łańcuch dekoratorów: StopwatchRuntime → TimeoutRuntime → wybrany runtime.
/// Wybór runtime'u następuje wyłącznie przez konfigurację (CODE_RUNTIME), bez zmian w agencie.
/// </summary>
public static class RuntimeFactory
{
    public static ICodeRuntime Create(string name) =>
        new StopwatchRuntime(
            new ConsoleLogRuntime(
                new TimeoutRuntime(CreateBase(name))));

    /// <summary>Surowy runtime bez dekoratorów — używany przez testy kontraktowe.</summary>
    public static ICodeRuntime CreateBase(string name) => name.ToLowerInvariant() switch
    {
        "python" => new PythonRuntime(),
        "jint" => new JintRuntime(),
        "node" => new NodeRuntime(),
        "docker" => new DockerRuntime(),
        "lua" => new LuaRuntime(),
        "webassembly" => new WebAssemblyRuntime(),
        _ => throw new ArgumentException($"Nieznany runtime: '{name}'. Dostępne: python, jint, node, webassembly, docker.", nameof(name))
    };

    public static CodeLanguage LanguageFor(string name) => name.ToLowerInvariant() switch
    {
        "jint" or "node" => CodeLanguage.JavaScript,
        "webassembly" => CodeLanguage.WebAssembly,
        "lua" => CodeLanguage.Lua,
        _ => CodeLanguage.Python
    };
}
