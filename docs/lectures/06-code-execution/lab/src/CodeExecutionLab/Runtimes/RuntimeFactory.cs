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
        "pythonnet" => new PythonNetRuntime(),
        "jint" => new JintRuntime(),
        "node" => new NodeRuntime(),
        "docker" => new DockerRuntime(),
        "lua" => new LuaRuntime(),
        "webassembly" => new WebAssemblyRuntime(),
        "go" => new GoRuntime(),
        _ => throw new ArgumentException($"Nieznany runtime: '{name}'. Dostępne: python, pythonnet, jint, node, lua, webassembly, go, docker.", nameof(name))
    };

    public static CodeLanguage LanguageFor(string name) => name.ToLowerInvariant() switch
    {
        "jint" or "node" => CodeLanguage.JavaScript,
        "webassembly" => CodeLanguage.WebAssembly,
        "lua" => CodeLanguage.Lua,
        "go" => CodeLanguage.Go,
        _ => CodeLanguage.Python
    };
}
