# Raport — Laboratorium 03

**Zespół:** Michał Chrostowski, Bartosz Dźwigała, Tomasz Stolarczyk
**Data:** 22.10.2026

## Środowisko

| Parametr | Wartość |
|----------|---------|
| Wersja Ollama | 0.40.12 (modele lokalne) + API PCSS (modele zdalne) |
| Sprzęt (CPU/GPU, RAM) | i7-9700F, GTX 1660 Ti 6 GB, 16 GB RAM |
| Liczba dostępnych notatek | 170 (do ewaluacji użyto 10 z `examples.csv`) |

## EXP-00 — Sposób oceny wyników

| Metryka | Definicja |
|---------|-----------|
| Floor accuracy | Odsetek poprawnie rozpoznanych pięter |
| Elevator accuracy | Odsetek poprawnie rozpoznanych informacji o windzie |
| Exact match | Odsetek notatek z oboma polami poprawnymi (główna metryka) |
| Poprawny JSON | Odpowiedzi, które da się sparsować do wymaganego formatu |
| Średni czas [ms] | Czas odpowiedzi; ważny dla czatu z gościem |

## EXP-01 — Porównanie modeli

| Model | Floor accuracy | Elevator accuracy | Exact match | Średni czas [ms] |
|-------|----------------|-------------------|-------------|------------------|
| gemma3:4b | 0.9 | 0.7 | 0.6 | 1070 |
| qwen3:4b | 1.0 | 1.0 | 1.0 | 13758 |
| deepseek-r1:8b | 1.0 | 0.6 | 0.6 | 30586 |
| DeepSeek-V4-Flash (PCSS) | 0.8 | 0.8 | 0.6 | 257 |
| GLM-5.3-Flash (PCSS) | 1.0 | 1.0 | 1.0 | 1346 |
| Mistral-Small-3.2:24b (PCSS) | 0.9 | 0.8 | 0.7 | 559 |

**Exact match** oznacza odsetek notatek, dla których model poprawnie rozpoznał jednocześnie oba pola: `floor` i `elevator`.

**Wnioski:** Większy model nie oznacza lepszego (deepseek-r1:8b < qwen3:4b). Lokalne modele „myślące” są dokładne, ale za wolne do czatu (14–31 s). Tylko GLM-5.3-Flash łączy wynik 1.0 z czasem ok. 1 s. Pole `elevator` jest trudniejsze od `floor` dla wszystkich modeli.

## EXP-02 — Prompting

| Model | Wariant | Floor accuracy | Elevator accuracy | Exact match |
|-------|---------|----------------|-------------------|-------------|
| gemma3:4b | zero / one / few-shot | 0.9 / 0.9 / 0.8 | 0.7 / 0.9 / 0.9 | 0.6 / 0.8 / 0.7 |
| DeepSeek-V4-Flash | zero / one / few-shot | 0.8 / 0.9 / 1.0 | 0.8 / 0.8 / 0.8 | 0.6 / 0.7 / 0.8 |
| Mistral-Small-3.2:24b | zero / one / few-shot | 0.9 / 0.9 / 1.0 | 0.8 / 0.8 / 0.8 | 0.7 / 0.7 / 0.8 |
| qwen3:4b, GLM-5.3-Flash | wszystkie | 1.0 | 1.0 | 1.0 |

**Wnioski:** Przykłady podnoszą exact match słabszych modeli o 0.1–0.2, głównie przez poprawę `floor`. Błędy `elevator` zostają, bo przykłady nie pokrywają przypadku „winda nie działa”. Najlepszym modelom przykłady nie szkodzą i nie spowalniają ich.

## EXP-03 — Temperature

Exact match przy 5 powtórzeniach każdej notatki:

| Model | 0.0 | 0.5 | 1.0 |
|-------|-----|-----|-----|
| gemma3:4b | 0.60 | 0.60 | 0.66 |
| DeepSeek-V4-Flash | 0.60 | 0.68 | 0.72 |
| Mistral-Small-3.2:24b | 0.70 | 0.64 | 0.64 |
| GLM-5.3-Flash | 1.00 (10/10 powtarzalnych) | 1.00 | 1.00 |

**Wnioski:** Wyższa temperatura czasem „trafia” w poprawną odpowiedź, ale losowo, więc nie można na tym polegać. Do ekstrakcji danych i decyzji agenta wybieramy temperaturę **0**, a do generowania treści można ją podnieść (0.5–0.7).

## EXP-04 — Structured output — JSON i walidacja

| Model | Wariant | Poprawny JSON | Exact match |
|-------|---------|---------------|-------------|
| gemma3:4b | prompt / json-schema | 10/10 / 10/10 | 0.6 / 0.7 |
| deepseek-r1:8b | prompt / json-schema | 10/10 / 10/10 | 0.6 / 0.6 |
| DeepSeek-V4-Flash | prompt / json-schema | 10/10 / 10/10 | 0.6 / 0.6 |
| GLM-5.3-Flash | prompt / json-schema | 10/10 / 10/10 | 1.0 / 1.0 |
| Mistral-Small-3.2:24b | prompt / json-schema | 10/10 / 10/10 | 0.7 / 0.7 |

**Wnioski:** Poprawny JSON nie oznacza poprawnej odpowiedzi: przy 10/10 poprawnych JSON-ów exact match wynosi od 0.6 do 1.0. Wiele modeli opakowuje JSON w blok Markdown (gemma3 bez obsługi tego w parserze: 0/10), dlatego json-schema warto włączać zawsze.

## Najtrudniejsze notatki

- **NOTE-010 „Winda nie działa, dostawa na 5p”**: 4 z 6 modeli zwracają `elevator: false`, bo mylą obecność windy z jej sprawnością. Poprawnie: tylko qwen3:4b i GLM-5.3-Flash.
- **NOTE-007 „4 piętro, winda tylko do 3”**: ten sam błąd u DeepSeek-V4-Flash, Mistral-Small i deepseek-r1:8b.
- **NOTE-003 „Parter…”**, **NOTE-009 „Drugie lub trzecie piętro”**: gemma3 i deepseek-r1 dopowiadają `elevator: false` mimo braku informacji o windzie (oczekiwane `null`). Podobnie gemma3 bierze numer mieszkania za piętro (NOTE-006 → `floor: 15`). Dla naimij to najgroźniejszy typ błędu, bo agent ma nie zgadywać.
- **NOTE-001 „Trzecie piętro”** i **NOTE-004 „IV piętro”**: DeepSeek-V4-Flash zwraca 2 i 3 (o jedno piętro za nisko), Mistral-Small myli się przy „Trzecie”. Liczebniki słowne i rzymskie są trudniejsze niż cyfry („2p”, „-1” rozpoznawane poprawnie). Few-shot poprawia `floor` do 1.0.

## Rekomendacja

Dla projektu **naimij** wybieramy:

- **Model:** **GLM-5.3-Flash** (PCSS). Jako jedyny ma wynik 1.0 we wszystkich eksperymentach, odpowiada w ok. 1 s, a dostęp studencki jest darmowy.
- **Prompt:** few-shot z przykładami przypadków brzegowych.
- **Wyjście:** json-schema wszędzie, gdzie odpowiedź trafia do kodu.
- **Temperatura:** 0 dla decyzji i wywołań narzędzi, niska (0–0.3) dla odpowiedzi do gościa, bo te mają opierać się na dokumentacji lokalu.
- **Zapas:** Mistral-Small-3.2:24b (API), qwen3:4b lokalnie do testów offline.
