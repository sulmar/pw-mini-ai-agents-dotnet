# Laboratorium: Code Execution w agentach AI

**Czas:** 90 minut
**Technologie:** .NET 10, Microsoft Agent Framework, wybrany runtime (Python, Jint, Node.js lub Docker)

## Cel

Rozszerzenie agenta AI o możliwość generowania i wykonywania kodu w Pythonie, JavaScript lub innym języku.

Priorytetem jest **integracja Code Execution z agentem AI**, a nie stopień skomplikowania wygenerowanego programu.

## Materiały startowe

Szkielet aplikacji w katalogu `src/` zawiera:

- Działającego agenta Microsoft Agent Framework (konsola, rozmowa interaktywna).
- Interfejs `ICodeRuntime` — wspólny kontrakt wykonywania kodu.
- Modele `ExecutionRequest`, `ExecutionResult` oraz `ExecutionLimits` (limity długości kodu, wyniku, prób i czasu).
- Dekoratory `StopwatchRuntime` (mierzy czas) i `TimeoutRuntime` (wymusza limit czasu) — gotowe.
- Narzędzie agenta `ExecuteCodeTool` — gotowe, z ograniczeniem liczby prób.
- Szkielety runtime'ów `PythonRuntime`, `JintRuntime`, `NodeRuntime`, `DockerRuntime` z miejscami `TODO`.
- Testy jednostkowe dekoratorów oraz testy kontraktowe runtime'u.

Architektura wywołania:

```
Agent → ExecuteCodeTool → StopwatchRuntime → TimeoutRuntime → ICodeRuntime
```

Wszystkie runtime'y implementują ten sam interfejs `ICodeRuntime`. Agent nie wie, który runtime jest używany.

## Struktura

```
src/
├── CodeExecutionLab.slnx
├── CodeExecutionLab/              # aplikacja konsolowa z agentem
│   ├── Runtimes/                  # kontrakty, dekoratory i szkielety runtime'ów
│   │   ├── ICodeRuntime.cs
│   │   ├── ExecutionRequest.cs
│   │   ├── ExecutionResult.cs
│   │   ├── ExecutionLimits.cs
│   │   ├── StopwatchRuntime.cs
│   │   ├── TimeoutRuntime.cs
│   │   ├── PythonRuntime.cs       # TODO
│   │   ├── JintRuntime.cs         # TODO
│   │   ├── NodeRuntime.cs         # TODO
│   │   └── DockerRuntime.cs       # TODO
│   └── ...
└── CodeExecutionLab.Tests/        # testy dekoratorów i testy kontraktowe
```

## Uruchomienie

### 1. Wymagania

- .NET SDK 10 (`dotnet --version`).
- Lokalny serwer Ollama (`ollama serve`) z pobranym modelem, np. `ollama pull llama3.1`.
- Dla wybranego runtime'u: Python 3 (`PythonRuntime`), Node.js (`NodeRuntime`) albo Docker (`DockerRuntime`). `JintRuntime` nie wymaga zewnętrznych programów.

### 2. Zmienne środowiskowe

| Zmienna | Wymagana | Domyślnie | Opis |
|---|---|---|---|
| `OLLAMA_URL` | nie | `http://localhost:11434` | Adres serwera Ollama. |
| `OLLAMA_MODEL` | nie | `llama3.1` | Nazwa modelu zainstalowanego w Ollama (musi obsługiwać tool calling). |
| `CODE_RUNTIME` | nie | `python` | Wybór runtime'u: `python`, `jint`, `node`, `docker`. |

Przykład (macOS/Linux):

```bash
export OLLAMA_MODEL=llama3.1
export CODE_RUNTIME=python
```

Przykład (Windows PowerShell):

```powershell
$env:OLLAMA_MODEL = "llama3.1"
$env:CODE_RUNTIME = "jint"
```

### 3. Uruchomienie agenta

Z katalogu `src/`:

```bash
dotnet build
dotnet run --project CodeExecutionLab
```

Po uruchomieniu wpisuj polecenia w konsoli. Wpisz `exit`, aby zakończyć.

Dopóki wybrany runtime nie jest zaimplementowany, narzędzie zwróci komunikat o braku implementacji (`NotImplementedException`), a agent poinformuje o tym użytkownika. Aplikacja kompiluje się i uruchamia się bez względu na stan implementacji.

### 4. Testy

Z katalogu `src/`:

```bash
dotnet test
```

- Testy dekoratorów i narzędzia używają fikcyjnego runtime'u i działają od razu.
- Testy kontraktowe (`Contracts/`) dotyczą runtime'u wskazanego przez `CODE_RUNTIME`. Dopóki runtime nie jest zaimplementowany, są **pomijane** (skipped), a nie zgłaszane jako błąd.

## Wybór runtime'u

Wybierz **jeden** runtime. Nie musisz implementować wszystkich.

| Runtime | Integracja | Uwagi |
|---|---|---|
| **Python** | zewnętrzny proces `python3` | Najprostszy do zrozumienia. Przekaż kod przez stdin lub plik tymczasowy, przechwyć stdout/stderr, ustaw limit czasu i zabij proces po anulowaniu. |
| **Node.js** | zewnętrzny proces `node` | Podobny do Pythona. Język JavaScript jest zgodny z wariantem Jint. |
| **Jint** | JavaScript w procesie .NET (pakiet NuGet `Jint`) | Bez zewnętrznych procesów. Wykonanie jest w procesie aplikacji, więc przerwanie pętli musisz obsłużyć sam (np. `TimeoutInterval` lub token anulowania w opcjach silnika). Przechwytywanie `console.log` wymaga własnego obiektu. |
| **Docker** | izolowany kontener (`docker run --rm`) | Najlepsza izolacja spośród czterech, ale wymaga działającego demona Dockera i obrazu. Wymuś brak sieci (`--network none`), limit pamięci i limit CPU. |

Kryteria wyboru: dostępność narzędzi na Twoim komputerze, czas, który chcesz poświęcić na integrację, oraz poziom izolacji, jakiego oczekujesz.

## Miejsca TODO

Zaimplementuj metodę `ExecuteAsync` w **jednej** z klas:

| Plik | Klasa | Metoda |
|---|---|---|
| `CodeExecutionLab/Runtimes/PythonRuntime.cs` | `PythonRuntime` | `ExecuteAsync` |
| `CodeExecutionLab/Runtimes/JintRuntime.cs` | `JintRuntime` | `ExecuteAsync` |
| `CodeExecutionLab/Runtimes/NodeRuntime.cs` | `NodeRuntime` | `ExecuteAsync` |
| `CodeExecutionLab/Runtimes/DockerRuntime.cs` | `DockerRuntime` | `ExecuteAsync` |

Wszystkie oznaczone miejsca zawierają komentarz `// TODO` i wskazówki dotyczące bibliotek. Nie zmieniaj sygnatury metody ani nie modyfikuj `ICodeRuntime`, dekoratorów ani agenta.

### Odpowiedzialność runtime'u (ważne)

Kontrakt jest opisany w `ICodeRuntime`. Runtime musi:

1. **Zatrzymać wykonanie po anulowaniu.** Po sygnale `CancellationToken` zabij proces (lub przerwij interpreter) i zwróć `ExecutionStatus.Cancelled`. Samo anulowanie `Task` nie kończy procesu systemowego.
2. **Nie rzucać wyjątków dla błędów kodu.** Składnia i błędy wykonania to `ExecutionStatus.Failed` z treścią w `Stderr`.
3. **Zwrócić wynik na stdout.** `Stdout` powinien zawierać to, co program wypisał.
4. **Nie przekazywać sekretów.** Nie przekazuj adresu i konfiguracji serwera modelu do procesu wykonującego kod.
5. **Obsłużyć nieobsługiwany język.** Zwróć `ExecutionStatus.Rejected`.

`TimeoutRuntime` zwraca `TimedOut` po przekroczeniu limitu i czeka krótko na reakcję runtime'u, ale nie zakłada, że proces został zatrzymany. Zatrzymanie jest Twoją odpowiedzialnością.

## Testy kontraktowe

Po implementacji runtime'u uruchom `dotnet test`. Testy kontraktowe sprawdzają:

1. Wykonanie poprawnego programu (`ExecutionStatus.Succeeded`, kod wyjścia 0).
2. Odczyt wyniku ze standardowego wyjścia.
3. Obsługę błędu składniowego (`Failed`, niepusty `Stderr`).
4. Obsługę błędu wykonania (`Failed`).
5. Przekroczenie limitu 5 sekund (`TimedOut`) dla nieskończonego programu.
6. Poprawne zakończenie wykonywania po anulowaniu (`Cancelled`, bez wiszącego procesu).

## Trzy przykładowe zadania dla agenta

Uruchom agenta i wpisz poniższe polecenia. Sprawdź, czy agent wygenerował kod, wykonał go i wykorzystał wynik.

1. **Koszt energii:** „Oblicz koszt energii dla urządzenia o mocy 750 W, pracującego 8 godzin, przy cenie 1,20 zł za kWh.”
   - Oczekiwany wynik: 7,20 zł.
2. **Liczby pierwsze:** „Znajdź wszystkie liczby pierwsze mniejsze od 1000 i podaj ich liczbę.”
   - Oczekiwany wynik: 168.
3. **Statystyka:** „Oblicz średnią i medianę dla zbioru liczb: 4, 8, 15, 16, 23, 42.”
   - Oczekiwany wynik: średnia 18,0; mediana 15,5.

Dodatkowo sprawdź, czy agent poprawia błędny program (np. poproś o kod z celowym błędem składni) i czy zachowuje się rozsądnie przy programie, który się nie kończy (np. `while True: pass`). Limit wykonania to 5 sekund.

## Bezpieczeństwo

**Timeout nie zastępuje izolacji.** Limit czasu ogranicza tylko czas działania, nie uprawnienia programu.

Opisz w swoim uzasadnieniu, jakie uprawnienia ma uruchamiany kod w wybranym runtime'ie:

- **Dostęp do systemu plików** — czy program może czytać lub zapisywać pliki hosta (np. pliki projektu, `~/.ssh`)?
- **Dostęp do sieci** — czy program może wysyłać dane na zewnątrz?
- **Zasoby hosta** — czy program może zużyć pamięć lub CPU, uruchamiać inne procesy?
- **Sekrety** — czy program widzi zmienne środowiskowe (np. `OLLAMA_URL`, `OLLAMA_MODEL`)?

Ograniczenia wbudowane w szkielet:

- Maksymalna długość kodu: 4000 znaków.
- Maksymalna długość wyniku zwracanego do agenta: 4000 znaków.
- Limit czasu: 5 sekund na wykonanie.
- Maksymalnie 4 próby na zadanie (1 próba i 3 poprawki).

**Nie uruchamiaj niezweryfikowanego kodu generowanego przez model na swoim komputerze poza laboratorium.** Proces lokalny (Python, Node.js, Jint) nie jest pełną izolacją bezpieczeństwa. Docker jest najbliżej izolacji, ale też nie jest gotową granicą bezpieczeństwa dla dowolnego kodu bez dodatkowej konfiguracji.

## Obsługa błędów agenta

Agent otrzymuje wynik wykonania (status, kod wyjścia, stdout, stderr) i może poprawić program. Narzędzie `ExecuteCodeTool` dopuszcza maksymalnie **trzy ponowienia** po błędzie, czyli łącznie cztery wykonania na jedno zadanie. Po wyczerpaniu limitu agent informuje użytkownika, zamiast wykonywać kolejne próby.

## Zadanie dodatkowe: Code Mode

Udostępnij wykonywanemu programowi co najmniej jedną kontrolowaną funkcję aplikacji, np. `getData(name)`. Agent powinien wygenerować kod, który samodzielnie wywoła tę funkcję i przetworzy otrzymane dane.

Ta część nie wymaga pełnej implementacji Code Mode ani integracji MCP. Wystarczy prosty, kontrolowany mechanizm.

## Kryteria zaliczenia

Przedstaw:

1. **Działającą integrację agenta** z wybranym runtime'em (agent generuje kod, wykonuje go i używa wyniku w odpowiedzi).
2. **Przykład wygenerowanego i wykonanego programu** (zrzut ekranu lub wklejony fragment konsoli).
3. **Przykład obsługi błędu** — program błędny składniowo lub wykonawczo i poprawka przez agenta.
4. **Krótkie uzasadnienie wyboru runtime'u** oraz opis ograniczeń bezpieczeństwa (sekcja „Bezpieczeństwo” wyżej).

Ocena dotyczy integracji Code Execution z agentem AI, a nie złożoności wygenerowanego programu.

## Plan pracy (90 minut)

| Etap | Czas |
|---|---:|
| Implementacja runtime'u | 35 min |
| Integracja i uruchomienie agenta | 20 min |
| Obsługa błędów | 15 min |
| Testy i analiza bezpieczeństwa | 20 min |
