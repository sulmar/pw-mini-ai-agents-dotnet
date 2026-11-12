# EXP-05 — układanie contestów dla grupy (SC-01, SC-02, SC-04)

**Zespół:** Jan Kaszycki, Wiktor Osior (TrAIningRound)

Szczegółowy opis eksperymentu dodatkowego z [Laboratorium 03](report.md). Omawiamy tylko scenariusze
SC-01, SC-02 i SC-04. Scenariusze SC-03 i SC-05 pomijamy, bo pula zadań kończy się na 1400, a najmocniejsi
uczestnicy mają tam 1650 i 1850: nie da się ułożyć contestu, który kończy się nad najmocniejszym,
więc te scenariusze nie sprawdzają tego, co chcemy.

## Cel

Sprawdzić, czy model językowy potrafi samodzielnie ułożyć **dobry contest treningowy** dla grupy
zawodników z Codeforces. To jedno z dwóch głównych działań TrAIningRound ([wizja](https://github.com/WiktorOsior/algobot-ai-agents-mini/blob/main/docs/VISION.md)).
Wynik ma odpowiedzieć na pytanie projektowe: czy układanie contestu można zostawić modelowi,
czy reguły trzeba sprawdzać w kodzie.

## Jak przebiegał test

1. Model dostaje jeden prompt: listę uczestników (rating i zadania z puli, które już rozwiązali)
   oraz pulę 20 zadań (id, rating, tagi, nazwa).
2. Ma wybrać dokładnie 4 zadania i uzasadnić wybór w 2–4 zdaniach.
3. Odpowiedź musi być w JSON. Wymuszamy to natywnym trybem JSON Schema (Ollama `format`,
   w API OpenAI `response_format`), schemat: `{"problems": [string], "justification": string}`.
4. Temperatura 0, jedno wywołanie na scenariusz i model.
5. Kod ([ContestSelection.cs](https://github.com/WiktorOsior/algobot-ai-agents-mini/blob/main/experiments/lab03-models-and-prompting/src/Experiments/EXP-05-contest-selection/ContestSelection.cs)) sprawdza
   odpowiedź według kryteriów poniżej. Wszystkie odpowiedzi są zapisane w
   [contest-results.csv](https://github.com/WiktorOsior/algobot-ai-agents-mini/blob/main/experiments/lab03-models-and-prompting/results/contest-results.csv), więc po zmianie kryteriów oceniamy je ponownie
   bez pytania modeli (`EXP-05-RESCORE`).

Modele:

| Model | Gdzie działa | Rozmiar |
|---|---|---|
| gemma3:4b | lokalnie, Ollama, GPU | 4B |
| qwen3:4b-instruct | lokalnie, Ollama, GPU | 4B |
| qwen2.5-coder:3b | lokalnie, Ollama, GPU | 3B |
| gemma4:e4b | lokalnie, Ollama, częściowo CPU (6,6 GB > 6 GB VRAM) | ~8B (efektywnie 4B) |
| gemma-4-31b (Q8_K_XL) | zdalnie przez LAN, llama.cpp na serwerze z NVIDIA CMP 170HX (4× 16 GB), API zgodne z OpenAI (model „myślący”) | 31B |

Sprzęt lokalny: Intel i7-9750HF, NVIDIA GTX 1660 Ti 6 GB, 31 GB RAM, laptop na zasilaniu.

### Prompt

Prompt jest taki sam dla każdego scenariusza, zmienia się tylko lista uczestników. Pełny prompt dla SC-02
(wypisuje go `dotnet run --project src/ModelLab -- EXP-05-PROMPT SC-02`):

```text
Jesteś agentem, który układa contesty treningowe z zadań Codeforces dla grupy zawodników.
Wybierz dokładnie 4 zadania z puli tak, żeby contest był uczciwy:
- żaden uczestnik nie rozwiązał wcześniej żadnego z wybranych zadań,
- trudność pasuje do poziomu grupy: najłatwiejsze zadanie da się zrobić dla najsłabszego uczestnika, a najtrudniejsze jest wyzwaniem dla najmocniejszego,
- zadania są ułożone od najłatwiejszego do najtrudniejszego,
- tematy są możliwie różne.
Uzasadnij wybór w 2–4 zdaniach.

Uczestnicy:
- szeldon555: rating 1216, rozwiązane zadania z puli: brak
- bstopczynski: rating 1248, rozwiązane zadania z puli: brak
- jkssss: rating 1371, rozwiązane zadania z puli: brak
- ania_k: rating 1310, rozwiązane zadania z puli: 6C, 508B, 203C, 1846E1

Pula zadań (id | rating | tagi | nazwa):
- 1228A | rating 800 | brute force, implementation | Distinct Digits
- 996A | rating 800 | dp, greedy | Hit the Lottery
- 1011A | rating 900 | greedy, implementation, sortings | Stages
- 1324A | rating 900 | implementation, number theory | Yet Another Tetris Problem
- 1475A | rating 900 | math, number theory | Odd Divisor
- 648A | rating 900 | constructive algorithms, implementation | Наибольший подъем
- 1760D | rating 1000 | implementation, two pointers | Challenging Valleys
- 712B | rating 1100 | implementation, strings | Memory and Trident
- 1512D | rating 1200 | constructive algorithms, data structures, greedy | Corrupted Array
- 2072C | rating 1200 | bitmasks, constructive algorithms, greedy | Creating Keys for StORages Has Become My Main Skill
- 2210C1 | rating 1200 | greedy, number theory | A Simple GCD Problem (Easy Version)
- 432B | rating 1200 | brute force, greedy, implementation | Football Kit
- 6C | rating 1200 | greedy, two pointers | Alice, Bob and Chocolate
- 1846E1 | rating 1300 | brute force, implementation, math | Rudolf and Snowflakes (simple version)
- 2181A | rating 1300 | binary search, math, strings | Alphabet City
- 234C | rating 1300 | dp, implementation | Weather
- 508B | rating 1300 | greedy, math, strings | Anton and currency you all know
- 203C | rating 1400 | greedy, sortings | Photographer
- 2126E | rating 1400 | math, number theory | G-C-D, Unlucky!
- 961C | rating 1400 | bitmasks, brute force, implementation | Chessboard

Zwróć wyłącznie JSON:
{
  "problems": ["id", ...],
  "justification": "uzasadnienie"
}
```

Uwaga: prompt formułuje wymagania ogólnie („najłatwiejsze zadanie da się zrobić dla najsłabszego,
a najtrudniejsze jest wyzwaniem dla najmocniejszego”) i prosi o kolejność rosnącą. Dokładnych progów
z kryteriów oceny model nie zna. To świadome ograniczenie tej wersji testu (patrz [Na przyszłe testy](#na-przyszłe-testy)).

## Kryteria oceny

Dobry contest zaczyna się trochę **poniżej najsłabszego** uczestnika, kończy **w okolicy najmocniejszego**
(najlepiej trochę powyżej),
ma **progresję** trudności i **różne tematy**. Kolejność, w jakiej model poda zadania, nie ma znaczenia,
bo można je potem posortować.

| Kryterium | Warunek |
|---|---|
| Poprawny | dokładnie 4 różne zadania z puli |
| Nierozwiązane | żaden uczestnik nie rozwiązał wcześniej wybranego zadania |
| Start | najłatwiejsze zadanie ma rating od (najsłabszy − 500) do najsłabszego |
| Koniec | najtrudniejsze zadanie ma rating od (najmocniejszy − 100) do (najmocniejszy + 300) |
| Rozrzut | różnica ratingów najtrudniejszego i najłatwiejszego co najmniej 200 |
| Tematy | każde zadanie ma co najmniej jeden tag, którego nie mają pozostałe trzy |
| Uzasadnienie | co najmniej 40 znaków |

Contest jest **dobry**, gdy spełnia wszystkie kryteria. Słowa „uczciwy” używamy tylko w węższym znaczeniu
kryterium „Nierozwiązane”: nikt z grupy nie zna wcześniej żadnego zadania, więc nikt nie ma przewagi.

Jeśli w puli nie ma nierozwiązanego zadania poniżej najsłabszego (albo od najmocniejszego − 100 wzwyż) uczestnika,
start (albo koniec) liczymy względem najbliższego dostępnego zadania. W omawianych scenariuszach dotyczy
to tylko startu w SC-04.

Dla każdego scenariusza policzyliśmy wszystkie możliwe 4-elementowe zestawy z puli i sprawdziliśmy,
ile z nich spełnia kryteria. Dzięki temu wiemy, że test da się zaliczyć.

## Pula zadań

20 zadań z Codeforces z ratingiem 800–1400, pobranych przez API (`problemset.problems`).
Do promptu trafiają tylko metadane, bez treści.

| Rating | Zadanie | Nazwa | Tagi |
|---|---|---|---|
| 800 | 1228A | Distinct Digits | brute force, implementation |
| 800 | 996A | Hit the Lottery | dp, greedy |
| 900 | 1011A | Stages | greedy, implementation, sortings |
| 900 | 1324A | Yet Another Tetris Problem | implementation, number theory |
| 900 | 1475A | Odd Divisor | math, number theory |
| 900 | 648A | Наибольший подъем | constructive algorithms, implementation |
| 1000 | 1760D | Challenging Valleys | implementation, two pointers |
| 1100 | 712B | Memory and Trident | implementation, strings |
| 1200 | 1512D | Corrupted Array | constructive algorithms, data structures, greedy |
| 1200 | 2072C | Creating Keys for StORages Has Become My Main Skill | bitmasks, constructive algorithms, greedy |
| 1200 | 2210C1 | A Simple GCD Problem (Easy Version) | greedy, number theory |
| 1200 | 432B | Football Kit | brute force, greedy, implementation |
| 1200 | 6C | Alice, Bob and Chocolate | greedy, two pointers |
| 1300 | 1846E1 | Rudolf and Snowflakes (simple version) | brute force, implementation, math |
| 1300 | 2181A | Alphabet City | binary search, math, strings |
| 1300 | 234C | Weather | dp, implementation |
| 1300 | 508B | Anton and currency you all know | greedy, math, strings |
| 1400 | 203C | Photographer | greedy, sortings |
| 1400 | 2126E | G-C-D, Unlucky! | math, number theory |
| 1400 | 961C | Chessboard | bitmasks, brute force, implementation |

Pula ma nierówny rozkład: po 4–5 zadań na poziomach 900, 1200 i 1300, ale tylko po jednym na 1000
(1760D) i 1100 (712B).

## Scenariusze

### SC-01

Trzech prawdziwych zawodników z Codeforces (rangi pupil). Nikt nie rozwiązał żadnego zadania z puli. Scenariusz bazowy: sprawdza samo dopasowanie trudności i tematów.

| Uczestnik | Rating | Rozwiązane zadania z puli | Konto |
|---|---|---|---|
| szeldon555 | 1216 | — | prawdziwe (rating z 21.10.2026) |
| bstopczynski | 1248 | — | prawdziwe (rating z 21.10.2026) |
| jkssss | 1371 | — | prawdziwe (rating z 21.10.2026) |

- dostępne (nierozwiązane) zadania: 20, rating 800–1400 (800: 2, 900: 4, 1000: 1, 1100: 1, 1200: 5, 1300: 4, 1400: 3)
- start: najłatwiejsze zadanie 716–1216
- koniec: najtrudniejsze zadanie 1271–1671
- możliwych dobrych contestów: **2009**

### SC-02

Ci sami trzej zawodnicy oraz wymyślona ania_k, która rozwiązała 4 zadania z puli, w tym 3 z poziomu 1300–1400. Sprawdza, czy model omija zadania już rozwiązane, gdy zabierają one część trudniejszych zadań.

| Uczestnik | Rating | Rozwiązane zadania z puli | Konto |
|---|---|---|---|
| szeldon555 | 1216 | — | prawdziwe (rating z 21.10.2026) |
| bstopczynski | 1248 | — | prawdziwe (rating z 21.10.2026) |
| jkssss | 1371 | — | prawdziwe (rating z 21.10.2026) |
| ania_k | 1310 | 6C, 508B, 203C, 1846E1 | wymyślone |

- dostępne (nierozwiązane) zadania: 16, rating 800–1400 (800: 2, 900: 4, 1000: 1, 1100: 1, 1200: 4, 1300: 2, 1400: 2)
- start: najłatwiejsze zadanie 716–1216
- koniec: najtrudniejsze zadanie 1271–1671
- możliwych dobrych contestów: **623**

### SC-04

Wymyślona słaba grupa 900–1100. Wszystkie zadania 800–1000 z puli są już rozwiązane, więc najłatwiejsze dostępne zadanie ma 1100. Sprawdza, czy model nie da grupie „pasujących” poziomem, ale rozwiązanych zadań.

| Uczestnik | Rating | Rozwiązane zadania z puli | Konto |
|---|---|---|---|
| olek | 900 | 1228A, 996A, 648A | wymyślone |
| zosia | 1000 | 1011A, 1324A, 1475A | wymyślone |
| piotr | 1100 | 1760D | wymyślone |

- dostępne (nierozwiązane) zadania: 13, rating 1100–1400 (1100: 1, 1200: 5, 1300: 4, 1400: 3)
- start: najłatwiejsze zadanie 400–1100 (w puli nie ma nierozwiązanego zadania ≤ najsłabszego, więc liczymy od najłatwiejszego dostępnego)
- koniec: najtrudniejsze zadanie 1000–1400
- możliwych dobrych contestów: **115**

## Wyniki — podsumowanie

Liczba scenariuszy (z 3), w których kryterium jest spełnione:

| Model | Poprawny | Nierozwiązane | Start | Koniec | Rozrzut | Tematy | Uzasadnienie | **Dobry** | Czas [s] |
|---|---|---|---|---|---|---|---|---|---|
| gemma-4-31b (zdalnie) | 3/3 | 3/3 | 3/3 | 3/3 | 3/3 | 3/3 | 3/3 | **3/3** | 55.6 |
| gemma4:e4b | 3/3 | 3/3 | 3/3 | 3/3 | 3/3 | 2/3 | 3/3 | **2/3** | 62.3 |
| qwen3:4b-instruct | 3/3 | 2/3 | 3/3 | 3/3 | 3/3 | 2/3 | 3/3 | **2/3** | 8.9 |
| gemma3:4b | 3/3 | 2/3 | 3/3 | 0/3 | 2/3 | 1/3 | 3/3 | **0/3** | 12.5 |
| qwen2.5-coder:3b | 3/3 | 2/3 | 3/3 | 0/3 | 0/3 | 3/3 | 3/3 | **0/3** | 6.9 |

Czas to średni czas jednej odpowiedzi. gemma4:e4b jest wolna, bo nie mieści się w VRAM;
gemma-4-31b jest wolna, bo przed odpowiedzią „myśli”.

## Wyniki — szczegóły

Zadania są posortowane według ratingu (model mógł podać je w innej kolejności).
⚠ oznacza zadanie, które ktoś z grupy już rozwiązał. Uzasadnienia są cytowane dosłownie.

### SC-01

#### gemma-4-31b (zdalnie) — ✓ dobry

| Rating | Zadanie | Tagi | |
|---|---|---|---|
| 800 | 996A | dp, greedy |  |
| 1000 | 1760D | implementation, two pointers |  |
| 1200 | 2072C | bitmasks, constructive algorithms, greedy |  |
| 1400 | 2126E | math, number theory |  |

Uzasadnienie modelu: „Zestaw obejmuje bardzo różnorodne tematy: DP, two pointers, bitmaski oraz teorię liczb. Trudność zadań rośnie od 800 do 1400 punktów, co zapewnia dostępność dla najsłabszego uczestnika i odpowiednie wyzwanie dla najsilniejszego.”

#### gemma4:e4b — ✓ dobry

| Rating | Zadanie | Tagi | |
|---|---|---|---|
| 800 | 1228A | brute force, implementation |  |
| 900 | 1011A | greedy, implementation, sortings |  |
| 1200 | 6C | greedy, two pointers |  |
| 1400 | 2126E | math, number theory |  |

Uzasadnienie modelu: „Wybrano zadania tworzące płynną krzywą trudności, zaczynając od prostego problemu implementacyjnego (800) i kończąc na wyzwaniu z teorii liczb (1400). Ta struktura zapewnia, że najsłabszy uczestnik ma szansę na pierwsze punkty, podczas gdy najmocniejszy będzie musiał zmierzyć się z zaawansowanym problemem matematycznym. Tematy są zróżnicowane: od implementacji, przez algorytmy zachłanne (greedy), aż po teorię liczb.”

#### qwen3:4b-instruct — ✓ dobry

| Rating | Zadanie | Tagi | |
|---|---|---|---|
| 800 | 1228A | brute force, implementation |  |
| 900 | 1011A | greedy, implementation, sortings |  |
| 1000 | 1760D | implementation, two pointers |  |
| 1300 | 2181A | binary search, math, strings |  |

Uzasadnienie modelu: „Wszystkie zadania są nowe dla uczestników. Zadania od 800 do 1300 ratingu są odpowiednie dla grupy o ratingach od 1216 do 1371. Wybrane zadania różnią się tematyką: implementacja, sortowanie, liczby, struktury danych i algorytmy. Ułożone są od łatwego do trudnego, co pozwala na płynny rozwój umiejętności. Zadania 1228A, 1011A, 1760D i 2181A oferują różnorodność i odpowiedni poziom trudności dla wszystkich uczestników.”

#### gemma3:4b — ✗ nie spełnia kryteriów

| Rating | Zadanie | Tagi | |
|---|---|---|---|
| 800 | 1228A | brute force, implementation |  |
| 900 | 1324A | implementation, number theory |  |
| 1200 | 6C | greedy, two pointers |  |
| 1200 | 2210C1 | greedy, number theory |  |

Błędy:

- koniec: najtrudniejsze ma 1200, a powinno mieć 1271–1671
- tematy: 1324A [implementation, number theory] nie wnosi nowego tematu
- tematy: 2210C1 [greedy, number theory] nie wnosi nowego tematu

Uzasadnienie modelu: „Wybieram te 4 zadania, aby stworzyć contest zróżnicowany pod względem tematycznym i trudności, dostosowany do poziomu uczestników. Zadania 6C i 1228A to problemy o niskim ratingu (800), które powinny być łatwe dla wszystkich, nawet dla najsłabszych. Zadanie 2210C1 to problem o ratingu 1200, który wymaga zastosowania algorytmów opartego na GCD, a 1324A to problem o ratingu 900 z wykorzystaniem liczb pierwszych i implementacji, który stanowi wyzwanie dla uczestników z wyższym ratingiem.  Uważam, że ta kombinacja zapewnia sprawiedliwą i angażującą rozgrywkę dla całej grupy.”

#### qwen2.5-coder:3b — ✗ nie spełnia kryteriów

| Rating | Zadanie | Tagi | |
|---|---|---|---|
| 800 | 1228A | brute force, implementation |  |
| 800 | 996A | dp, greedy |  |
| 900 | 1011A | greedy, implementation, sortings |  |
| 900 | 1324A | implementation, number theory |  |

Błędy:

- koniec: najtrudniejsze ma 900, a powinno mieć 1271–1671
- rozrzut: 900−800 = 100, a powinno być co najmniej 200

Uzasadnienie modelu: „Wybrane zadania są łatwe i łatwiejsze niż zadania z puli dla grupy zawodników. Najłatwiejsze zadanie (1228A) jest łatwe do rozwiązywania i jest dla najsłabszego uczestnika. Najtrudniejsze zadanie (1324A) jest trudne, ale jest dla najmocniejszego uczestnika. Zadania są ułożone od najłatwiejszego do najtrudniejszego, co jest kluczowe dla uczciwego kontestu. Tematy są różne, co zapewnia różnorodność i różnorodność w rozwiązywaniu zadań.”

### SC-02

#### gemma-4-31b (zdalnie) — ✓ dobry

| Rating | Zadanie | Tagi | |
|---|---|---|---|
| 800 | 996A | dp, greedy |  |
| 1100 | 712B | implementation, strings |  |
| 1200 | 2072C | bitmasks, constructive algorithms, greedy |  |
| 1400 | 2126E | math, number theory |  |

Uzasadnienie modelu: „Zestaw oferuje progresywny wzrost trudności (800, 1100, 1200, 1400), co zapewnia dostępność dla najsłabszego uczestnika i stanowi wyzwanie dla najmocniejszego. Tematyka jest zróżnicowana: programowanie dynamiczne, operacje na stringach, maski bitowe oraz teoria liczb. Wybrane zadania nie zostały wcześniej rozwiązane przez żadnego z uczestników.”

#### gemma4:e4b — ✓ dobry

| Rating | Zadanie | Tagi | |
|---|---|---|---|
| 800 | 1228A | brute force, implementation |  |
| 900 | 1011A | greedy, implementation, sortings |  |
| 1200 | 1512D | constructive algorithms, data structures, greedy |  |
| 1400 | 2126E | math, number theory |  |

Uzasadnienie modelu: „Wybrano cztery zadania o rosnącej trudności, zaczynając od 1228A (800) jako przystępnego wprowadzenia dla najsłabszego uczestnika. Następnie, 1011A (900) wprowadza element algorytmiki opartej na sortowaniu i greedy, a 1512D (1200) stanowi wyzwanie konstrukcyjne. Najtrudniejsze zadanie, 2126E (1400), wymaga zaawansowanej wiedzy z teorii liczb, idealnie testując najmocniejszych zawodników. Wszystkie zadania są nowe dla wszystkich uczestników, zapewniając uczciwy start.”

#### qwen3:4b-instruct — ✓ dobry

| Rating | Zadanie | Tagi | |
|---|---|---|---|
| 800 | 1228A | brute force, implementation |  |
| 900 | 1011A | greedy, implementation, sortings |  |
| 1000 | 1760D | implementation, two pointers |  |
| 1300 | 2181A | binary search, math, strings |  |

Uzasadnienie modelu: „Ania_k już rozwiązała 6C, 508B, 203C i 1846E1, więc te zadania są wykluczone. Najniższy rating uczestników to 1216, więc zadania powinny mieć rating od 800 do 1300. Wybrane zadania: 1228A (800 - implementacja, łatwe), 1011A (900 - sortowanie), 1760D (1000 - dwa wskaźniki), 2181A (1300 - binarny wyszukiwania) są nowe, różnią się tematyką (implementacja, sortowanie, dwa wskaźniki, binarny wyszukiwania) i spełniają warunki trudności i uczciwości.”

#### gemma3:4b — ✗ nie spełnia kryteriów

| Rating | Zadanie | Tagi | |
|---|---|---|---|
| 800 | 1228A | brute force, implementation |  |
| 800 | 996A | dp, greedy |  |
| 900 | 1011A | greedy, implementation, sortings |  |
| 1000 | 1760D | implementation, two pointers |  |

Błędy:

- koniec: najtrudniejsze ma 1000, a powinno mieć 1271–1671

Uzasadnienie modelu: „Wybieram te cztery zadania, aby stworzyć contest zróżnicowany pod względem tematycznym i trudności, dostosowany do poziomu uczestników.  Zaczynam od prostych zadań, takich jak 1228A (Distinct Digits) i 996A (Hit the Lottery) o ratingu 800, które powinny być rozwiązywane przez nawet najsłabszych uczestników. Następnie dodaję 1011A (Stages) i 1760D (Challenging Valleys) o ratingu 900 i 1000, aby wprowadzić nieco większy poziom wyzwania.  Zadania te obejmują różne techniki, takie jak brute force, greedy, DP i konstrukcje algorytmiczne, co powinno sprawić, że contest będzie interesujący dla wszystkich uczestników, a jednocześnie nie zbytnio frustrujący dla najsłabszych.”

#### qwen2.5-coder:3b — ✗ nie spełnia kryteriów

| Rating | Zadanie | Tagi | |
|---|---|---|---|
| 800 | 1228A | brute force, implementation |  |
| 800 | 996A | dp, greedy |  |
| 900 | 1011A | greedy, implementation, sortings |  |
| 900 | 1324A | implementation, number theory |  |

Błędy:

- koniec: najtrudniejsze ma 900, a powinno mieć 1271–1671
- rozrzut: 900−800 = 100, a powinno być co najmniej 200

Uzasadnienie modelu: „Wybrane zadania są łatwe i łatwiejsze niż zadania z puli, które rozwiązali już uczestnicy. Zadania są ułożone od najłatwiejszego do najtrudniejszego, co jest kluczowe dla uczciwego kontestu. Tematy są różne, co zapewnia różnorodność i różnorodność w rozwiązywaniu zadań. Najłatwiejsze zadanie (1228A) jest dla najsłabszego uczestnika, a najtrudniejsze (1324A) jest wyzwaniem dla najmocniejszego. Te zadania pomogą uczestnikom zwiększyć swoją umiejętności i poziom rozwiązywania zadań.”

### SC-04

#### gemma-4-31b (zdalnie) — ✓ dobry

| Rating | Zadanie | Tagi | |
|---|---|---|---|
| 1100 | 712B | implementation, strings |  |
| 1200 | 1512D | constructive algorithms, data structures, greedy |  |
| 1300 | 234C | dp, implementation |  |
| 1400 | 2126E | math, number theory |  |

Uzasadnienie modelu: „Wybrałem zadania, których żaden z uczestników wcześniej nie rozwiązał, zaczynając od najłatwiejszego dostępnego (1100) i kończąc na wymagającym wyzwaniu (1400). Trudność rośnie stopniowo, a tematyka jest zróżnicowana: obejmuje stringi, algorytmy konstrukcyjne, programowanie dynamiczne oraz teorię liczb.”

#### gemma4:e4b — ✗ nie spełnia kryteriów

| Rating | Zadanie | Tagi | |
|---|---|---|---|
| 1100 | 712B | implementation, strings |  |
| 1200 | 432B | brute force, greedy, implementation |  |
| 1300 | 2181A | binary search, math, strings |  |
| 1400 | 2126E | math, number theory |  |

Błędy:

- tematy: 712B [implementation, strings] nie wnosi nowego tematu

Uzasadnienie modelu: „Wybrano cztery zadania, które zapewniają płynną progresję trudności, zaczynając od 712B (1100) i kończąc na 2126E (1400). Zadania te są unikalne dla każdego uczestnika, a ich tematyka jest zróżnicowana – obejmują implementację na ciągach, algorytmy zachłanne, wyszukiwanie binarne oraz teorię liczb. Taka struktura pozwala najsłabszemu uczestnikowi na rozwiązanie pierwszych zadań, podczas gdy najmocniejszy będzie musiał zmierzyć się z wyzwaniem na końcu.”

#### qwen3:4b-instruct — ✗ nie spełnia kryteriów

| Rating | Zadanie | Tagi | |
|---|---|---|---|
| 900 | 1011A | greedy, implementation, sortings | ⚠ rozwiązane: zosia |
| 1100 | 712B | implementation, strings |  |
| 1200 | 2072C | bitmasks, constructive algorithms, greedy |  |
| 1300 | 2181A | binary search, math, strings |  |

Błędy:

- zadania już rozwiązane: 1011A (zosia)
- tematy: 712B [implementation, strings] nie wnosi nowego tematu

Uzasadnienie modelu: „Zadania 1011A (900), 712B (1100), 2072C (1200) i 2181A (1300) są nowe dla wszystkich uczestników, mają odpowiednie ratingy i tematy: od implementacji i sortowania do binarnego wyszukiwania i stringów. Są różne tematycznie i spełniają warunki trudności: najłatwiejsze (1011A) jest dostępne dla oleka, a najtrudniejsze (2181A) jest wyzwanie dla piotra. Wszystkie zadania nie były wcześniej rozwiązane przez żadnego uczestnika.”

#### gemma3:4b — ✗ nie spełnia kryteriów

| Rating | Zadanie | Tagi | |
|---|---|---|---|
| 800 | 1228A | brute force, implementation | ⚠ rozwiązane: olek |
| 900 | 1011A | greedy, implementation, sortings | ⚠ rozwiązane: zosia |
| 900 | 1324A | implementation, number theory | ⚠ rozwiązane: zosia |
| 900 | 1475A | math, number theory | ⚠ rozwiązane: zosia |

Błędy:

- zadania już rozwiązane: 1228A (olek), 1011A (zosia), 1324A (zosia), 1475A (zosia)
- koniec: najtrudniejsze ma 900, a powinno mieć 1000–1400
- rozrzut: 900−800 = 100, a powinno być co najmniej 200
- tematy: 1324A [implementation, number theory] nie wnosi nowego tematu

Uzasadnienie modelu: „Wybieram te cztery zadania, aby stworzyć contest zróżnicowany pod względem tematycznym i trudności, dostosowany do poziomu uczestników. Dla oleka (900) proponuję 1228A (Distinct Digits) i 996A (Hit the Lottery) - zadania implementacyjne o niskim poziomie trudności. Zosia (1000) może spróbować 1011A (Stages) i 1324A (Yet Another Tetris Problem) - zadania z elementami greedy i number theory. Piotr (1100) ma szansę na 1475A (Odd Divisor) - zadanie z zakresu number theory.  Układam zadania od najłatwiejszych do trudniejszych, zapewniając, że nawet najsłabszy uczestnik ma szansę na sukces, a najmocniejsi będą mieli wyzwanie.”

#### qwen2.5-coder:3b — ✗ nie spełnia kryteriów

| Rating | Zadanie | Tagi | |
|---|---|---|---|
| 800 | 1228A | brute force, implementation | ⚠ rozwiązane: olek |
| 800 | 996A | dp, greedy | ⚠ rozwiązane: olek |
| 900 | 1011A | greedy, implementation, sortings | ⚠ rozwiązane: zosia |
| 900 | 1324A | implementation, number theory | ⚠ rozwiązane: zosia |

Błędy:

- zadania już rozwiązane: 1228A (olek), 996A (olek), 1011A (zosia), 1324A (zosia)
- koniec: najtrudniejsze ma 900, a powinno mieć 1000–1400
- rozrzut: 900−800 = 100, a powinno być co najmniej 200

Uzasadnienie modelu: „Wybrane zadania są łatwe i łatwiejsze niż zadania, które rozwiązałoby najsłabszy uczestnik. Zadania są również różne i różnorodne w tematach, co pomaga uczestnikom rozwiązać je w różnym stylu. Najłatwiejsze zadanie (1228A) jest dla najsłabszego uczestnika, a najtrudniejsze (1324A) jest wyzwaniem dla najmocniejszego. Dodatkowo, zadania są ułożone od najłatwiejszego do najtrudniejszego, co pomaga uczestnikom zrozumieć rozwój ich umiejętności.”

## Wnioski

1. **Tylko gemma-4-31b ułożyła dobry contest we wszystkich trzech scenariuszach.** Zawsze omijała
   rozwiązane zadania, zaczynała nisko (w SC-01 i SC-02 od 800, w SC-04 od najłatwiejszego dostępnego
   zadania 1100), kończyła na 1400 i dobierała różne tematy. Przykład dla SC-01: 996A 800 [dp, greedy]
   → 1760D 1000 [two pointers] → 2072C 1200 [bitmasks, constructive] → 2126E 1400 [number theory].
2. **gemma4:e4b i qwen3:4b-instruct: po 2/3, ale z różnych powodów.**
   - gemma4:e4b zawsze dobrze dobiera poziomy i omija rozwiązane zadania, a w SC-04 raz powtórzyła temat
     (dwa zadania ze stringami: 712B i 2181A). To drobny błąd.
   - qwen3:4b-instruct w SC-01 i SC-02 ułożył sensowne contesty 800 → 1300, ale w SC-04 dał grupie zadanie
     1011A, które rozwiązała zosia. To poważniejszy błąd, bo łamie podstawową regułę uczciwości.
     Za to jest ~7× szybszy (9 s zamiast 62 s).
3. **gemma3:4b i qwen2.5-coder:3b nie ułożyły ani jednego dobrego contestu (0/3).** Typowe błędy:
   - **nie dochodzą do końca:** biorą zadania 800–1000 nawet dla grupy,
     której najmocniejszy ma 1371,
   - **wybierają rozwiązane zadania:** w SC-04, gdzie wszystkie łatwe zadania są już zrobione,
     oba modele dały grupie wyłącznie rozwiązane zadania,
   - **qwen2.5-coder:3b ignoruje uczestników:** w każdym scenariuszu wybrał te same zadania
     1228A, 996A, 1011A i 1324A, czyli pierwsze z listy.
4. **Uzasadnienia nie są wiarygodne.** Każdy model zawsze dawał uzasadnienie i zawsze twierdził, że reguły
   są spełnione. Na przykład w SC-04:
   - qwen3:4b-instruct napisał, że zadania „nie były wcześniej rozwiązane przez żadnego uczestnika”,
     choć 1011A rozwiązała zosia,
   - gemma3:4b w uzasadnieniu przydzieliła olkowi 996A, którego wcale nie wybrała, a wszystkie wybrane
     zadania były już rozwiązane.

   Z uzasadnienia nie da się więc wywnioskować, czy contest jest dobry. Trzeba to sprawdzić w kodzie.
5. **Jakość ma swoją cenę w czasie:** gemma-4-31b potrzebowała średnio ~56 s na contest, małe modele 7–12 s.
   Przy układaniu contestu (raz na trening) to akceptowalne, przy rozmowie na czacie nie.

### Co to znaczy dla projektu

- Do układania contestów lepiej użyć dużego modelu (gemma-4-31b), a do szybkiego odczytu intencji
  z czatu małego (qwen2.5-coder:3b, patrz [raport](report.md)).
- Reguły, które da się sprawdzić automatycznie (zadania rozwiązane, zakres ratingu, różne tematy), warto
  sprawdzać w kodzie także przy dużym modelu. W agencie zrobimy to jako narzędzie (etap 3, tool calling):
  narzędzie zwraca tylko dozwolone zadania, model układa z nich contest i uzasadnia wybór, a kod sprawdza
  wynik przed pokazaniem go użytkownikowi.

## Ograniczenia testu

- **Mała próba:** 3 scenariusze i jedno wywołanie na model. Różnica jednego scenariusza to 33 punkty
  procentowe.
- **Brak pełnej powtarzalności zdalnego modelu:** gemma-4-31b przy temperaturze 0 dała dla SC-01 inny zestaw
  w dwóch przebiegach (oba zestawy są dobre według obecnych kryteriów).
- **Prompt nie zawierał dokładnych reguł oceny:** model wiedział tylko ogólnie, czego chcemy, i był proszony
  o kolejność rosnącą, której już nie oceniamy.
- **Kryterium tematów może być za surowe.** `implementation` występuje w połowie puli i często powoduje,
  że zadanie „nie wnosi nowego tematu”, mimo że contest jest sensowny (gemma4:e4b w SC-04).
- **Nierówna pula:** na poziomach 1000 i 1100 jest tylko po jednym zadaniu, co zawęża wybór.
- **Kryteria zmieniały się w trakcie.** Ostatnia zmiana: koniec contestu liczymy od (najmocniejszy − 100),
  a nie od najmocniejszego, bo zadanie 1300 dla grupy z najmocniejszym 1371 też jest dobrym zakończeniem.
  Dzięki temu qwen3:4b-instruct zaliczył SC-01 i SC-02. Pełna historia zmian jest w
  [raporcie](report.md#exp-05--dobry-contest-dla-grupy-eksperyment-dodatkowy). Odpowiedzi modeli są
  cały czas te same, zmieniała się tylko ich ocena.

## Na przyszłe testy

1. **Wpisać reguły wprost do promptu:** zacznij do 500 poniżej najsłabszego, skończ od 100 poniżej
   do 300 powyżej najmocniejszego, progresja co najmniej 200, każde zadanie z innym tematem, kolejność dowolna.
   Porównać wyniki ze starym promptem, żeby zobaczyć, ile daje samo dokładniejsze polecenie.
2. **Rozszerzyć pulę** o zadania 1500–2200 i wyrównać poziomy 1000–1100, żeby sensownie przetestować
   mocniejsze grupy (obecne SC-03 i SC-05).
3. **Powtarzać każdy scenariusz kilka razy** (np. 3×) i podawać odsetek dobrych contestów.
4. **Rozważyć pomijanie `implementation`** przy sprawdzaniu różnorodności tematów.
5. **Sprawdzić wariant z narzędziem:** model dostaje tylko listę dozwolonych zadań z kodu i układa z niej
   contest. To będzie test architektury z etapu 3.

## Odtworzenie

Z katalogu `experiments/lab03-models-and-prompting`:

```bash
dotnet run --project src/ModelLab -- EXP-05
```

```bash
OPENAI_BASE_URL=http://adres-serwera:port/v1 dotnet run --project src/ModelLab -- EXP-05 openai/gemma-4-31b:Q8_K_XL
```

```bash
dotnet run --project src/ModelLab -- EXP-05-RESCORE
```

Pierwsze polecenie uruchamia lokalne modele, drugie zdalną gemma-4-31b, a trzecie ocenia ponownie zapisane
odpowiedzi według aktualnych kryteriów.
