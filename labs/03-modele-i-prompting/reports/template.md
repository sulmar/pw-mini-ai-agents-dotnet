# Raport — Laboratorium 03

**Zespół:** Karol Wójcik, Mateusz Rabantek, Julian Sowiński  
**Data:**  16.10.2026

## Środowisko

| Parametr | Wartość |
|----------|---------|
| Wersja Ollama | ta z lab3 readme|
| Sprzęt (CPU/GPU, RAM) | Apple M2, 16 GB|
| Liczba dostępnych notatek | 170 |

## EXP-00 — Sposób oceny wyników

Jak oceniamy poprawność odpowiedzi modelu? Jakie metryki przyjmujemy i dlaczego?

| Metryka | Definicja |
|---------|-----------|
| Floor accuracy | Odsetek notatek, w których `floor` zwrócony przez model jest równy oczekiwanemu (łącznie z `null`): poprawne `floor` / wszystkie notatki |
| Elevator accuracy | Odsetek notatek, w których `elevator` zwrócony przez model jest równy oczekiwanemu (`true` / `false` / `null`): poprawne `elevator` / wszystkie notatki |
| Exact match | Odsetek notatek, w których **oba** pola są poprawne jednocześnie: notatki z poprawnym `floor` i `elevator` / wszystkie notatki. To główna metryka, bo agent potrzebuje obu wartości naraz |
| Poprawny JSON | Liczba odpowiedzi, które przeszły walidację (obiekt JSON, oba pola obecne, `floor` to liczba całkowita lub `null`, `elevator` to wartość logiczna lub `null`) / wszystkie odpowiedzi |
| Średni czas [ms] | Średni czas od wysłania zapytania do otrzymania odpowiedzi modelu, liczony na jedną notatkę. Mierzy koszt i opóźnienie agenta |
| Powtarzalne notatki | Liczba notatek, dla których wszystkie 5 powtórzeń tego samego zapytania dało identyczną odpowiedź (EXP-03). Mierzy stabilność modelu |

## EXP-01 — Porównanie modeli

| Model | Floor accuracy | Elevator accuracy | Exact match | Średni czas [ms] |
|-------|----------------|-------------------|-------------|------------------|
| qwen3:4b| 1| 1| 1| 37762|
| deepseek-r1| 0.6| 0.6| 0.5| 39381|


**Exact match** oznacza odsetek notatek, dla których model poprawnie rozpoznał jednocześnie oba pola: `floor` i `elevator`.

**Wnioski:**

## EXP-02 — Prompting

| Wariant | Floor accuracy | Elevator accuracy | Exact match |
|---------|----------------|-------------------|-------------|
| zero-shot | 0.6| 0.6| 0.5|
| one-shot | 0.9| 0.8| 0.7|
| few-shot | 0.8| 0.8| 0.7|

**Wnioski:**

## EXP-03 — Temperature

| Temperature | Exact match | Powtarzalne notatki |
|-------------|-------------|---------------------|
| 0.0 | | |
| 0.5 | | |
| 1.0 | | |

**Wnioski:**

## EXP-04 — Structured output — JSON i walidacja

| Wariant | Poprawny JSON | Exact match |
|---------|---------------|-------------|
| prompt | | |
| json-schema | | |

**Wnioski:**

## Najtrudniejsze notatki

Które notatki model najczęściej interpretował błędnie i dlaczego?

## Rekomendacja

Który model i prompt wybieramy do projektu — i jakie mamy dowody, że to dobry wybór?
