namespace CodeExecutionLab.Runtimes;

/// <summary>
/// Buduje łańcuch dekoratorów: StopwatchRuntime → ConsoleLogRuntime → TimeoutRuntime → wybrany runtime.
/// Wybór runtime'u następuje wyłącznie przez konfigurację (CODE_RUNTIME), bez zmian w agencie.
///
/// <para>Nazwy procesowe: <c>python</c>, <c>node</c>, <c>go</c> (host) oraz <c>docker-python</c>,
/// <c>docker-node</c>, <c>docker-go</c> (kontener). <c>docker</c> to alias dla <c>docker-python</c>.</para>
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
        "python" => new ProcessRuntime(Languages.Python, new HostWrapper()),
        "node" => new ProcessRuntime(Languages.JavaScript, new HostWrapper()),
        "go" => new ProcessRuntime(Languages.Go, new HostWrapper()),
        "docker" or "docker-python" => new ProcessRuntime(Languages.Python, new DockerWrapper()),
        "docker-node" => new ProcessRuntime(Languages.JavaScript, new DockerWrapper()),
        "docker-go" => new ProcessRuntime(Languages.Go, new DockerWrapper()),
        "pythonnet" => new PythonNetRuntime(),
        "jint" => new JintRuntime(),
        "lua" => new LuaRuntime(),
        "webassembly" => new WebAssemblyRuntime(),
        _ => throw new ArgumentException(
            $"Nieznany runtime: '{name}'. Dostępne: python, node, go, docker-python, docker-node, docker-go, " +
            "pythonnet, jint, lua, webassembly.",
            nameof(name))
    };

    public static CodeLanguage LanguageFor(string name) => name.ToLowerInvariant() switch
    {
        "jint" or "node" or "docker-node" => CodeLanguage.JavaScript,
        "webassembly" => CodeLanguage.WebAssembly,
        "lua" => CodeLanguage.Lua,
        "go" or "docker-go" => CodeLanguage.Go,
        _ => CodeLanguage.Python
    };
}
