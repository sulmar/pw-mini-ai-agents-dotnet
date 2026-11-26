# Zadanie dla Claude: Przygotowanie laboratorium Code Execution w .NET

## Cel

Przygotuj repozytorium startowe do 90-minutowego laboratorium **Code Execution w agentach AI**.

Studenci otrzymają działającą aplikację konsolową .NET z agentem Microsoft Agent Framework. Ich zadaniem będzie samodzielna implementacja integracji z wybranym środowiskiem wykonawczym.

**Nie implementuj za studentów runtime'ów.**

## 1. Architektura

Zastosuj następujące komponenty:

- `ICodeRuntime` – wspólny interfejs wykonywania kodu.
- `ExecutionRequest` – język, kod źródłowy, limit czasu.
- `ExecutionResult` – powodzenie, stdout, stderr, exit code, czas wykonania.
- `ExecuteCodeTool` – narzędzie udostępniane agentowi.
- `StopwatchRuntime` – dekorator mierzący czas wykonania.
- `TimeoutRuntime` – dekorator kontrolujący limit czasu.
- `PythonRuntime` – szkielet do implementacji przez studenta.
- `JintRuntime` – alternatywny szkielet.
- `NodeRuntime` – alternatywny szkielet.
- `DockerRuntime` – opcjonalny wariant zaawansowany.

Architektura wywołania:

Agent → ExecuteCodeTool → StopwatchRuntime → TimeoutRuntime → ICodeRuntime

Wszystkie runtime'y muszą implementować ten sam interfejs.

## 2. Aplikacja startowa

Utwórz aplikację konsolową .NET wykorzystującą Microsoft Agent Framework.

Wymagania:

1. Konfiguracja modelu przez zmienne środowiskowe.
2. Rejestracja narzędzia ExecuteCodeTool.
3. Interaktywna rozmowa z agentem.
4. Agent generuje kod odpowiedni do polecenia użytkownika.
5. Agent wykonuje kod za pomocą narzędzia.
6. Wynik wykonania wraca do agenta.
7. W przypadku błędu agent może poprawić kod i ponowić wykonanie, maksymalnie trzy razy.

Wykorzystaj aktualne, stabilne lub odpowiednio oznaczone wersje pakietów. Zweryfikuj rzeczywiste API Microsoft Agent Framework zamiast zakładać, że przykłady ze starszych wersji nadal działają.

## 3. Kontrakty i dekoratory

Zaimplementuj kompletne kontrakty oraz dekoratory.

**StopwatchRuntime:**
- Mierzy czas całej operacji.
- Uzupełnia DurationMs w ExecutionResult.
- Nie zawiera logiki specyficznej dla języka.

**TimeoutRuntime:**
- Wymusza limit czasu.
- Przekazuje CancellationToken do runtime'u.
- Zapewnia jednoznaczny rezultat przekroczenia limitu.
- Nie zakłada, że anulowanie Task automatycznie kończy proces systemowy.

Ustal i udokumentuj odpowiedzialność runtime'u za zatrzymanie procesu lub wykonania po anulowaniu.

## 4. Implementacje dla studentów

Przygotuj osobne klasy:

- PythonRuntime
- JintRuntime
- NodeRuntime
- DockerRuntime

Pozostaw metodę ExecuteAsync bez implementacji, z komentarzami TODO.

Każda klasa powinna zawierać krótkie wskazówki dotyczące odpowiednich bibliotek i mechanizmów integracji.

Student wybiera **jeden runtime**, nie musi implementować wszystkich.

Przygotuj konfigurację pozwalającą łatwo przełączyć wybrany runtime bez modyfikowania logiki agenta.

## 5. Testy

Dodaj testy jednostkowe dekoratorów wykorzystujące fikcyjny runtime, bez zależności od interpretera Python lub Node.js.

Przygotuj testy kontraktowe, które student uruchomi po implementacji runtime'u:

1. Wykonanie poprawnego programu.
2. Odczyt wyniku ze standardowego wyjścia.
3. Obsługa błędu składniowego.
4. Obsługa błędu wykonania.
5. Przekroczenie limitu 5 sekund.
6. Poprawne zakończenie wykonywania po anulowaniu.

Testy wymagające konkretnego runtime'u powinny być pomijane do czasu jego implementacji, a nie powodować błędu całego projektu startowego.

## 6. Bezpieczeństwo

Nie traktuj Jint ani zwykłego procesu Python/Node.js jako pełnej izolacji bezpieczeństwa.

Wprowadź ograniczenia długości kodu wejściowego i rozmiaru wyniku. Nie przekazuj wykonywanemu kodowi kluczy API modelu.

Udokumentuj zagrożenia związane z dostępem do systemu plików, sieci i zasobów hosta.

Nie uruchamiaj niezweryfikowanego kodu wygenerowanego przez model na hoście deweloperskim podczas automatycznej weryfikacji projektu.

## 7. Dokumentacja dla studentów

Przygotuj krótki README zawierający:

- Cel laboratorium.
- Instrukcję uruchomienia.
- Opis wyboru runtime'u.
- Wskazanie miejsc TODO.
- Trzy przykładowe zadania testowe.
- Kryteria zaliczenia.

Zaplanuj pracę na 90 minut:

| Etap | Czas |
|---|---:|
| Implementacja runtime'u | 35 min |
| Integracja i uruchomienie agenta | 20 min |
| Obsługa błędów | 15 min |
| Testy i analiza bezpieczeństwa | 20 min |

Nie dodawaj rozbudowanej infrastruktury ani zbędnych abstrakcji.

## 8. Zadanie dodatkowe: Code Mode

Zaprojektuj możliwość późniejszego udostępnienia wykonywanemu programowi kontrolowanych funkcji aplikacji, np. getData(name).

Nie implementuj jeszcze pełnego Code Mode ani integracji MCP. Przygotuj jedynie krótką propozycję rozszerzenia architektury.

## 9. Weryfikacja

Po implementacji:

1. Przywróć pakiety NuGet.
2. Skompiluj całe rozwiązanie.
3. Uruchom testy niewymagające implementacji studenckich.
4. Sprawdź, czy projekt można uruchomić po dostarczeniu implementacji ICodeRuntime.
5. Zweryfikuj, że w klasach runtime'ów pozostają miejsca TODO.
6. Przedstaw strukturę repozytorium oraz listę przygotowanych plików.

**Ważne:** aplikacja startowa ma się kompilować, mimo że implementacje runtime'ów są niekompletne. Ich wywołanie może zgłaszać NotImplementedException.

## Oczekiwany rezultat

Gotowe repozytorium startowe do laboratorium, w którym student koncentruje się na najważniejszym zagadnieniu: **integracji środowiska Code Execution z agentem AI**.

Priorytetem jest prostota architektury, czytelność kodu i możliwość ukończenia ćwiczenia w 90 minut.