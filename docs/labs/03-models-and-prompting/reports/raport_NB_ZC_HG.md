# Raport — Laboratorium 03

**Zespół:** Natalia Bakalarska, Zofia Cymerys, Helena Gala  
**Data:** 28.10.2026

## Środowisko

| Parametr | Wartość |
| :---- | :---- |
| Wersja Ollama | 0.40.12 |
| Sprzęt (CPU/GPU, RAM) | Intel Core Ultra 9 275HX, 64 GB RAM, NVIDIA GeForce RTX 5070 Ti Laptop GPU (12 GB)  |
| Liczba dostępnych notatek | 170 |

## EXP-00 — Sposób oceny wyników

Jak oceniamy poprawność odpowiedzi modelu? Jakie metryki przyjmujemy i dlaczego?

| Metryka | Definicja |
| :---- | :---- |
| Floor accuracy | Odsetek poprawnie rozpoznanych pięter  |
| Elevator accuracy | Odsetek poprawnie rozpoznanych informacji o windzie |
| Exact match | Odsetek notatek, dla których pola dla pięter i wind są poprawne |
| Średni czas \[ms\] | uśredniony czas oczekiwania na odpowiedź |
| Powtarzalne notatki | ilość odpowiedzi w których został uzyskany ten sam wynik po powtórzeniu zapytania |
| Poprawny json | ilość odpowiedzi w których został uzyskany json o poprawnej strukturze |

## EXP-01 — Porównanie modeli

| Model | Floor accuracy | Elevator accuracy | Exact match | Średni czas \[ms\]  (\*uśredniony dodatkowo między wywołaniami) |
| :---- | :---- | :---- | :---- | :---- |
| gemma3:4b | 0.9 | 0.8 | 0.7 | 940\* |
| mistral | 0.7 | 0.7 | 0.5 | 12165\* |
| qwen3:4b | 1 | 1 | 1 | 21224\* |

**Exact match** oznacza odsetek notatek, dla których model poprawnie rozpoznał jednocześnie oba pola: `floor` i `elevator`.  
**Wnioski:**

- **Qwen3:4b** osiągnął bezbłędny wynik (1.0 we wszystkich metrykach jakościowych), wykazując bezbłędną interpretację języka polskiego i kontekstu logistycznego. Odbywa się to jednak kosztem bardzo długiego czasu wnioskowania (ponad 21 s na zapytanie).  
- **Gemma3:4b** deklasuje konkurencję pod względem wydajności – czas na poziomie 940 ms pozwala na zastosowania interaktywne przy zachowaniu przyzwoitej trafności bazowej (Exact match \= 0.7).  
- **Mistral** wypadł najsłabiej pod każdym względem (Exact match 0.5 przy czasie ponad 12 s), zmagając się z polską fleksją oraz subtelnościami językowymi w notatkach.

## EXP-02 — Prompting

#### **gemma3:4b**

| Wariant | Floor accuracy | Elevator accuracy | Exact match |
| :---- | :---- | :---- | :---- |
| zero-shot | 0.9 | 0.8 | 0.7 |
| one-shot | 1 | 0.9 | 0.9 |
| few-shot | 0.8 | 0.9 | 0.7 |

#### **mistral**

| Wariant | Floor accuracy | Elevator accuracy | Exact match |
| :---- | :---- | :---- | :---- |
| zero-shot | 0.7 | 0.7 | 0.5 |
| one-shot | 0.7 | 0.9 | 0.7 |
| few-shot | 0.8 | 0.8 | 0.6 |

#### **qwen3:4b**

| Wariant | Floor accuracy | Elevator accuracy | Exact match |
| :---- | :---- | :---- | :---- |
| zero-shot | 1 | 1 | 1 |
| one-shot | 1 | 1 | 1 |
| few-shot | 1 | 1 | 1 |

**Wnioski:**

- Dodanie pojedynczego przykładu (**one-shot**) przyniosło wyraźną poprawę w modelach o niższej skuteczności bazowej: dla `gemma3:4b` Exact match wzrósł z 0.7 do 0.9, a dla `mistral` z 0.5 do 0.7. Pojedynczy przykład skutecznie doprecyzował schemat odpowiedzi i sposób interpretacji skrótów.   
- Wariant **few-shot** spowodował w modelach `gemma3:4b` i `mistral` nieznaczny spadek metryki Exact match względem one-shot. Wprowadzenie zbyt wielu przykładów w oknie kontekstowym modeli o mniejszej skali może prowadzić do nadmiernego skupienia na wzorcach z promptu (overfitting kontekstowy) kosztem analizowanej notatki.   
- **Qwen3:4b** okazał się całkowicie odporny na zmiany strategii promptowania, osiągając maksymalny wynik 1.0 już w trybie zero-shot, co świadczy o jego znakomitym wstępnym dostrojeniu instrukcyjnym do ekstrakcji encji. 

## EXP-03 — Temperature

#### **gemma3:4b**

| Temperature | Exact match | Powtarzalne notatki |
| :---- | :---- | :---- |
| 0.0 | 0.7 | 10/10 |
| 0.25 | 0.7 | 10/10 |
| 0.5 | 0.7 | 10/10 |
| 0.75 | 0.72 | 9/10 |
| 1.0 | 0.68 | 10/10 |

#### **mistral**

| Temperature | Exact match | Powtarzalne notatki |
| :---- | :---- | :---- |
| 0.0 | 0.5 | 10/10 |
| 0.25 | 0.46 | 8/10 |
| 0.5 | 0.36 | 2/10 |
| 0.75 | 0.44 | 1/10 |
| 1.0 | 0.3 | 3/10 |

#### **qwen3:4b**

| Temperature | Exact match | Powtarzalne notatki |
| :---- | :---- | :---- |
| 0.0 | 1 | 10/10 |
| 0.25 | 1 | 10/10 |
| 0.5 | 1 | 10/10 |
| 0.75 | 1 | 10/10 |
| 1.0 | 1 | 10/10 |

**Wnioski:**

- Wzrost temperatury w zadaniach ekstrakcji ustrukturyzowanej jest zjawiskiem niepożądanym. W przypadku modelu `mistral` wzrost parametru do wartości 0.5–1.0 doprowadził do drastycznego spadku powtarzalności (do poziomu 1–3/10) oraz spadku Exact match do zaledwie 0.30.   
- gemma3:4b wykazuje dużą stabilność i odporność na wahania temperatury, utrzymując wysoką powtarzalność nawet przy $T=1.0$.   
- `qwen3:4b` zachował 100% poprawności i powtarzalności w całym badaniu, co wskazuje na silną dominację najbardziej prawdopodobnych tokenów wyjściowych w rozkładzie prawdopodobieństwa dla tego schematu danych.   
- Do celów produkcyjnych w zadaniach ekstrakcji do JSON optymalną nastawą pozostaje **Temperature \= 0.0**, gwarantująca pełną deterministyczność generacji. 

## EXP-04 — Structured output — JSON i walidacja

#### **gemma3:4b**

| Wariant | Poprawny JSON | Exact match |
| :---- | :---- | :---- |
| prompt | 10/10 | 0.7 |
| json-schema | 10/10 | 0.7 |

#### **mistral**

| Wariant | Poprawny JSON | Exact match |
| :---- | :---- | :---- |
| prompt | 10/10 | 0.7 |
| json-schema | 10/10 | 0.5 |

#### **qwen3:4b**

| Wariant | Poprawny JSON | Exact match |
| :---- | :---- | :---- |
| prompt | 10/10 | 1 |
| json-schema | 10/10 | 1 |

**Wnioski:**

- Wszystkie testowane modele przy użyciu środowiska Ollama poprawnie generowały składnię JSON (10/10) w obu podejściach, co oznacza, że parser nie napotykał błędów składniowych ani niechcianych znaczników Markdown.   
- Wymuszenie schematu (`json-schema`) na poziomie silnika inferencji gwarantuje obecność i typy wymaganych pól. W przypadku modelu `mistral` wariant ten ujawnił jednak spadek merytorycznego Exact match (z 0.7 do 0.5), co wynika z faktu, że sztywno narzucona struktura uniemożliwiła modelowi swobodną korektę wartości przy niejednoznacznych danych wejściowych.   
- Dla `qwen3:4b` oba warianty dają maksymalny wynik, natomiast w przypadku `gemma3:4b` wdrożenie `json-schema` zabezpiecza poprawność typów bez utraty jakości semantycznej. 

## Najtrudniejsze notatki

Głównie problematyczne okazały się notatki:

* NOTE-009 "Drugie lub trzecie pietro" \- modele próbują zgadnąć  2 lub 3 piętro zamiast przyznać, że nie da się określić piętra i wpisać null  
* NOTE-006 "Mieszkanie 15, domofon 15" \- często odpowiedzią stawało się 15

Zdarzały się też pewne błędy dla:

* NOTE-002 "2p bez windy, drzwi na koncu" \- zapis typowy dla tego problemu “2p” bywał niejasny nawet, gdy model otrzymał wcześniej przykłady  
* NOTE-010 "Winda nie dziala, dostawa na 5p" \- niedziałająca winda była mylona z brakiem windy w budynku

## Rekomendacja

**Qwen3:4b** może być używany do operacji, które wymagają precyzji, ale jednocześnie nie wymagają szybkiej odpowiedzi. Można by go użyć do generowania prompta użytkownika dla agenta, który będzie działać później.  
**Gemma3:4** jest szybka i wystarczająco poprawna dla większości zastosowań.  
**Mistral** nie zostanie użyty w naszym projekcie, ponieważ jego działanie jest zbyt nieprzewidywalne i może nawet prowadzić do wyjątków i zatrzymania programu.  
