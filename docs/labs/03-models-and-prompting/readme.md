# Tworzenie agentów AI w .NET

## Laboratorium 03 — Modele i prompting

### Który model wybrać dla agenta i jak opisać, czego od niego oczekujemy?

---

**Marcin Sulecki**  
marcin.sulecki@pw.edu.pl

Na tym laboratorium sprawdzimy trzy rzeczy:

1. **Model** — jak wybór modelu wpływa na jakość i czas odpowiedzi?
2. **Prompt** — jak *zero-shot*, *one-shot* i *few-shot* wpływają na poprawność?
3. **Temperature** — jak *temperatura* wpływa na powtarzalność i poprawność odpowiedzi?

Dodatkowo sprawdzimy **structured output (JSON)**, ponieważ agent musi umieć przetwarzać odpowiedzi modelu programowo.

### Zadanie

Z notatki kierowcy wyodrębnij dwie informacje: piętro docelowe dostawy i informację o windzie. Sprawdzamy, czy model potrafi jednocześnie rozpoznawać kilka informacji, nie dopowiadając brakujących danych.

Przykładowe notatki (`data/examples.csv`):

| id | note | floor | elevator |
|----|------|-------|----------|
| NOTE-001 | Trzecie piętro, winda po prawej | 3 | true |
| NOTE-002 | 2p bez windy, drzwi na końcu | 2 | false |
| NOTE-003 | Parter, wejście od podwórza | 0 | null |
| NOTE-004 | IV piętro, lokal 12 | 4 | null |
| NOTE-005 | Winda na końcu korytarza | null | true |
| NOTE-006 | Mieszkanie 15, domofon 15 | null | null |
| NOTE-007 | 4 piętro, winda tylko do 3 | 4 | true |
| NOTE-008 | Piętro -1, brak windy | -1 | false |
| NOTE-009 | Drugie lub trzecie piętro | null | null |
| NOTE-010 | Winda nie działa, dostawa na 5p | 5 | true |

Oczekiwany wynik JSON:

```json
{
  "floor": 3,
  "elevator": true
}
```

Przyjmujemy następującą interpretację:

* `floor` — piętro docelowe dostawy; `null`, gdy nie można go jednoznacznie ustalić.
* `elevator` — `true`, gdy notatka potwierdza obecność windy, `false`, gdy wyraźnie stwierdza jej brak, `null`, gdy brak informacji.

> „Winda nie działa” oznacza `elevator: true`, ponieważ badamy obecność windy, a nie jej sprawność.

### Eksperymenty

| Eksperyment | Cel |
|-------------|-----|
| EXP-00 | Zaprojektowanie sposobu oceny wyników |
| EXP-01 | Porównanie dwóch modeli |
| EXP-02 | Porównanie zero-shot, one-shot i few-shot |
| EXP-03 | Zbadanie wpływu temperature |
| EXP-04 | Structured output — JSON i walidacja |

**[Instrukcje eksperymentów →](./experiments.md)**

### Instalacja Ollama i pobieranie modeli

Do wykonania eksperymentów potrzebna jest Ollama — narzędzie umożliwiające lokalne uruchamianie modeli językowych.

#### 1. Zainstaluj Ollama

Pobierz wersję dla swojego systemu (Windows, macOS lub Linux):

https://ollama.com/download

#### 2. Pobierz modele

W terminalu wykonaj:

```bash
ollama pull gemma3:4b
ollama pull qwen3:4b
```

Możesz również wybrać inny model obsługiwany przez Ollama, dostosowany do możliwości swojego komputera.

Dostępne modele: https://ollama.com/library

#### 3. Sprawdź pobrane modele

```bash
ollama list
```

#### 4. Sprawdź działanie modelu

```bash
ollama run gemma3:4b
```

Wpisz przykładowe pytanie:

```text
Na którym piętrze znajduje się lokal?
Notatka: "Budynek bez windy, lokal na 3p".
```

Aby zakończyć rozmowę, wpisz `/bye`.

#### 5. Uruchom eksperymenty

Upewnij się, że Ollama działa. W razie potrzeby uruchom serwer:

```bash
ollama serve
```

Następnie, z katalogu laboratorium:

```bash
dotnet run --project src/ModelLab -- EXP-01
dotnet run --project src/ModelLab -- EXP-02
dotnet run --project src/ModelLab -- EXP-03
dotnet run --project src/ModelLab -- EXP-04
```

Wyniki są dopisywane do `results/results.csv`, a raport przygotuj na podstawie `reports/template.md`.

Domyślnie eksperymenty używają modeli `gemma3:4b` i `qwen3:4b`. Inny model wskażesz parametrem `--model`. Można podać kilka modeli — powtarzając parametr albo rozdzielając nazwy przecinkiem:

```bash
dotnet run --project src/ModelLab -- EXP-01 --model gemma3:4b --model qwen2.5-coder
dotnet run --project src/ModelLab -- EXP-01 --model gemma3:4b,qwen2.5-coder,llama3.2
dotnet run --project src/ModelLab -- EXP-02 --model qwen2.5-coder
```

W EXP-01 modele są porównywane w jednym przebiegu, a EXP-02 ... EXP-04 uruchamiają się kolejno dla każdego podanego modelu (wiersze w `results.csv` różnią się kolumną modelu).

> **Uwaga:** nazwy modeli muszą odpowiadać modelom pobranym przez Ollama (`ollama list`).

> **Wskazówka:** modele 4B wymagają kilku GB pamięci RAM. Jeżeli komputer nie pozwala na ich płynne uruchamianie, wybierz mniejszy model. W eksperymencie EXP-01 porównuj modele na tym samym zbiorze notatek.

### Struktura

```
03-models-and-prompting/
├── experiments.md           # instrukcje eksperymentów
├── src/
│   ├── ModelLab/            # wspólny klient Ollama i uruchamianie eksperymentów
│   └── Experiments/
│       ├── EXP-01-model-comparison/
│       ├── EXP-02-prompting/
│       ├── EXP-03-temperature/
│       └── EXP-04-structured-output/
├── data/
│   ├── examples.csv         # notatki testowe z oczekiwanym wynikiem
│   ├── prompt-examples.csv  # przykłady do one-shot i few-shot
│   └── notes.csv            # prawdziwe notatki kierowców (bez etykiet)
└── reports/
    ├── README.md            # zestawienie raportów zespołów
    └── template.md
```
