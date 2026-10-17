# Raport — Laboratorium 03

**Zespół:** Karol Wójcik, Mateusz Rabantek, Julian Sowiński  
**Data:**  16.10.2026

## Środowisko

| Parametr | Wartość |
|----------|---------|
| Wersja Ollama | ta z lab3 readme|
| Sprzęt (CPU/GPU, RAM) | Apple M2, 16 GB|
| Liczba dostępnych notatek | 170 |

### Środowisko — Gemini API

| Parametr | Wartość |
|----------|---------|
| Dostawca modeli | Gemini API (Google AI Studio, darmowy tier), endpoint `generateContent` |
| Uruchamianie modeli | w chmurze, bez lokalnej Ollamy; klient `src/ModelLab/GeminiClient.cs` |
| Sprzęt | nie dotyczy, czasy obejmują opóźnienie sieci i API |

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

### EXP-01 — Gemini API

| Model | Floor accuracy | Elevator accuracy | Exact match | Średni czas [ms] |
|-------|----------------|-------------------|-------------|------------------|
| gemini-3.5-flash-lite | 1 | 0.8 | 0.8 | 594 |
| gemini-3.8-flash | 1 | 0.9 | 0.9 | 38241 |


**Exact match** oznacza odsetek notatek, dla których model poprawnie rozpoznał jednocześnie oba pola: `floor` i `elevator`.

**Wnioski:**

- Wszystkie modele poza `deepseek-r1` bezbłędnie rozpoznają piętro, także w formach „IV piętro”, „2p”, „Parter”, „-1” i przy niejednoznaczności („Drugie lub trzecie piętro” → `null`).
- Najlepszy wynik osiągnął lokalny `qwen3:4b` (exact match 1.0). Modele Gemini myliły się wyłącznie na polu `elevator`.
- `gemini-3.8-flash` jest lepszy od `flash-lite` o jedną notatkę (poprawnie zinterpretował NOTE-007), ale ok. 64 razy wolniejszy (38 s vs 0,6 s). Jego czas jest podobny do lokalnych modeli 4B na M2.
- `gemini-3.5-flash-lite` jest ok. 60 razy szybszy od modeli lokalnych przy exact match 0.8.

## EXP-02 — Prompting

| Wariant | Floor accuracy | Elevator accuracy | Exact match |
|---------|----------------|-------------------|-------------|
| zero-shot | 0.6| 0.6| 0.5|
| one-shot | 0.9| 0.8| 0.7|
| few-shot | 0.8| 0.8| 0.7|

### EXP-02 — Gemini API (gemini-3.5-flash-lite)

| Wariant | Floor accuracy | Elevator accuracy | Exact match |
|---------|----------------|-------------------|-------------|
| zero-shot | 1 | 0.8 | 0.8 |
| one-shot | 1 | 0.8 | 0.8 |
| few-shot | 1 | 0.8 | 0.8 |

**Wnioski:**

- Dla słabszego `deepseek-r1` przykłady wyraźnie pomogły: exact match wzrósł z 0.5 do 0.7, głównie dzięki lepszemu rozpoznawaniu piętra.
- Dla `gemini-3.5-flash-lite` przykłady nie zmieniły nic. We wszystkich wariantach model mylił te same dwie notatki (NOTE-007, NOTE-010).

## EXP-03 — Temperature

| Temperature | Exact match | Powtarzalne notatki |
|-------------|-------------|---------------------|
| 0.0 | | |
| 0.5 | | |
| 1.0 | | |

### EXP-03 — Gemini API (gemini-3.5-flash-lite, 3 powtórzenia)

Liczbę powtórzeń zmniejszyliśmy z 5 do 3, bo przebieg z 5 powtórzeniami przekraczał dzienny limit darmowego tieru.

| Temperature | Exact match | Powtarzalne notatki |
|-------------|-------------|---------------------|
| 0.0 | 0.8 | 10/10 |
| 0.2 | 0.8 | 10/10 |
| 1.0 | 0.8 | 10/10 |

**Wnioski:**

- Temperatura nie wpłynęła ani na poprawność, ani na powtarzalność: przy każdej wartości, także 1.0, każda notatka dała 3 identyczne odpowiedzi.
- Błędy przy NOTE-007 i NOTE-010 są systematyczne, a nie losowe: model zawsze odpowiada tak samo źle. Temperatura tego nie naprawi, potrzebna jest zmiana promptu.

## EXP-04 — Structured output — JSON i walidacja

| Wariant | Poprawny JSON | Exact match |
|---------|---------------|-------------|
| prompt | | |
| json-schema | | |

### EXP-04 — Gemini API (gemini-3.5-flash-lite)

| Wariant | Poprawny JSON | Exact match |
|---------|---------------|-------------|
| prompt | 9/10 | 0.7 |
| json-mode | 10/10 | 0.8 |
| json-schema | 10/10 | 0.8 |

**Wnioski:**

## Najtrudniejsze notatki

Które notatki model najczęściej interpretował błędnie i dlaczego?

W eksperymentach z Gemini wszystkie błędy dotyczą pola `elevator` i tych samych dwóch notatek:

| Notatka | Oczekiwano | Model zwrócił | Gdzie |
|---------|------------|---------------|-------|
| NOTE-007 „4 piętro, winda tylko do 3” | `elevator: true` | `false` | flash-lite we wszystkich eksperymentach; 3.8-flash poprawnie |
| NOTE-010 „Winda nie działa, dostawa na 5p” | `elevator: true` | `false` | wszystkie modele Gemini, we wszystkich eksperymentach |

## Rekomendacja

Który model i prompt wybieramy do projektu — i jakie mamy dowody, że to dobry wybór?

gemini-3.5-flash-lite jest darmowy i nie trzeba lokalnie to plus.
