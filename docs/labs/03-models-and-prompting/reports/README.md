# Zestawienie raportów — Laboratorium 03

Zadanie: ekstrakcja z notatek kierowców pól `floor` (piętro) i `elevator` (czy jest winda) do JSON.

## Zespoły

| Zespół | Raport | Środowisko | Modele |
|---|---|---|---|
| Ewelina Karpińska, Maciej Mazurek | [raport-karpinska-mazurek.md](raport-karpinska-mazurek.md) | Ollama 0.40.9, ARM, 32 GB RAM (CPU) | llama3.2:1b, qwen2.5:1.5b, gemma2:2b |
| Michał Mueller, Kazimierz Krześniak, Jakub Motyka | [raport-MMK.md](raport-MMK.md) | API PCSS | Bielik 11B, DeepSeek-V3.1, DeepSeek-V4-Flash, Qwen3.5-397B-A17B |
| Igor Kowalik, Aleksander Sergiej, Wiktor Szymonek | [raport-ikowalik-wszymonek-asergiej.md](raport-ikowalik-wszymonek-asergiej.md) | OpenRouter | Xiaomi mimo-v2.6-flash, Mistral large-4.0 |
| Hubert Zalewski, Paweł Zakrzewski | [raport-zalewski-zakrzewski.pdf](raport-zalewski-zakrzewski.pdf) | Ollama 0.40.13, i5-12400F + RTX 3070, 16 GB RAM | gemma3:4b, llama3.2:3b, qwen3:4b |

Wszystkie zespoły testowały na ok. 10 notatkach z `examples.csv`, więc różnica 10 p.p. to jedna notatka.

## EXP-01 — Porównanie modeli

| Zespół | Model | Floor | Elevator | Exact match | Średni czas [ms] |
|---|---|---|---|---|---|
| Karpińska/Mazurek | llama3.2:1b | 50% | 30% | 20% | 1664 |
| Karpińska/Mazurek | qwen2.5:1.5b | 70% | 30% | 20% | 1324 |
| Karpińska/Mazurek | gemma2:2b | 80% | 70% | 60% | 4010 |
| Zalewski/Zakrzewski | llama3.2:3b | 80% | 50% | 40% | 136 |
| Zalewski/Zakrzewski | gemma3:4b | 80% | 70% | 60% | 501 |
| Zalewski/Zakrzewski | **qwen3:4b** | **100%** | **100%** | **100%** | 8519 |
| MMK | Bielik 11B | 20% | 30% | 20% | 1998 |
| MMK | DeepSeek-V4-Flash | 70% | 70% | 60% | 213 |
| MMK | DeepSeek-V3.1 | 100% | 80% | 80% | 683–953 |
| MMK | Qwen3.5-397B-A17B | 100% | 90% | 90% | 12734 |
| Kowalik/Sergiej/Szymonek | Xiaomi mimo-v2.6-flash | 40% | 40% | 40% | 15486 |
| Kowalik/Sergiej/Szymonek | Mistral large-4.0 | 80% | 70% | 70% | 11360 |

Uwaga: zespoły różnie liczyły niepoprawny JSON — część uznawała go za błędną odpowiedź (Kowalik/Sergiej/Szymonek, Zalewski/Zakrzewski), więc wyniki modeli „opakowujących” JSON w markdown są tam zaniżone.

## EXP-02 — Prompting (exact match)

| Zespół | Model | zero-shot | one-shot | few-shot |
|---|---|---|---|---|
| Karpińska/Mazurek | llama3.2:1b | 20% | 10% | 20% |
| Karpińska/Mazurek | qwen2.5:1.5b | 20% | 10% | 10% |
| Karpińska/Mazurek | gemma2:2b | 60% | 60% | **80%** |
| Zalewski/Zakrzewski | llama3.2:3b | 40% | **50%** | 20% |
| Zalewski/Zakrzewski | gemma3:4b | 60% | **90%** | 70% |
| Zalewski/Zakrzewski | qwen3:4b | 100% | 100% | 100% |
| MMK | DeepSeek-V4-Flash | 57% | **75%** | **75%** |
| Kowalik/Sergiej/Szymonek | Mistral large-4.0 | 90% | **100%** | **100%** |

## EXP-03 — Temperature (exact match)

| Zespół | Model | 0.0 | 0.5 | 1.0 |
|---|---|---|---|---|
| Karpińska/Mazurek | llama3.2:1b | 20% | 36% | 26% |
| Zalewski/Zakrzewski | gemma3:4b | 60% | 60% | 62% |
| Zalewski/Zakrzewski | llama3.2:3b | 40% | 40% | 28% |
| Zalewski/Zakrzewski | qwen3:4b | 100% | 100% | 100% |
| MMK | DeepSeek-V4-Flash | 62% | 67% | 61% |
| Kowalik/Sergiej/Szymonek | Mistral large-4.0 | **84%** | 78% | 64% |

Przy 0.0 wyniki są powtarzalne we wszystkich zespołach; przy 1.0 rośnie wariancja i pojawiają się niepoprawne JSON-y.

## EXP-04 — Structured output

| Zespół | Model | Poprawny JSON: prompt → json-schema | Exact match: prompt → json-schema |
|---|---|---|---|
| Karpińska/Mazurek | qwen2.5:1.5b | 0/10 → 10/10 | 20% → 20% |
| Karpińska/Mazurek | gemma2:2b | 0/10 → 10/10 | 60% → 60% |
| Zalewski/Zakrzewski | gemma3:4b | 10/10 → 10/10 | 60% → 70% |
| Zalewski/Zakrzewski | llama3.2:3b | 10/10 → 10/10 | 40% → 40% |
| Zalewski/Zakrzewski | qwen3:4b | 10/10 → 10/10 | 100% → 100% |
| MMK | DeepSeek-V4-Flash | 9/10 → 10/10 | 70% → 70% |
| Kowalik/Sergiej/Szymonek | Mistral large-4.0 | 70% → 100% | 70% → 100% |

## Najtrudniejsze notatki (powtarzają się w raportach)

- **„Winda nie działa”** — modele zwracają `elevator=false`, mylą stan techniczny z obecnością windy (3 zespoły).
- **„4 piętro, winda tylko do 3”** (NOTE-007) — błędy w piętrze albo windzie (2 zespoły).
- **„Drugie lub trzecie piętro”** (NOTE-009) — model wybiera jedną wartość zamiast `null` (2 zespoły).
- **Brak informacji o windzie** (NOTE-003/004) — model „dopowiada” `false` zamiast `null`.
- Format: odpowiedź opakowana w ```` ```json ```` łamie parser (wszystkie zespoły).

## Wspólne wnioski

1. **json-schema rozwiązuje problem formatu** — u każdego zespołu dało 100% poprawnych JSON-ów. Trafność rośnie tylko tam, gdzie wcześniej JSON był liczony jako błąd.
2. **Przykłady w prompcie pomagają**, ale więcej nie zawsze znaczy lepiej — u Zalewskiego/Zakrzewskiego few-shot był gorszy od one-shot (dla llama3.2:3b nawet od zero-shot).
3. **Temperatura 0.0** to bezpieczny wybór do ekstrakcji: najwyższa powtarzalność, podobna lub lepsza trafność.
4. **Wielkość modelu nie przesądza**: lokalny qwen3:4b (tryb myślenia) osiągnął 100%, a Bielik 11B przez API — 20%. Ceną za „myślenie” jest czas (kilka–kilkanaście sekund na notatkę).
5. Modele najczęściej zgadują zamiast zwrócić `null` — warto to jawnie pokazać w przykładach.

## Rekomendacje zespołów

| Zespół | Rekomendacja |
|---|---|
| Karpińska/Mazurek | gemma2:2b + few-shot + json-schema, temperatura 0.0 |
| MMK | DeepSeek-V3.1 |
| Kowalik/Sergiej/Szymonek | Mistral large-4.0 + json-schema, temperatura 0.0 (opcjonalnie few-shot) |
| Zalewski/Zakrzewski | qwen3:4b + zero-shot + json-schema, temperatura 0; dla szybkości gemma3:4b + one-shot |
