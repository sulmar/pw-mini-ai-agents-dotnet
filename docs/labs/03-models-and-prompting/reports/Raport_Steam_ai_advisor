# Raport — Laboratorium 03

**Zespół: Jakub Jaworski, Patryk Chachulski**  

## Środowisko

| Parametr | Wartość |
|----------|---------|
| Wersja Ollama | gemma3:4b, qwen3:4b, llama3.2:3b|
| Sprzęt (CPU/GPU, RAM) | AMD Ryzen 5 7535HS (6 rdzeni / 12 wątków), AMD Radeon 660M (iGPU), 32 GB RAM |
| Liczba dostępnych notatek | 170 |

Eksperymenty uruchamiane lokalnie przez Ollamę. `qwen3:4b` przerwano po EXP-01 i EXP-02, bo czas wykonywania był za długi. EXP-03 i EXP-04 wykonano więc tylko dla `gemma3:4b` i `llama3.2:3b`.

## EXP-00 — Sposób oceny wyników
| Metryka | Definicja |
|---------|-----------|
| Floor accuracy | Odsetek notatek z poprawnie rozpoznanym polem `floor` |
| Elevator accuracy | Odsetek notatek z poprawnie rozpoznanym polem `elevator` |
| Exact match | Odsetek notatek, dla których oba pola są jednocześnie poprawne |
| Poprawny JSON | Liczba odpowiedzi, które dało się sparsować do obiektu z polami `floor` i `elevator` o właściwych typach |
| Średni czas [ms] | Średni czas odpowiedzi modelu na jedną notatkę |
| Powtarzalne notatki | liczba notatek, dla których wszystkie powtórzenia zwróciły identyczną odpowiedź |

## EXP-01 — Porównanie modeli

| Model | Floor accuracy | Elevator accuracy | Exact match | Poprawny JSON | Średni czas [ms] |
|-------|----------------|-------------------|-------------|---------------|------------------|
| gemma3:4b | 0% | 0% | 0% | 0/10 | 3720 |
| qwen3:4b | 100% | 100% | 100% | 10/10 | 59239 |
| llama3.2:3b | 50% | 40% | 20% | 8/10 | 1470 |

**Exact match** oznacza odsetek notatek, dla których model poprawnie rozpoznał jednocześnie oba pola: `floor` i `elevator`.

**Wnioski:**

`qwen3:4b` jako jedyny osiągnął 100% exact match i 10/10 poprawnego JSON, ale średnio **59 s** na notatkę — ok. 16× wolniej niż Gemma i ok. 40× wolniej niż Llama.

Zera Gemmy w tym eksperymencie nie oznaczają, że model nie rozumie notatek. Parser odrzuca wszystko, co nie jest czystym obiektem JSON (np. odpowiedź w bloku Markdown). EXP-04 pokazuje, że po wymuszeniu JSON Schema Gemma skacze do 60% exact match. Llama jest najszybsza (~1,5 s), ale przy zero-shot trafia obie pola tylko w 2 na 10 notatek i w 2 przypadkach w ogóle nie zwraca poprawnego JSON.

## EXP-02 — Prompting
| Model | Wariant | Floor accuracy | Elevator accuracy | Exact match | Poprawny JSON | Średni czas [ms] |
|-------|---------|----------------|-------------------|-------------|---------------|------------------|
| gemma3:4b | zero-shot | 0% | 0% | 0% | 0/10 | 4003 |
| gemma3:4b | one-shot | 0% | 0% | 0% | 0/10 | 3911 |
| gemma3:4b | few-shot | 0% | 0% | 0% | 0/10 | 5249 |
| qwen3:4b | zero-shot | 100% | 100% | 100% | 10/10 | 52014 |
| qwen3:4b | few-shot | 100% | 100% | 100% | 10/10 | 75256 |
| llama3.2:3b | zero-shot | 50% | 40% | 20% | 8/10 | 1679 |
| llama3.2:3b | one-shot | 90% | 60% | 50% | 10/10 | 1102 |
| llama3.2:3b | few-shot | 80% | 60% | 40% | 10/10 | 1206 |

**Wnioski:**
Jeden przykład w prompcie pomaga, o ile model w ogóle zwraca poprawny JSON. Dokładanie kolejnych przykładów nie musi już poprawiać wyniku.
- **llama3.2:3b** — tu prompting działa. Zero-shot: 8/10 JSON i 20% exact match. Już **one-shot** domyka format (10/10 JSON) i podnosi piętro z 50% do 90%, windę z 40% do 60%, exact match z 20% do **50%**. Few-shot nie daje dalszego wzrostu.
- **gemma3:4b** — zero-shot, one-shot i few-shot kończą na 0/10 JSON. Przykłady nie naprawiają formatu wyjścia; Gemma bez JSON Schema pozostaje nieczytelna dla parsera niezależnie od liczby strzałów.
- **qwen3:4b** — zero-shot już daje 100%, więc few-shot nic nie dodaje do jakości, tylko wydłuża prompt i czas z 52 s do 75 s.

## EXP-03 — Temperature

### gemma3:4b

| Temperature | Exact match | Poprawny JSON | Średni czas [ms] |
|-------------|-------------|---------------|------------------|
| 0.0 | 0% | 0/50 | 1904 |
| 0.5 | 0% | 0/50 | 1838 |
| 1.0 | 0% | 0/50 | 1833 |

### llama3.2:3b

| Temperature | Exact match | Floor accuracy | Elevator accuracy | Poprawny JSON | Średni czas [ms] |
|-------------|-------------|----------------|-------------------|---------------|------------------|
| 0.0 | 20% | 50% | 40% | 40/50 | 927 |
| 0.5 | 24% | 54% | 38% | 39/50 | 826 |
| 1.0 | 32% | 66% | 36% | 42/50 | 789 |

**Wnioski:**

Dla Gemmy temperatura nic nie zmienia: przy zerze poprawnego JSON metryki trafności pozostają zerowe niezależnie od `0.0` / `0.5` / `1.0`. Problem jest w formacie.

Dla Llamy wyższa temperatura nie pogorszyła exact match na tej małej próbie.

## EXP-04 — Structured output — JSON i walidacja

### llama3.2:3b

| Wariant | Poprawny JSON | Floor accuracy | Elevator accuracy | Exact match | Średni czas [ms] |
|---------|---------------|----------------|-------------------|-------------|------------------|
| prompt | 8/10 | 50% | 40% | 20% | 1160 |
| json-schema | 10/10 | 70% | 50% | 30% | 1001 |

### gemma3:4b

| Wariant | Poprawny JSON | Floor accuracy | Elevator accuracy | Exact match | Średni czas [ms] |
|---------|---------------|----------------|-------------------|-------------|------------------|
| prompt | 0/10 | 0% | 0% | 0% | 3355 |
| json-schema | 10/10 | 80% | 70% | 60% | 2681 |

**Wnioski:**

Poprawny składniowo JSON nie oznacza poprawnej odpowiedzi. Llama przy `json-schema` ma 10/10 JSON, ale tylko 30% exact match — model zwraca poprawny obiekt, który merytorycznie jest zły.

JSON Schema jest jednak konieczne, żeby w ogóle dało się odpowiedź przetworzyć:

- Gemma: `0/10 → 10/10` JSON i `0% → 60%`
- Llama: `8/10 → 10/10` JSON, exact match tylko `20% → 30%`. Schema słabo poprawia rozumienie.

## Najtrudniejsze notatki

- **NOTE-007** — „4 piętro, winda tylko do 3”. Oczekiwane: `floor=4`, `elevator=true`. Winda istnieje, nawet jeśli nie dojeżdża na docelowe piętro. Łatwo zwrócić `false`.
- **NOTE-010** — „Winda nie działa, dostawa na 5p”. Oczekiwane: `floor=5`, `elevator=true`. Badamy obecność windy, nie sprawność. „Nie działa” brzmi jak brak.
- **NOTE-009** — „Drugie lub trzecie piętro”. Oczekiwane: `floor=null`. Model ma tendencję zgadywać jedno z pięter zamiast przyznać niejednoznaczność.
- **NOTE-004** — „IV piętro, lokal 12”. Cyfry rzymskie + brak informacji o windzie (`elevator=null`).
- **NOTE-003** — „Parter…” ma być `floor=0`, nie `null` i nie `1`.
- **NOTE-006** — „Mieszkanie 15, domofon 15”: oba pola `null`; numer mieszkania to nie piętro.

## Rekomendacja
Wybieramy **`gemma3:4b`
