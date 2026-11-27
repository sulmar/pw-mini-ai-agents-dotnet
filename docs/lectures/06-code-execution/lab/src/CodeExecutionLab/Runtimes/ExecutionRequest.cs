namespace CodeExecutionLab.Runtimes;

public enum CodeLanguage
{
    Python,
    JavaScript,
    WebAssembly,
    Lua
}

/// <summary>Żądanie wykonania kodu.</summary>
/// <param name="Language">Język, w którym napisany jest kod.</param>
/// <param name="Code">Kod źródłowy (limit długości egzekwowany przez <c>ExecuteCodeTool</c>).</param>
/// <param name="Timeout">Maksymalny czas wykonania, egzekwowany przez <c>TimeoutRuntime</c>.</param>
public sealed record ExecutionRequest(CodeLanguage Language, string Code, TimeSpan Timeout);
