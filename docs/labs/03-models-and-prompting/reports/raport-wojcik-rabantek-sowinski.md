# Raport — Laboratorium 03

**Zespół:** Karol Wójcik, Mateusz Rabantek, Julian Sowiński  
**Data:**  16.10.2026

## Środowisko

| Parametr | Wartość |
|----------|---------|
| Wersja Ollama | ta z lab3 readme|
| Sprzęt 1 (modele 4B / R1) | Apple M2, 16 GB|
| Sprzęt 2 (gemma4:12b) | PC z dedykowanym GPU (16 GB VRAM), 32 GB RAM |
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
| gemma4:12b| 1| 1| 1| 1990|

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
- Spośród modeli lokalnych bezbłędny wynik (`exact match` 1.0) osiągnęły `qwen3:4b` oraz `gemma4:12b`.
- Uruchomienie większego modelu `gemma4:12b` na komputerze z dedykowaną kartą graficzną pozwoliło skrócić średni czas odpowiedzi do ok. 2 sekund (1990 ms) — to prawie 20-krotnie szybciej niż mniejszy model `qwen3:4b` działający na procesorze Apple M2 (37,7 s).
- `gemini-3.5-flash-lite` pozostaje najszybszy bezwzględnie (594 ms), jednak ustępuje modelom `qwen3:4b` i `gemma4:12b` pod względem dokładności (0.8 vs 1.0).

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

## EXP-02 — gemma4:12b

| Wariant | Floor accuracy | Elevator accuracy | Exact match |
|---------|----------------|-------------------|-------------|
| zero-shot | 1| 1| 1|
| one-shot | 1| 1| 1|
| few-shot | 1| 1| 1|

**Wnioski:**

- Dla słabszego `deepseek-r1` przykłady wyraźnie pomogły: exact match wzrósł z 0.5 do 0.7, głównie dzięki lepszemu rozpoznawaniu piętra.
- Dla `gemini-3.5-flash-lite` przykłady nie zmieniły nic. We wszystkich wariantach model mylił te same dwie notatki (NOTE-007, NOTE-010).
- Model `gemma4:12b` osiągnął wynik 1.0 już w wariancie `zero-shot` (oraz w `one-shot` i `few-shot`). Dzięki większej liczbie parametrów (12B) sama instrukcja systemowa w zupełności wystarcza do bezbłędnej interpretacji trudnych przypadków — dodawanie przykładów do promptu nie jest konieczne, co pozwala oszczędzić tokeny wejściowe.

## EXP-03 — Temperature (gemma4:12b)

| Temperature | Exact match | Powtarzalne notatki |
|-------------|-------------|---------------------|
| 0.0 | 1| 10/10|
| 0.2 | 1| 10/10|
| 1.0 | 1| 10/10|

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
- Dla modelu `gemma4:12b` zmiana temperatury w zakresie od `0.0` do `1.0` nie wywołała żadnych wahań: w każdym wariancie model utrzymał 100% skuteczności (`exact match` 1.0) oraz pełną powtarzalność (10/10 identycznych odpowiedzi w 5 próbach).

## EXP-04 — Structured output — JSON i walidacja (gemma4:12b)

| Wariant | Poprawny JSON | Exact match |
|---------|---------------|-------------|
| prompt | 0/10| 0|
| json-schema | 10/10| 1|

### EXP-04 — Gemini API (gemini-3.5-flash-lite)

| Wariant | Poprawny JSON | Exact match |
|---------|---------------|-------------|
| prompt | 9/10 | 0.7 |
| json-mode | 10/10 | 0.8 |
| json-schema | 10/10 | 0.8 |

**Wnioski:**

- W wariancie `prompt` model `gemma4:12b` uzyskał wynik 0/10 poprawnych obiektów JSON (`exact match` 0). Z logów wynika, że model poprawnie wyciągał informacje z notatek, ale domyślnie opakowywał odpowiedź w znaczniki Markdown (` ```json ... ``` `), co powodowało odrzucenie odpowiedzi przez walidator.
- Włączenie natywnego wymuszania struktury (`json-schema` / `format` w Ollama) całkowicie rozwiązało ten problem dla `gemma4:12b`: poprawność formatu wzrosła z 0/10 do 10/10, a `exact match` z 0 do 1.0.
- Podobną tendencję widać w `gemini-3.5-flash-lite`, gdzie sam `prompt` zawiódł w 1 na 10 przypadków (9/10), natomiast tryby `json-mode` i `json-schema` zagwarantowały 100% poprawności składniowej (10/10).
- W systemach agentowych poleganie wyłącznie na prompcie przy generowaniu formatu JSON jest ryzykowne — zawsze należy stosować natywne wymuszanie schematu (`Structured Outputs`).

## Najtrudniejsze notatki

Które notatki model najczęściej interpretował błędnie i dlaczego?

W eksperymentach z Gemini wszystkie błędy dotyczą pola `elevator` i tych samych dwóch notatek:

| Notatka | Oczekiwano | Model zwrócił | Gdzie |
|---------|------------|---------------|-------|
| NOTE-007 „4 piętro, winda tylko do 3” | `elevator: true` | `false` | flash-lite we wszystkich eksperymentach; 3.8-flash poprawnie |
| NOTE-010 „Winda nie działa, dostawa na 5p” | `elevator: true` | `false` | wszystkie modele Gemini, we wszystkich eksperymentach |

Dla porównania, lokalny model `gemma4:12b` (po zastosowaniu `json-schema`) poprawnie poradził sobie z obiema problematycznymi notatkami (`NOTE-007` oraz `NOTE-010`). W przeciwieństwie do modeli Gemini nie pomylił fizycznego istnienia windy w budynku (`elevator: true`) z jej tymczasową awarią lub ograniczonym zasięgiem.

## Rekomendacja

Który model i prompt wybieramy do projektu — i jakie mamy dowody, że to dobry wybór?

gemini-3.5-flash-lite jest darmowy i nie trzeba lokalnie to plus.
Rozwiązanie lokalne: `gemma4:12b`
- Daje bezbłędną dokładność (`exact match` 1.0) i pełną powtarzalność (10/10). Na komputerze z dedykowaną kartą graficzną odpowiada w niespełna 2 sekundy (1990 ms), co stanowi bardzo dobry kompromis między czasem odpowiedzi a jakością.
