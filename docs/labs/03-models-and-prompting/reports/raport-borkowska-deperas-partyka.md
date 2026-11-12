# Raport — Laboratorium 03

**Zespół:** MealBrAIn (Patrycja Borkowska, Maria Deperas, Oliwia Partyka) 

**Data:** 22.10.2026

## Środowisko

| Parametr | K1 | K2 | K3 |
|----------|----|----|----|
| Osoba | Patrycja| Maria | Oliwia |
| CPU | Intel Core Ultra 9 275HX, 2,70 GHz| Intel Core i7-1360P | AMD Ryzen 7 8840HS | 
| GPU |NVIDIA GeForce RTX 5070 Ti Laptop GPU — 12 GB VRAM | Intel Iris Xe Graphics (zintegrowane) | AMD Radeon 780M |  
| RAM | 64 GB| 15,6 GB | 16 GB | 
| Model A | gemma3:4b | smollm2:1.7b | phi4-mini |
| Model B | qwen3:4b| granite3.3:2b | llama3.2 |

Model A i model B oznaczają dwa modele badane na danym komputerze, zgodnie z tabelą powyżej.

Wszystkie komputery korzystają z tego samego zbioru testowego, promptów i ustawień eksperymentów. Czasy odpowiedzi porównujemy przede wszystkim w obrębie jednego komputera; porównanie między komputerami uwzględnia różnice sprzętowe.

| Zbiór danych | Liczba notatek |
|-------------|----------------|
| Zbiór testowy używany w eksperymentach (examples.csv) | 10 |

## EXP-00 — Sposób oceny wyników

Jak oceniamy poprawność odpowiedzi modelu? Jakie metryki przyjmujemy i dlaczego?

| Metryka | Definicja |
|---------|-----------|
| **Floor accuracy** | Trafność rozpoznania numeru piętra. |
| **Elevator accuracy** | Trafność rozpoznania obecności windy. |
| **Exact match** | Odsetek notatek z poprawnym piętrem i windą jednocześnie. |

## EXP-01 — Porównanie modeli

### Komputer K1

| Model | Floor accuracy | Elevator accuracy | Exact match | Średni czas [ms] |
|-------|----------------|-------------------|-------------|------------------|
| gemma3:4b |90% | 80%| 70%| 2309|
| qwen3:4b | 100% | 100% |100% | 111451|

### Komputer K2

| Model | Floor accuracy | Elevator accuracy | Exact match | Średni czas [ms] |
|-------|----------------|-------------------|-------------|------------------|
| smollm2:1.7b | 60% | 30% | 20% | 2187 |
| granite3.3:2b | 60% | 50% | 50% | 1691 |

### Komputer K3

| Model | Floor accuracy | Elevator accuracy | Exact match | Średni czas [ms] |
|-------|----------------|-------------------|-------------|------------------|
| phi4-mini | 80% | 90% | 70% | 2168 |
| llama3.2 | 80% | 50% | 40% | 1715 |

**Exact match** oznacza odsetek notatek, dla których model poprawnie rozpoznał jednocześnie oba pola: floor i elevator.

**Wnioski (K1, K2, K3 i porównanie):**

K2: granite3.3:2b wypadł lepiej niż smollm2:1.7b pod każdym względem: ten sam floor accuracy (60%), ale wyższy elevator accuracy (50% do 30%) i exact match (50% do 20%), przy krótszym czasie (1691 ms do 2187 ms). Oba modele zwróciły poprawny JSON w 10/10 przypadków, więc różnice wynikają z interpretacji notatek, nie z formatu. Oba modele dopowiadały windę (`true`), gdy notatka o niej nie wspominała (NOTE-004, 006, 009), i mylnie traktowały „winda nie działa” jako brak windy (NOTE-010).

K3: Na badanym zbiorze 10 notatek model phi4-mini uzyskał lepsze wyniki niż llama3.2. Oba modele poprawnie rozpoznały piętro w 80% przypadków, ale phi4-mini znacznie lepiej rozpoznawał informację o windzie — 90% wobec 50%. Dzięki temu poprawnie określił oba pola jednocześnie w 70% notatek, a llama3.2 w 40%. Phi4-mini odpowiadał średnio o 453 ms wolniej, jednak większa poprawność uzasadnia wybór tego modelu do dalszych eksperymentów EXP-02–04. Wyniki dotyczą niewielkiego zbioru testowego.

## EXP-02 — Prompting

### Komputer K1

| Wariant | Floor accuracy | Elevator accuracy | Exact match |
|---------|----------------|-------------------|-------------|
| zero-shot | 100% | 100% | 100% |
| one-shot | 100% | 100% | 100% |
| few-shot | 100% | 100% | 100% |

### Komputer K2

| Wariant | Floor accuracy | Elevator accuracy | Exact match |
|---------|----------------|-------------------|-------------|
| zero-shot | 60% | 50% | 50% |
| one-shot | 80% | 70% | 60% |
| few-shot | 60% | 80% | 40% |

### Komputer K3

Model: phi4-mini

| Wariant | Floor accuracy | Elevator accuracy | Exact match |
|---------|----------------|-------------------|-------------|
| zero-shot | 80% | 90% | 70% |
| one-shot | 80% | 80% | 60% |
| few-shot | 80% | 60% | 40% |

**Wnioski (K1, K2, K3 i porównanie):**

K2 (granite3.3:2b): najlepszy exact match dał one-shot (60%), a zero-shot (50%) i few-shot (40%) wypadły gorzej. Jeden przykład naprawił NOTE-005 (model przestał zgadywać piętro), natomiast few-shot popsuł wcześniej poprawne NOTE-003 i NOTE-007, choć dał najwyższy elevator accuracy (80%). Więcej przykładów nie znaczy więc lepszych wyników. Żaden wariant nie rozwiązał NOTE-004, 006 i 010.

K3: W badanym zbiorze najlepszy wynik uzyskał wariant zero-shot — Exact match wyniósł 70%. Dodanie jednego przykładu obniżyło wynik do 60%, a kilku przykładów do 40%. Poprawność rozpoznawania piętra pozostała na poziomie 80%, natomiast poprawność informacji o windzie spadała wraz z dodawaniem przykładów.
Wariant few-shot poprawił interpretację notatki „Mieszkanie 15, domofon 15” — model przestał traktować numer mieszkania jako piętro. Jednocześnie pojawiły się nowe błędy: model pomijał wyraźne informacje o windzie oraz pomylił piętro dostawy z piętrem, do którego dojeżdża winda. Dodanie przykładów nie gwarantuje więc poprawy; znaczenie ma ich dobór.

## EXP-03 — Temperature

### Komputer K1

| Temperature | Exact match | Powtarzalne notatki |
|-------------|-------------|---------------------|
| 0.0 | 100%| 10/10 |
| 0.5 |  100%| 10/10 |
| 1.0 |   100%| 10/10 |
### Komputer K2

| Temperature | Exact match | Powtarzalne notatki |
|-------------|-------------|---------------------|
| 0.0 | 50% | 10/10 |
| 0.5 | 50% | 7/10 |
| 1.0 | 52% | 7/10 |

### Komputer K3

Model: phi4-mini

| Temperature | Exact match | Powtarzalne notatki |
|-------------|-------------|---------------------|
| 0.0 | 70% | 10/10 |
| 0.5 | 68% | 8/10 |
| 1.0 | 64% | 7/10 |

Powtarzalne notatki to notatki z takim samym wynikiem we wszystkich pięciu powtórzeniach dla danej temperatury. Liczbę przepisujemy z terminala; nie jest zapisywana w results.csv. Powtarzalna odpowiedź może być błędna.

**Wnioski (K1, K2, K3 i porównanie):**

K2 (granite3.3:2b): przy temperaturze 0.0 model jest w pełni powtarzalny (10/10), a exact match prawie się nie zmienia (50% → 50% → 52%), więc wyższa temperatura nie poprawia jakości. Przy 0.5 i 1.0 pojawia się niestabilność na niepewnych notatkach (NOTE-005, 009) oraz błędny JSON (46/50 przy 1.0). Powtarzalna odpowiedź może być błędna (NOTE-004, 006, 010). Dla ekstrakcji danych wybieramy 0.0.

K3: W badaniu modelu phi4-mini wzrost temperatury zmniejszał powtarzalność odpowiedzi oraz ich poprawność.Do ekstrakcji danych wybieramy temperaturę 0, która w tym eksperymencie zapewniła najwyższą poprawność i powtarzalność.
Powtarzalność nie gwarantuje jednak poprawności. Przy temperaturze 0 model konsekwentnie uznawał numer mieszkania 15 za piętro, wybierał piętro 2 z niejednoznacznej informacji „drugie lub trzecie” oraz traktował niedziałającą windę jako jej brak.

## EXP-04 — Structured output — JSON i walidacja

### Komputer K1

| Wariant | Poprawny JSON | Exact match |
|---------|---------------|-------------|
| prompt | 10/10| 100%|
| json-schema | 10/10| 100%|

### Komputer K2

| Wariant | Poprawny JSON | Exact match |
|---------|---------------|-------------|
| prompt | 10/10 | 50% |
| json-schema | 10/10 | 50% |

### Komputer K3

Model: phi4-mini

| Wariant | Poprawny JSON | Exact match |
|---------|---------------|-------------|
| prompt | 10/10 | 70% |
| json-schema | 10/10 | 60% |

**Wnioski (K1, K2, K3 i porównanie):**

K2 (granite3.3:2b): oba warianty dały poprawny JSON w 10/10 przypadków i identyczny exact match (50%), a json-schema był nieco szybszy (1118 ms do 1425 ms). Poprawny składniowo JSON nie oznacza poprawnej odpowiedzi: połowa wyników jest błędna (NOTE-004, 005, 006, 009, 010), mimo że każdy da się przetworzyć programowo. Schemat gwarantuje format, ale nie zrozumienie notatki.

K3: Oba warianty zwróciły poprawny JSON w 10/10 przypadków, jednak prompt uzyskał wyższy Exact match: 70% wobec 60%. Wariant json-schema był szybszy, ale nie poprawił trafności odpowiedzi. Poprawny format JSON nie gwarantuje poprawności zawartych w nim danych.

## Najtrudniejsze notatki

Które notatki model najczęściej interpretował błędnie i dlaczego?

K2: Najczęściej błędnie interpretowane były cztery notatki. NOTE-004 („IV piętro, lokal 12”), NOTE-006 („Mieszkanie 15, domofon 15”), NOTE-009 („Drugie lub trzecie piętro”) i NOTE-010 („Winda nie działa, dostawa na 5p”) zostały błędnie rozpoznane we wszystkich 21 odpowiedziach modelu granite3.3:2b, licząc warianty i powtórzenia eksperymentów.
W NOTE-004 model zawsze zwracał `floor=12`, biorąc numer lokalu za piętro i nie rozpoznając rzymskiej cyfry IV. W NOTE-006 zawsze zwracał `floor=15`, uznając numer mieszkania za piętro. W NOTE-009 wybierał jedną z opcji (`floor=2`) albo zwracał niepoprawny JSON, zamiast `null`. W NOTE-010 zawsze zwracał `elevator: false`, myląc niesprawność windy z jej brakiem.
Problematyczna była też NOTE-005 („Winda na końcu korytarza”): model zgadywał piętro (`floor=0`) i pomijał windę, błąd wystąpił w 16 z 21 odpowiedzi; poprawny wynik pojawiał się głównie w wariantach one-shot i few-shot oraz sporadycznie przy wyższej temperaturze. Rzadziej błędy pojawiały się przy NOTE-007 („4 piętro, winda tylko do 3”), gdzie few-shot pomylił piętro docelowe z zasięgiem windy, oraz przy NOTE-003 („Parter, wejście od podwórza”), gdzie few-shot dopowiedział windę. Główne trudności dotyczyły więc odróżniania numeru lokalu od piętra, niejednoznaczności i niedopowiadania brakujących danych.

K3: Najczęściej błędnie interpretowane były trzy notatki. W NOTE-009 („Drugie lub trzecie piętro”) model wybierał piętro 2, zamiast zwrócić `null`, ponieważ nie można jednoznacznie ustalić piętra dostawy. W NOTE-010 („Winda nie działa, dostawa na 5p”) zwracał `elevator: false` lub `null`, myląc niesprawność windy z jej brakiem. Obie notatki zostały błędnie rozpoznane we wszystkich 21 odpowiedziach modelu phi4-mini, licząc warianty i powtórzenia eksperymentów.
Problematyczna była również NOTE-006 („Mieszkanie 15, domofon 15”). Model uznawał numer mieszkania za piętro, mimo że notatka nie zawierała informacji o piętrze. Błąd wystąpił w 20 z 21 odpowiedzi; poprawny wynik uzyskano jedynie w wariancie few-shot.
Rzadziej pojawiały się błędy przy NOTE-007 („4 piętro, winda tylko do 3”), gdzie model w wariancie few-shot pomylił piętro docelowe z zasięgiem windy, oraz przy NOTE-003 („Parter, wejście od podwórza”), gdzie dopowiadał informację o windzie. Główne trudności dotyczyły więc rozróżniania znaczenia liczb, interpretowania niejednoznaczności i niedopowiadania brakujących danych.
