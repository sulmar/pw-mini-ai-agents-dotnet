# Raport — Laboratorium 03

**Zespół:**  Ewelina Karpińska, Maciej Mazurek
**Data:**  16.10-21.10

## Środowisko

| Parametr | Wartość |
|----------|---------|
| Wersja Ollama | Ollama 0.40.9 |
| Sprzęt (CPU/GPU, RAM) | CPU: ARM 3.42GHz, RAM: 32GB, GPU: 128MB |
| Liczba dostępnych notatek | 10 |

## EXP-00 — Sposób oceny wyników

Jak oceniamy poprawność odpowiedzi modelu? Jakie metryki przyjmujemy i dlaczego?

| Metryka | Definicja |
|---------|-----------|
| Floor accuracy | Odsetek notatek, w których model poprawnie wywnioskował piętro docelowe (lub poprawnie zwrócił null, gdy brak było jednoznacznej informacji). |
| Elevator accuracy | Odsetek notatek, w których model trafnie określił istnienie windy (true, false lub null). |
| Exact match | Odsetek, gdzie piętro oraz winda są jednocześnie poprawnie określone. |
| Średni czas [ms] | średni czas oczekiwania na odpowiedź w ms. |
| Poprawny JSON | Poprawna struktura JSON. |
| Powtarzalne notatki | Powtarzalność tych samych odpowiedzi przy tych samych notatkach. |


## EXP-01 — Porównanie modeli

| Model | Floor accuracy | Elevator accuracy | Exact match | Średni czas [ms] | Poprawny JSON | 
|---|---|---|---|---|
| llama3.2:1b | 0.5 | 0.3 | 0.2 | 1664 | 1 |
| qwen2.5:1.5b | 0.7 | 0.3 | 0.2 | 1324 | 0.0 |
| gemma2:2b | 0.8 | 0.7 | 0.6 | 4010 | 0.0 |


**Exact match** oznacza odsetek notatek, dla których model poprawnie rozpoznał jednocześnie oba pola: `floor` i `elevator`.

**Wnioski:**
JSON nie jest poprawny z powodu dopisku ```json przy kazdej odpowiedzi w qwen oraz gemma. Poza tym gemma najlepiej wypadła.

## EXP-02 — Prompting

| Model | Wariant | Floor accuracy | Elevator accuracy | Exact match | Średni czas [ms] | Poprawny JSON |
|---|---|---|---|---|---|
| llama3.2:1b | zero-shot | 0.5 | 0.3 | 0.2 | 940 | 1 |
| llama3.2:1b | one-shot | 0.5 | 0.1 | 0.1 | 859 | 0.8 |
| llama3.2:1b | few-shot | 0.7 | 0.2 | 0.2 | 970 | 0.9 |

| Model | Wariant | Floor accuracy | Elevator accuracy | Exact match | Średni czas [ms] | Poprawny JSON |
|---|---|---|---|---|---|
| qwen2.5:1.5b | zero-shot | 0.7 | 0.3 | 0.2 | 1279 | 0.0 |
| qwen2.5:1.5b | one-shot | 0.6 | 0.4 | 0.1 | 889 | 0.0 |
| qwen2.5:1.5b | few-shot | 0.7 | 0.4 | 0.1 | 985 | 0.0 |

| Model | Wariant | Floor accuracy | Elevator accuracy | Exact match | Średni czas [ms] | Poprawny JSON |
|---|---|---|---|---|---|
| gemma2:2b | zero-shot | 0.8 | 0.7 | 0.6 | 4315 | 0.0 |
| gemma2:2b | one-shot | 0.9 | 0.7 | 0.6 | 4676 | 0.0 |
| gemma2:2b | few-shot | 0.9 | 0.9 | 0.8 | 6085 | 0.0 |

**Wnioski:**
Few-shot wypada najlepiej w każdym prawie aspekcie.

## EXP-03 — Temperature

llama3.2:1b
| Temperature | Exact match | Powtarzalne notatki |
|---|---|---|
| 0.0 | 0.2 |  (10/10), valid JSON: 50/50 |
| 0.5 | 0.36 |  0/10 (duża wariancja), valid JSON: 50/50 |
| 1.0 | 0.26 |  0/10 (wysoka wariancja) valid JSON: 43–50/50 |

**Wnioski:**
Mimo iż exact match jest częstsze przy wyższej temperaturze, to wtedy nie ma żadnej stabilności.

## EXP-04 — Structured output — JSON i walidacja

llama3.2:1b daje poprawny json więc nie robi różnicy żadnej.

qwen2.5:1.5b 
| Wariant | Poprawny JSON | Exact match |
|---|---|---|
| prompt | 0/10 | 0.2 |
| json-schema | 10/10  | 0.2 |

gemma2:2b
| Wariant | Poprawny JSON | Exact match |
|---|---|---|
| prompt | 0/10 | 0.6 |
| json-schema | 10/10  | 0.6 |

**Wnioski:**
prompt nic nie zmienił, ale json-schema kompletnie naprawił problem w obu modelach.

## Najtrudniejsze notatki
"Drugie lub trzeciez piętro" - wybierają jedno z nich zamiast dać nulla.
"Winda nie działa" - tutaj dawały false zamiast true. Model nie rozpoznaje stanu technicznego od istnienia windy.


## Rekomendacja

Najlepszy mały jest oczywiście gemma2:2b, ale trzeba pamiętać o json-schema. Few-shot oraz temperatura 0.0 dla stabilności.