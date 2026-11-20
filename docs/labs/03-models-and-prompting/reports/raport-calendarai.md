# Raport — Laboratorium 03: Modele i prompting

**Zespół:** nie podano

**Data:** 23.10.2026
**Źródła:** `readme.md`, `reports/template.md` i `results/results.csv` z przekazanego katalogu laboratorium.

## Zakres i ograniczenia danych

W pliku `data/notes.csv` znajdują się 172 notatki, mimo że szablon raportu podaje 170. Eksperymenty zapisane w `results.csv` były jednak oceniane na zestawie 10 etykietowanych przykładów (`total = 10`; dla EXP-03: 10 przykładów × 5 powtórzeń = 50 odpowiedzi). Wnioski o jakości należy więc traktować jako wstępne.

Wyniki nie zawierają wersji Ollama, konfiguracji sprzętowej, odpowiedzi dla pojedynczych notatek ani liczby stabilnych notatek z EXP-03. Tych danych nie można rzetelnie odtworzyć z CSV.

## Środowisko

| Parametr | Wartość                      |
|----------|------------------------------|
| Wersja Ollama | najnowsza    |
| Sprzęt (CPU/GPU, RAM) | Apple M3 / 16GB RAM          |
| Liczba notatek w `data/notes.csv` | 172                          |
| Zestaw oceny | 10 etykietowanych przykładów |

## EXP-00 — Sposób oceny wyników

| Metryka | Definicja |
|---------|-----------|
| Floor accuracy | Udział odpowiedzi z poprawnie rozpoznanym `floor`, także gdy poprawną wartością jest `null`. |
| Elevator accuracy | Udział odpowiedzi z poprawnie rozpoznanym `elevator`, także dla `null`. |
| Exact match | Udział notatek, dla których oba pola — `floor` i `elevator` — są poprawne jednocześnie. |
| Poprawny JSON | Udział odpowiedzi, które parser zaakceptował jako prawidłowy JSON zgodny z oczekiwanym formatem. |
| Średni czas | Średni czas odpowiedzi modelu w milisekundach. |

Najważniejszą miarą do wyboru rozwiązania jest **exact match**: agent potrzebuje obu danych w jednej, programowo użytecznej odpowiedzi. Dokładności pól osobno pokazują natomiast, gdzie model traci jakość.

## EXP-01 — Porównanie modeli

Poniżej uśredniono powtórzone wiersze tego samego modelu z `results.csv`; metryki jakościowe llama3.2 i qwen2.5 były identyczne między ich powtórzeniami. W nawiasie podano liczbę zapisanych uruchomień.

| Model | Floor accuracy | Elevator accuracy | Exact match | Średni czas [ms] | Poprawny JSON |
|-------|----------------|-------------------|-------------|------------------|---------------|
| llama3.2:3b (n=3) | 60% | 50% | 30% | 663,0 | 90% |
| qwen2.5:3b (n=2) | 60% | 60% | 40% | 766,5 | 100% |
| deepseek-r1:7b (n=1) | 0% | 0% | 0% | 13 732,0 | 0% |

**Wnioski:** qwen2.5:3b uzyskał o 10 p.p. wyższy exact match i elevator accuracy niż llama3.2:3b, przy pełnej poprawności JSON. Jest przeciętnie o ok. 104 ms wolniejszy. Zapis dla deepseek-r1:7b wskazuje na problem z formatem odpowiedzi: nie został zaakceptowany żaden JSON, a średni czas był ponad 17 razy dłuższy niż dla qwen2.5:3b. Nie należy wybierać go bez naprawy promptu/parsing-u i ponownego testu.

## EXP-02 — Prompting

CSV zawiera wyniki dla dwóch modeli, dlatego zestawienie rozdzielono według modelu. Wariant **one-shot** nie został wykonany: w kodzie eksperymentu pozostawiono dla niego `TODO`, a w CSV nie ma odpowiedniego wiersza.

| Model | Wariant | Floor accuracy | Elevator accuracy | Exact match | Średni czas [ms] |
|-------|---------|----------------|-------------------|-------------|------------------|
| qwen2.5:3b | zero-shot | 60% | 60% | 40% | 496 |
| qwen2.5:3b | one-shot | — | — | — | — |
| qwen2.5:3b | few-shot | 60% | 80% | 50% | 528 |
| llama3.2:3b | zero-shot | 60% | 50% | 30% | 521 |
| llama3.2:3b | one-shot | — | — | — | — |
| llama3.2:3b | few-shot | 80% | 60% | 40% | 508 |

**Wnioski:** few-shot poprawił exact match dla obu modeli: o 10 p.p. dla qwen2.5:3b oraz llama3.2:3b. Najlepszy pojedynczy wynik osiągnął qwen2.5:3b z few-shot — 50% exact match i 80% elevator accuracy; koszt czasowy względem zero-shot wyniósł 32 ms. Brak one-shot uniemożliwia pełne porównanie trzech metod promptowania.

## EXP-03 — Temperature

| Model | Temperature | Exact match | Powtarzalne notatki | Średni czas [ms] |
|-------|-------------|-------------|---------------------|------------------|
| llama3.2:3b | 0,0 | 30% | Brak w CSV | 454 |
| llama3.2:3b | 0,5 | 24% | Brak w CSV | 436 |
| llama3.2:3b | 1,0 | 36% | Brak w CSV | 418 |
| qwen2.5:3b | 0,0 | 40% | Brak w CSV | 520 |
| qwen2.5:3b | 0,5 | 38% | Brak w CSV | 624 |
| qwen2.5:3b | 1,0 | 30% | Brak w CSV | 848 |

**Wnioski:** dla qwen2.5:3b najlepszy exact match dała temperatura 0,0; wyższa temperatura pogorszyła jakość i zwiększyła czas, szczególnie przy 1,0. Dla llama3.2:3b najwyższy wynik zanotowano przy 1,0, lecz różnica wynosi tylko 6 p.p. na małej próbie. Kod eksperymentu oblicza liczbę powtarzalnych notatek, ale nie zapisuje jej do `results.csv`, więc nie da się potwierdzić powtarzalności na podstawie dostarczonych danych.

## EXP-04 — Structured output — JSON i walidacja

| Model | Wariant | Poprawny JSON | Exact match | Średni czas [ms] |
|-------|---------|---------------|-------------|------------------|
| qwen2.5:3b | prompt | 100% (10/10) | 40% | 572 |
| qwen2.5:3b | json-schema | 100% (10/10) | 40% | 555 |
| llama3.2:3b | prompt | 90% (9/10) | 30% | 519 |
| llama3.2:3b | json-schema | 100% (10/10) | 30% | 506 |

**Wnioski:** JSON Schema nie poprawił exact match, lecz dla llama3.2:3b podniósł poprawność JSON z 90% do 100% i skrócił średni czas o 13 ms. Dla qwen2.5:3b oba warianty dały 100% poprawnego JSON; schema była o 17 ms szybsza. W integracji programistycznej warto stosować JSON Schema, ponieważ ogranicza ryzyko błędu parsowania bez pogorszenia jakości w tych pomiarach.

## Najtrudniejsze notatki

`results.csv` przechowuje wyłącznie metryki zbiorcze, więc nie wskazuje identyfikatorów notatek ani błędnych odpowiedzi. Nie można zatem rzetelnie określić, które konkretne notatki były najczęściej błędnie interpretowane. Z definicji zadania szczególnej kontroli po dodaniu logowania per notatka wymagają:

- notatki z wieloma piętrami lub niejednoznacznym piętrem docelowym;
- notatki, gdzie występuje liczba będąca kodem, numerem lokalu albo godziną;
- rozróżnienie „brak windy”, „winda jest, ale nie działa” i brak informacji o windzie.

Warto dopisać do wyników identyfikator notatki, odpowiedź modelu i oczekiwaną etykietę. Pozwoli to przekuć tę listę hipotez w mierzalną analizę błędów.

## Rekomendacja

Do dalszego prototypu rekomendowany jest **qwen2.5:3b z promptem few-shot, temperaturą 0,0 i JSON Schema**. Najmocniejsze bezpośrednie dowody to: najlepszy zaobserwowany exact match (50%) i elevator accuracy (80%) dla few-shot oraz 100% poprawnego JSON w teście structured output. Temperaturę 0,0 wybrano, ponieważ była najlepsza dla qwen2.5:3b w EXP-03 (40% exact match) i najkrótsza dla niego wśród badanych temperatur.

Ta rekomendacja wymaga walidacji w jednym, połączonym eksperymencie: w CSV nie ma jeszcze pomiaru dokładnie konfiguracji **qwen2.5:3b + few-shot + temperatura 0,0 + JSON Schema**. Przed wdrożeniem należy też uruchomić one-shot, zapisać wyniki per notatka i poszerzyć zestaw etykietowany poza 10 przykładów.
