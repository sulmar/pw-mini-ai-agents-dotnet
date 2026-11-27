namespace CodeExecutionLab.Runtimes;

/// <summary>
/// Opis języka: jaki program go uruchamia, jakie ma argumenty i zmienne środowiska oraz
/// jakiego obrazu kontenera używać (gdy wykonanie idzie przez Docker).
/// </summary>
public sealed record LanguageProfile(
    CodeLanguage Language,
    string Image,
    string FileName,
    IReadOnlyList<string> Arguments,
    string? SourceFileName = null,
    IReadOnlyDictionary<string, string>? Environment = null,
    string MemoryLimit = "128m")
{
    public ProcessCommand ToCommand(string code) => new(
        FileName,
        Arguments,
        code,
        SourceFileName,
        Environment ?? new Dictionary<string, string>(),
        MemoryLimit);
}

public static class Languages
{
    public static readonly LanguageProfile Python = new(
        CodeLanguage.Python,
        Image: "python:3.12-slim-bookworm",
        FileName: "python3",
        Arguments: ["-I", "-"]);

    public static readonly LanguageProfile JavaScript = new(
        CodeLanguage.JavaScript,
        Image: "node:22-slim",
        FileName: "node",
        Arguments: []);

    public static readonly LanguageProfile Go = new(
        CodeLanguage.Go,
        Image: "golang:1.27",
        FileName: "go",
        Arguments: ["run", "main.go"],
        SourceFileName: "main.go",
        // Kompilator Go nie mieści się w 128 MB (w kontenerze kończy się sygnałem "killed").
        MemoryLimit: "512m",
        Environment: new Dictionary<string, string>
        {
            ["GOPROXY"] = "off",
            ["GOTOOLCHAIN"] = "local",
            ["GOFLAGS"] = "-mod=readonly",
            ["CGO_ENABLED"] = "0",
            ["GOTELEMETRY"] = "off",
        });
}
