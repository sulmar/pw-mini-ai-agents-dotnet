namespace CodeExecutionLab.Configuration;

/// <summary>Konfiguracja z zmiennych środowiskowych. Sekrety i adresy nigdy nie trafiają do runtime'u.</summary>
public sealed record LabSettings(string OllamaUrl, string Model, string RuntimeName)
{
    public static LabSettings FromEnvironment() => new(
        OllamaUrl: Environment.GetEnvironmentVariable("OLLAMA_URL") ?? "http://localhost:11434",
        Model: Environment.GetEnvironmentVariable("OLLAMA_MODEL") ?? "qwen3:8b",
        RuntimeName: Environment.GetEnvironmentVariable("CODE_RUNTIME") ?? "lua");
}
