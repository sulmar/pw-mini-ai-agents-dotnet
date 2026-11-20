# Raport — Laboratorium 03

**Zespół:** Jan Kaszycki, Wiktor Osior (TrAIningRound)

Zamiast notatek kierowców badamy zadanie z naszego projektu: agent na czacie musi zrozumieć,
czego chce użytkownik (EXP-01 – EXP-04), a potem umieć ułożyć dobry contest dla grupy (EXP-05).
Opis zadań i danych: [README](https://github.com/WiktorOsior/algobot-ai-agents-mini/blob/main/experiments/lab03-models-and-prompting/README.md). Dokładny opis eksperymentów (dane, prompty, wyniki każdej
wiadomości, pełne interakcje z modelami): [opis-eksperymentow.md](opis-eksperymentow.md).

## Środowisko

| Parametr | Wartość |
|----------|---------|
| Wersja Ollama | 0.40.3 |
| Sprzęt (CPU/GPU, RAM) | Intel i7-9750HF, NVIDIA GTX 1660 Ti 6 GB, 31 GB RAM |
| Model zdalny | gemma-4-31b (Q8_K_XL), llama.cpp na serwerze z kartami NVIDIA CMP 170HX (4× 16 GB), dostęp przez LAN, API zgodne z OpenAI |
| .NET | 9.0 |
| Liczba wiadomości testowych | 25 (+ 5 przykładów do promptów) |
| Liczba scenariuszy contestu | 5 (pula 20 zadań, 800–1400) |

Wszystkie pomiary czasu są wykonane na laptopie podłączonym do zasilania. Pierwsza próba na baterii
(`results/EXP-01-bateria.log`) była kilka razy wolniejsza: GPU pracowało na 300 MHz zamiast ~1100 MHz.

## EXP-00 — Sposób oceny wyników

| Metryka | Definicja |
|---------|-----------|
| Mode accuracy | odsetek wiadomości z poprawnym `mode` (solo / contest / null) |
| Rating accuracy | odsetek wiadomości, w których oba końce zakresu (`rating_min`, `rating_max`) są poprawne |
| Topic accuracy | odsetek wiadomości z poprawnym tagiem |
| Exact match | odsetek wiadomości, w których wszystkie pola są poprawne |
| Poprawny JSON | odpowiedź da się sparsować i ma wszystkie pola właściwego typu |
| Powtarzalność (EXP-03) | liczba wiadomości, dla których 3 powtórzenia dały identyczną odpowiedź |

Exact match jest główną metryką, bo agent potrzebuje wszystkich pól naraz: zły zakres ratingu
przy dobrym temacie i tak daje złe zadanie. Metryki per pole pokazują, *co* model myli.

Wartość `null` traktujemy jak każdą inną. Jeśli użytkownik nie podał ratingu, a model wpisze 1600,
to jest błąd, bo agent powinien wtedy dopytać albo sprawdzić profil, a nie zgadywać.

Dla EXP-05 kryteria dobrego contestu sprawdza kod (opis w [README](https://github.com/WiktorOsior/algobot-ai-agents-mini/blob/main/experiments/lab03-models-and-prompting/README.md#zadania-dla-modelu)).
Contest jest **dobry**, gdy spełnia wszystkie kryteria. Słowa „uczciwy” używamy tylko w węższym znaczeniu
kryterium „Nierozwiązane”: nikt z grupy nie zna wcześniej żadnego zadania, więc nikt nie ma przewagi.

## EXP-01 — Porównanie modeli

Prompt zero-shot, temperatura 0.

| Model | Mode | Rating | Topic | Exact match | Poprawny JSON | Średni czas [ms] |
|-------|------|--------|-------|-------------|---------------|------------------|
| gemma3:4b | 96% | 68% | 64% | 32% | 25/25 | 3741 |
| qwen3:4b-instruct | 100% | 76% | 92% | 72% | 25/25 | 1220 |
| **qwen2.5-coder:3b** | 96% | 92% | 96% | **84%** | 25/25 | **939** |
| **gemma4:e4b** | 92% | 100% | 96% | **88%** | 25/25 | 14968 |
| **gemma-4-31b** (zdalnie) | 100% | 100% | 100% | **100%** | 25/25 | 9145 |

**Wnioski:**

- **Większy lub nowszy model nie zawsze jest lepszy.** Najmniejszy model, qwen2.5-coder:3b, jest drugi
  pod względem jakości i najszybszy. gemma3:4b jest wyraźnie słabsza od niego, mimo że jest większa.
- **gemma4:e4b ma najlepszy wynik (88%), ale jest 16× wolniejsza** od qwen2.5-coder:3b. Model ma 6,6 GB
  i nie mieści się w 6 GB VRAM, więc część obliczeń idzie na CPU. Różnica jakości to 1 wiadomość
  na 25, a 15 s czekania na każdą odpowiedź w czacie jest nie do przyjęcia.
- **gemma-4-31b (zdalnie) rozwiązała wszystkie 25 wiadomości,** także te, na których wykładały się
  wszystkie lokalne modele: „dwa wskaźniki” → `two pointers` i „Div 3 albo Div 4” → 800..1599.
  To model ~8× większy od lokalnych i „myślący”: zanim odpowie, pisze rozumowanie (w osobnym polu
  `reasoning_content`, którego nie oceniamy). Stąd 9 s na odpowiedź, mimo mocniejszego sprzętu.
  Pokazuje to, że nasz zbiór testowy da się rozwiązać w całości, a błędy małych modeli wynikają
  z ich ograniczeń, a nie z niejasnych etykiet.
- **gemma3:4b myli pola:** wpisuje do `topic` rzeczy, które nie są tagami (`div2`, `div4`, `pupil`,
  `teams`), i zostawia `rating_max = null` przy jednej liczbie („mam 1216” → 1216..null).
- **qwen3:4b-instruct zgaduje rating,** którego nie podano: „Chcę poćwiczyć dp” → 1400..1599,
  „mam 1216” → cały zakres pupila 1200..1399. Dlatego ma słaby rating accuracy przy dobrym `mode`.

Do EXP-02 – EXP-04 wybraliśmy **qwen2.5-coder:3b**. Uwaga: skrypt miał wybrać model z najwyższym exact
match, ale przez polskie ustawienia regionalne (`sort -n` nie rozpoznał kropki dziesiętnej) wybrał
qwen2.5-coder:3b zamiast gemma4:e4b. Uznaliśmy, że to i tak właściwy wybór ze względu na czas
odpowiedzi, więc nie powtarzaliśmy eksperymentów.

## EXP-02 — Prompting

Model qwen2.5-coder:3b, temperatura 0. One-shot używa pierwszego przykładu z `prompt-examples.csv`,
few-shot wszystkich pięciu.

| Wariant | Mode | Rating | Topic | Exact match | Średni czas [ms] |
|---------|------|--------|-------|-------------|------------------|
| zero-shot | 96% | 92% | 96% | **84%** | 956 |
| one-shot | 96% | 88% | 96% | 80% | 693 |
| few-shot | 100% | 84% | 96% | 80% | 704 |

**Wnioski:**

- **Przykłady nie pomogły, a nawet trochę zaszkodziły.** Dopiero po dodaniu przykładów zaczęła przechodzić
  tylko jedna wiadomość: MSG-007 (rozmowa rekrutacyjna) w few-shot. Za to dwie nowe przestały.
  Prompt z tabelami rang i dywizji okazał się wystarczający.
- **Few-shot wprowadził nowe błędy przez naśladowanie przykładu.** Przykład „Div 1” → `1900..null`
  sprawił, że model zaczął wpisywać `rating_max = null` także dla Div 2 i dla rangi expert
  (MSG-002, MSG-011: `1600..null` zamiast `1600..1899`). Model skopiował wzór zamiast reguły.
- Przy dobrze opisanych regułach przykłady trzeba dobierać ostrożnie: przykład z wyjątkiem
  (Div 1 bez górnej granicy) uczy wyjątku jako reguły.

## EXP-03 — Temperature

Model qwen2.5-coder:3b, prompt zero-shot, każda wiadomość 3 razy.

| Temperature | Exact match | Powtarzalne wiadomości |
|-------------|-------------|---------------------|
| 0.0 | 84% | 25/25 |
| 0.5 | 71% | 14/25 |
| 1.0 | 60% | 8/25 |

**Wnioski:**

- Przy temperaturze 0 model odpowiada za każdym razem tak samo i najlepiej. Każde zwiększenie temperatury
  obniża zarówno poprawność, jak i powtarzalność.
- Najbardziej niestabilne są wiadomości, w których trzeba coś wywnioskować: zakres z dwóch dywizji
  (MSG-016), „coś trudniejszego niż ostatnio” (MSG-004), rozmowa rekrutacyjna (MSG-007).
- **Do odczytu intencji i wyboru zadań używamy temperatury 0.** Wyższa temperatura może mieć sens tylko
  przy generowaniu treści, np. przy wyjaśnieniach teachera, gdzie różnorodność sformułowań nie szkodzi.

## EXP-04 — Structured output — JSON i walidacja

Model qwen2.5-coder:3b, temperatura 0.

| Wariant | Poprawny JSON | Exact match | Średni czas [ms] |
|---------|---------------|-------------|------------------|
| prompt | 25/25 | 84% | 690 |
| json-schema | 25/25 | 84% | 678 |

**Wnioski:**

- qwen2.5-coder:3b zwraca poprawny JSON także bez wymuszania schematu, więc oba warianty dały ten sam wynik.
- **Poprawny składniowo JSON nie oznacza poprawnej odpowiedzi:** 25/25 poprawnych JSON-ów, ale tylko 84%
  poprawnych odpowiedzi. Schemat pilnuje typów, nie sensu.
- JSON Schema i tak zostawiamy w agencie, bo chroni przed modelami, które dodają komentarz albo blok
  kodu wokół JSON (tak robiła gemma3:4b).

## EXP-05 — Dobry contest dla grupy (eksperyment dodatkowy)

Szczegółowy opis testów i wyników dla scenariuszy SC-01, SC-02 i SC-04: [exp05-contesty.md](exp05-contesty.md).

Każdy model dostaje 5 grup uczestników i pulę 20 zadań. Ma wybrać 4 zadania i uzasadnić wybór.
JSON Schema, temperatura 0.

Dobry contest ma progresję trudności: zaczyna się trochę poniżej najsłabszego uczestnika i kończy
w okolicy najmocniejszego (od 100 poniżej do 300 powyżej), a zadania dotyczą różnych tematów. Kolejność, w jakiej model poda zadania,
nie ma znaczenia, bo można je potem posortować. Kryteria sprawdzane w kodzie:

| Kryterium | Warunek |
|---|---|
| poprawny | dokładnie 4 różne zadania z puli |
| nierozwiązane | żaden uczestnik nie rozwiązał wybranego zadania |
| start | najłatwiejsze zadanie od (najsłabszy − 500) do najsłabszego |
| koniec | najtrudniejsze zadanie od (najmocniejszy − 100) do (najmocniejszy + 300) |
| rozrzut | różnica ratingów najtrudniejszego i najłatwiejszego co najmniej 200 |
| tematy | każde zadanie ma co najmniej jeden tag, którego nie mają pozostałe trzy |
| uzasadnienie | co najmniej 40 znaków |

Gdy w puli nie ma nierozwiązanego zadania poniżej najsłabszego albo w okolicy najmocniejszego uczestnika,
start i koniec liczymy względem najbliższego dostępnego zadania. Dotyczy to SC-03 i SC-05 (najmocniejszy
ma 1650 i 1850, a pula kończy się na 1400) oraz SC-04 (wszystkie zadania 800–900 są już rozwiązane).
Dla każdego scenariusza sprawdziliśmy, licząc wszystkie zestawy, że da się ułożyć co najmniej 74
dobre contesty.

Liczba scenariuszy, w których kryterium jest spełnione:

| Model | Poprawny | Nierozwiązane | Start | Koniec | Rozrzut | Tematy | Uzasadnienie | **Dobry** | Czas [s] |
|-------|---|---|---|---|---|---|---|---|---|
| gemma3:4b | 4/5 | 2/5 | 4/5 | 0/5 | 3/5 | 2/5 | 5/5 | **0/5** | 11.7 |
| qwen3:4b-instruct | 5/5 | 3/5 | 4/5 | 4/5 | 5/5 | 2/5 | 5/5 | **2/5** | 8.2 |
| qwen2.5-coder:3b | 5/5 | 3/5 | 4/5 | 0/5 | 0/5 | 5/5 | 5/5 | **0/5** | 6.1 |
| gemma4:e4b | 5/5 | 5/5 | 4/5 | 5/5 | 5/5 | 3/5 | 5/5 | **2/5** | 60.4 |
| gemma-4-31b (zdalnie) | 5/5 | 5/5 | 4/5 | 5/5 | 5/5 | 5/5 | 5/5 | **4/5** | 59.5 |

Historia kryteriów: w pierwszej wersji wymagaliśmy, żeby *każde* zadanie mieściło się w zakresie
[najsłabszy − 400, najmocniejszy + 200] i żeby zadania były podane rosnąco. Karało to zadania 800 dla
grupy od 1216, czyli dokładnie pożądany łagodny start, a kolejność i tak można poprawić w kodzie.
Nie sprawdzaliśmy też różnorodności tematów, choć prompt o nią prosił. Na koniec złagodziliśmy warunek
końca: najtrudniejsze zadanie może mieć do 100 mniej niż najmocniejszy uczestnik. Odpowiedzi modeli zostały te same;
przeliczyliśmy je według nowych kryteriów
(`EXP-05-RESCORE`, aktualne oceny w `results/EXP-05-rescore.log` i `results/contest-results.csv`;
logi `EXP-05.log` i `EXP-05-gemma-4-31b.log` zawierają pełne odpowiedzi, ale oceny według starych kryteriów). Wcześniej poprawiliśmy też scenariusz SC-04, w którym przy starych kryteriach
nie dało się ułożyć żadnego dobrego contestu, i uruchomiliśmy modele ponownie.

**Wnioski:**

- **gemma-4-31b układa dobre contesty: 4/5.** Zawsze omija rozwiązane zadania, ma progresję, kończy
  na odpowiednio trudnym zadaniu i dobiera różne tematy. Przykład dla trzech prawdziwych pupili
  (1216–1371): 996A 800 [dp, greedy] → 1760D 1000 [impl, two pointers] → 2072C 1200 [bitmasks,
  constructive] → 2126E 1400 [math, number theory].
- **gemma4:e4b ma dobre poziomy, ale powtarza tematy: 2/5.** W SC-03 dała dwa zadania greedy + sortings
  (1011A i 203C), w SC-04 dwa zadania ze stringami (712B i 2181A).
- **Obie gemmy wyłożyły się na SC-05** (grupa 1371–1850): zaczęły od 1228A (800), czyli 571 poniżej
  najsłabszego, choć w puli były nierozwiązane zadania 1000–1100. Przy mocnej grupie modele dalej
  „na wszelki wypadek” zaczynają od najłatwiejszego zadania z puli.
- **qwen3:4b-instruct jako jedyny mały model ułożył dobre contesty (2/5)**, w SC-01 i SC-02
  (800 → 1300 dla grupy 1216–1371). W SC-04 i SC-05 dał jednak zadania już rozwiązane.
- **Pozostałe małe modele (3–4B) nie układają dobrych contestów: 0/5.** Głównie nie dochodzą do końca:
  biorą najłatwiejsze zadania z puli (800–1000), więc najmocniejszy uczestnik nie dostaje wyzwania.
  Do tego wybierają zadania, które uczestnik już rozwiązał, mimo że lista była w prompcie.
- **qwen2.5-coder:3b ignoruje uczestników:** w każdym scenariuszu wybrał te same 4 zadania
  (1228A, 996A, 1011A, 1324A), czyli pierwsze z listy.
- **Uzasadnienia brzmią przekonująco, ale bywają fałszywe.** gemma3 w SC-05 wymyśliła zadanie o id „6”
  i przydzieliła markowi 234C, a ewie 2181A, czyli zadania, które każde z nich już rozwiązało.
  gemma-4-31b napisała w SC-05, że zakres 800–1200 jest „najlepiej dopasowany do możliwości grupy”
  1371–1850. Wszystkie modele zawsze dawały uzasadnienie, więc samo kryterium „jest uzasadnienie”
  niczego nie mówi o jakości.
- **Gemma 4 jest wolna:** około 60 s na contest (e4b lokalnie, bo nie mieści się w VRAM; 31b zdalnie,
  bo długo myśli). Przy układaniu contestu to akceptowalne, bo robi się to raz, a nie w każdej wiadomości.
- **Zdalny serwer nie jest w pełni deterministyczny.** gemma-4-31b przy temperaturze 0 dała dla SC-01
  inny zestaw niż w pierwszym przebiegu, mimo identycznego promptu. Wynik jednego przebiegu
  na 5 scenariuszach jest więc orientacyjny.

**Wniosek dla projektu:** twarde reguły (zadania już rozwiązane, zakres ratingu) warto sprawdzać w kodzie
nawet przy dobrym modelu, bo gemma 4 też raz wyszła poza zakres, a mały model nie radzi sobie z nimi wcale.
W agencie zrobimy to jako narzędzie (etap 3, tool calling): narzędzie zwraca tylko dozwolone zadania
z odpowiedniego zakresu, a model układa z nich progresję i uzasadnia wybór. Do układania contestów
lepiej użyć większego modelu (gemma-4-31b) niż do odczytu intencji.

**Na przyszłe testy:** prompt EXP-05 mówił tylko ogólnie, że „najłatwiejsze zadanie da się zrobić dla
najsłabszego, a najtrudniejsze jest wyzwaniem dla najmocniejszego”. Modele mogły więc nie wiedzieć,
czego dokładnie oczekujemy. W kolejnej wersji wpiszemy do promptu wprost te same reguły, które sprawdza
kod: zacznij trochę poniżej najsłabszego (do 500), skończ w okolicy najmocniejszego (od 100 poniżej do 300 powyżej),
zrób progresję co najmniej 200, każde zadanie z innym tematem, a kolejność nie ma znaczenia.
Wtedy sprawdzimy, ile poprawia samo dokładniejsze polecenie, szczególnie w małych modelach.

## Najtrudniejsze wiadomości

| Wiadomość | Problem |
|---|---|
| MSG-009 „Mam rating 1550 i słabo mi idą dwa wskaźniki” | żaden lokalny model nie rozpoznał „dwa wskaźniki” jako `two pointers`; to polskie tłumaczenie nazwy techniki, a lista tagów jest po angielsku |
| MSG-004 „Daj coś trudniejszego niż ostatnio” | modele zgadują rating (1600..null, 1600..1899) albo uznają, że to nie jest prośba (`mode = null`); poprawnie: solo bez ratingu, bo agent musi sprawdzić historię użytkownika |
| MSG-016 „Daj zadanie z drzew, poziom Div 3 albo Div 4” | model bierze tylko pierwszą dywizję (1400..1599) zamiast zakresu obejmującego obie (800..1599) |
| MSG-007 „Szykuję się do rozmowy rekrutacyjnej, potrzebuję zadań na stringi” | qwen2.5-coder uznaje to za `contest`, prawdopodobnie przez „zadania” w liczbie mnogiej |

Wszystkie te wiadomości poprawnie rozwiązała dopiero gemma-4-31b. Wspólna przyczyna: najtrudniejsze są wiadomości, w których trzeba coś *wywnioskować* (tłumaczenie terminu,
odniesienie do historii, suma dwóch zakresów), a nie tylko przepisać wartość.

Możliwe poprawki: dopisać do listy tagów polskie nazwy (`two pointers` = dwa wskaźniki), dodać regułę
„kilka dywizji = zakres obejmujący wszystkie” jako przykład, a przy „trudniejsze niż ostatnio” celowo
zostawić `null` i pozwolić agentowi sprawdzić historię użytkownika przez API Codeforces.

## Finalne wnioski

**Do odczytu intencji na czacie wybieramy qwen2.5-coder:3b z promptem zero-shot, temperaturą 0
i JSON Schema.**

- 84% exact match przy średnio 0,7–0,9 s na odpowiedź (EXP-01, EXP-04): drugi wynik jakości
  i najszybszy model. Lepsza gemma4:e4b (88%) jest na naszym sprzęcie 16× wolniejsza, a na czacie
  liczy się czas odpowiedzi.
- Przykłady w prompcie nie poprawiają wyniku, a few-shot wprowadza nowe błędy (EXP-02).
- Temperatura 0 daje najlepszy wynik i pełną powtarzalność (EXP-03).
- JSON jest zawsze poprawny, a schemat zabezpiecza przed modelami, które dodają tekst wokół JSON (EXP-04).

**Do układania contestów potrzebny jest mocniejszy model, np. gemma-4-31b uruchamiana zdalnie przez LAN.**

- Małe modele nie radzą sobie z tym zadaniem: qwen2.5-coder:3b ignorował uczestników i w każdym
  scenariuszu wybierał te same zadania, a pozostałe modele 3–4B dawały zadania już rozwiązane
  i za łatwe dla najmocniejszych (EXP-05).
- gemma-4-31b ułożyła dobry contest w 4 z 5 scenariuszy, a w trzech scenariuszach, które da się
  sensownie ocenić na obecnej puli (SC-01, SC-02, SC-04), we wszystkich
  ([szczegóły](exp05-contesty.md)). Odpowiada w ~40–60 s, co przy układaniu contestu
  (raz na trening) jest akceptowalne.
- Reguły, które da się sprawdzić automatycznie (zadania już rozwiązane, zakres ratingu, różne tematy),
  i tak sprawdzimy w kodzie jako narzędzie (etap 3, tool calling), bo nawet duży model raz się pomylił.

**Co jeszcze trzeba sprawdzić:**

- jak qwen2.5-coder:3b radzi sobie z wywoływaniem narzędzi; jeśli słabo, kandydatem jest
  qwen3:4b-instruct (72%, 1,2 s),
- czy serwer z gemma-4-31b będzie dostępny dla projektu na stałe; jeśli nie, zapasowo gemma4:e4b lokalnie
  (dobre poziomy trudności, ale wolna i czasem powtarza tematy),
- zbiór testowy to 25 wiadomości i 5 scenariuszy, więc wyniki traktujemy jako orientacyjne; rozszerzymy go
  przy ewaluacji (etap 7).
