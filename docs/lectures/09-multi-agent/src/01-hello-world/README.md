# MAF Hello World: Autor i Recenzent

Minimalna aplikacja konsolowa .NET 10 z Microsoft Agent Framework i Claude (Anthropic API).

## Gdzie jest projekt

Projekt znajduje się w repozytorium w katalogu:

```text
docs/lectures/09-multi-agent/src/01-hello-world/
├── MultiAgentHelloWorld.csproj   # plik projektu (pakiety: Microsoft.Agents.AI.Workflows, Anthropic)
├── Program.cs                    # cały przykład: dwaj agenci i workflow sekwencyjny
└── README.md                     # ten plik
```

Wszystkie polecenia poniżej uruchamiaj z katalogu głównego repozytorium
(`pw-mini-ai-agents-dotnet`). Jeśli jesteś już w katalogu projektu, pomiń
opcję `--project` i wpisz po prostu `dotnet run`.

```text
Polecenie użytkownika -> Autor -> Recenzent -> gotowe powitanie
```

Autor pisze dwuzdaniowe powitanie, a Recenzent redaguje je do jednego zdania.
Obaj używają tego samego modelu, lecz mają osobne nazwy i instrukcje.
Kolejność jest ustalona w kodzie przez `AgentWorkflowBuilder.BuildSequential`.
MAF przekazuje następnemu agentowi dotychczasową rozmowę, w tym odpowiedź Autora.
Konsola pokazuje odpowiedzi obu agentów strumieniowo.

## Uruchomienie

Wymagania: SDK .NET 10, dostęp do NuGet oraz klucz API Anthropic (Claude) z dostępem do wybranego modelu.
Wywołania modelu są płatne zgodnie z kontem dostawcy. Klucza nie zapisujemy w repozytorium.

Z katalogu głównego repozytorium (zsh/bash):

```bash
# Wczytaj klucz bez pokazywania go w terminalu ani historii poleceń.
read -s 'ANTHROPIC_API_KEY?Klucz API: '
export ANTHROPIC_API_KEY
echo
export ANTHROPIC_MODEL='claude-opus-5-5'

dotnet run --project docs/lectures/09-multi-agent/src/01-hello-world
```

Polecenie `read` powyżej jest przeznaczone dla zsh. W bash użyj `read -s -p 'Klucz API: ' ANTHROPIC_API_KEY`.

Możesz przekazać własne zadanie:

```bash
dotnet run --project docs/lectures/09-multi-agent/src/01-hello-world -- \
  "Przywitaj studentów na prezentacji Shelly Lab."
```

Model można zmienić zmienną `ANTHROPIC_MODEL` (np. `claude-sonnet-5-5` lub `claude-haiku-5-5`). W przypadku braku klucza program
kończy działanie z komunikatem i kodem 1. Treść odpowiedzi jest niedeterministyczna.

## Co pokazać studentom

1. Dwie instrukcje w `Program.cs`: podział odpowiedzialności.
2. `BuildSequential([author, reviewer])`: jawna kolejność pracy.
3. Strumień zdarzeń: widać wynik pierwszego agenta przed odpowiedzią drugiego.
4. Zmień instrukcję Recenzenta, aby zadawał jedno pytanie do publiczności.
5. Zapytaj: czy tutaj jeden agent wykonałby zadanie równie dobrze?

To dydaktyczny pipeline dwóch agentów. Recenzent jest redaktorem tekstu, nie
niezależnym testem poprawności. Nie ma tu autonomicznego delegowania, pętli
poprawek ani oceny jakości wyniku.

## Most do Shelly Lab

W następnym przykładzie Autor może zostać Programistą, a Recenzent Testerem.
Wtedy dodajemy narzędzia MCP, kryteria akceptacji, rzeczywiste uruchomienie
testów w symulatorze i warunkowy powrót do Programisty. Wdrożenie na urządzenie
to osobny etap z potwierdzeniem egzekwowanym przez aplikację.

## Dokumentacja

- [Claude API: C# SDK](https://platform.claude.com/docs/en/get-started)
- [Microsoft Learn: Sequential orchestration](https://learn.microsoft.com/en-us/agent-framework/workflows/orchestrations/sequential?pivots=programming-language-csharp)

Wersje pakietów są zapisane w pliku projektu.
