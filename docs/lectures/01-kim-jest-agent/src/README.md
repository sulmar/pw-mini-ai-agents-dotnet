# Przykłady — agent szukający kawy

Oba projekty wymagają SDK .NET 10. Polecenia uruchamiaj z katalogu wykładu (`docs/lectures/01-kim-jest-agent`).

## 01 — agent na regułach

Student chce kupić kawę. Automat jest zepsuty, kawiarnia jest otwarta. Decyzję podejmuje program: obserwuje środowisko i wybiera działanie według warunków w kodzie.

```bash
dotnet run --project 01-agent-szukajacy-kawy
```

Oczekiwany wynik:

```
Sprawdzam automat...
Automat nie działa. Idę do kawiarni.
```

## 02 — agent z LLM (Ollama)

Ta sama sytuacja, uzupełniona o czas do zajęć. Zamiast reguł decyzję (`AUTOMAT`, `KAWIARNIA` albo `REZYGNUJ`) podejmuje lokalny model.

Najpierw pobierz model i uruchom Ollamę:

```bash
ollama pull qwen2.5:3b
ollama serve
```

`ollama serve` zostaw w osobnym terminalu. Potem uruchom agenta:

```bash
dotnet run --project 02-agent-z-llm
```

Program wypisuje opis środowiska, pyta model pod `http://localhost:11434` i wykonuje rozpoznane działanie. Odpowiedź modelu nie jest deterministyczna. Jeśli nie da się jej zmapować na jedno z trzech działań, program wypisze: `Nie rozpoznano decyzji modelu.`
