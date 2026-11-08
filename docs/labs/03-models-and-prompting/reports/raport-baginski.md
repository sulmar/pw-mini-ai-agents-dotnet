# Raport — Laboratorium 03

**Zespół:** Adam Bagiński  
**Data:** 22.10.2026

## Środowisko

| Parametr | Wartość |
|----------|---------|
| Wersja Ollama | 0.40.8 |
| Sprzęt (CPU/GPU, RAM) | Intel Core Ultra 9 185H, Intel Arc Graphics, około 31,4 GiB RAM dostępnego systemowi; wykorzystania GPU nie rejestrowano |
| Liczba dostępnych notatek | Szablon podaje 170; eksperymenty wykonano na 10 opisanych notatkach z `data/examples.csv` |
| Przykłady do promptów | 4 osobne notatki z `data/prompt-examples.csv` |
| System i platforma | Windows, PowerShell, .NET SDK 10.0.403 |
| Modele lokalne | `gemma3:4b`, `qwen3:4b`, uruchamiane przez Ollamę |
| Model zdalny | `gpt-6-luna`, OpenAI API, `reasoning_effort = none` |

Wyniki pochodzą z `results/results.csv` oraz zapisanych logów. Gemmę i Lunę zbadano w EXP-01–04, a Qwena tylko w EXP-01. Brakujących pomiarów nie zastępowano wartościami szacunkowymi. Czas modelu lokalnego oraz modelu API obejmuje różne warunki wykonania, dlatego porównanie dotyczy użytych konfiguracji.

## EXP-00 — Sposób oceny wyników

Odpowiedź jest porównywana z etykietami oczekiwanymi dla każdej notatki. Sprawdzamy oddzielnie poprawność formatu, rozpoznanie piętra i rozpoznanie obecności windy. Brak informacji jest reprezentowany przez `null`. Zdanie „Winda nie działa” oznacza `elevator = true`, ponieważ badamy obecność windy, a nie jej sprawność.

| Metryka | Definicja |
|---------|-----------|
| Floor accuracy | Odsetek wszystkich odpowiedzi z poprawnie rozpoznanym piętrem |
| Elevator accuracy | Odsetek wszystkich odpowiedzi z poprawną informacją o obecności windy |
| Exact match | Odsetek wszystkich odpowiedzi, w których oba pola są poprawne jednocześnie |
| Poprawny JSON | Liczba odpowiedzi zaakceptowanych przez parser: obiekt JSON zawierający pola `floor` i `elevator` o właściwych typach |
| Średni czas | Średni czas całego wywołania modelu w milisekundach |
| Powtarzalne notatki | Liczba notatek, dla których pięć prób dało jeden identyczny zapis wyniku lub błędu parsowania |

Niepoprawny format powoduje zaliczenie odpowiedzi jako błędnej we wszystkich metrykach poprawności. Dzięki oddzielnym metrykom można rozróżnić problemy z kontraktem odpowiedzi i problemy z interpretacją danych.

## EXP-01 — Porównanie modeli

Wszystkie modele otrzymały ten sam prompt zero-shot i te same 10 notatek. Temperatura wynosiła 0.

| Model | Floor accuracy | Elevator accuracy | Exact match | Średni czas [ms] |
|-------|----------------|-------------------|-------------|------------------|
| Gemma3:4b | 0%* | 0%* | 0%* | 4929 |
| Qwen3:4b | 100% | 100% | 100% | 78243 |
| GPT-6 Luna | 100% | 90% | 90% | 1975 |

**Exact match** oznacza odsetek notatek, dla których model poprawnie rozpoznał jednocześnie oba pola: `floor` i `elevator`.

\* Gemma uzyskała 0/10 odpowiedzi akceptowanych przez parser; Qwen i Luna po 10/10. Odpowiedzi Gemmy zawierały znaczniki Markdown otaczające JSON, dlatego wynik 0% nie oznacza, że wszystkie wyodrębnione wartości były błędne.

W tabeli użyto ostatniego pomiaru Gemmy z EXP-01. Poprzedni pomiar miał takie same wyniki poprawności oraz średni czas 5032 ms; nie uśredniano obu serii.

**Wnioski:** Qwen osiągnął najwyższą poprawność, lecz jego średni czas był około 39,6 razy dłuższy niż Luny. Luna pomyliła wyłącznie informację o windzie w NOTE-010, zwracając `null` zamiast `true`. Gemma nie spełniła wymagań formatu aplikacji. Wyniki dotyczą pojedynczych serii i małego zbioru, a nie ogólnego rankingu modeli.

## EXP-02 — Prompting

One-shot zawierał pierwszy przykład z `prompt-examples.csv`, a few-shot wszystkie cztery przykłady. Temperatura wynosiła 0.

| Wariant | Floor accuracy | Elevator accuracy | Exact match |
|---------|----------------|-------------------|-------------|
| Gemma — zero-shot | 0%* | 0%* | 0%* |
| Gemma — one-shot | 0%* | 0%* | 0%* |
| Gemma — few-shot | 0%* | 0%* | 0%* |
| Luna — zero-shot | 100% | 90% | 90% |
| Luna — one-shot | 100% | 90% | 90% |
| Luna — few-shot | 100% | 100% | 100% |

\* W każdym wariancie Gemmy parser odrzucił wszystkie 10 odpowiedzi. Qwena nie badano w EXP-02.

**Wnioski:** Dodanie przykładów nie rozwiązało problemu formatu Gemmy. Dla Luny cztery przykłady zwiększyły exact match z 90% do 100%. Poprawie uległa NOTE-010: zero-shot zwracał `elevator = null`, one-shot `false`, a few-shot poprawne `true`. Jeden przykład nie wystarczył do poprawienia tego przypadku. Średnie czasy Luny wyniosły odpowiednio 1565, 1214 i 1106 ms. Pojedyncza seria nie dowodzi jednak, że dodawanie przykładów zawsze skraca czas odpowiedzi.

## EXP-03 — Temperature

Każdą z 10 notatek przetwarzano pięć razy przy każdej temperaturze, otrzymując 50 odpowiedzi na konfigurację. Użyto promptu zero-shot.

| Temperature | Exact match | Powtarzalne notatki |
|-------------|-------------|---------------------|
| Gemma — 0.0 | 0% | 10/10* |
| Gemma — 0.5 | 0% | 10/10* |
| Gemma — 1.0 | 0% | 10/10* |
| Luna — 0.0 | 90% | 9/10 |
| Luna — 0.5 | 92% | 9/10 |
| Luna — 1.0 | 92% | 9/10 |

\* Dla Gemmy powtarzał się komunikat „Niepoprawny JSON”, a niekoniecznie surowa odpowiedź modelu. Wszystkie 50 odpowiedzi przy każdej temperaturze odrzucono. Dla Luny wszystkie odpowiedzi przeszły walidację. Qwena nie badano w EXP-03.

**Wnioski:** Temperatura nie rozwiązała problemu formatu Gemmy. W Lunie jedyną niepowtarzalną notatką była NOTE-010. Przy temperaturze 0 model zwrócił cztery razy `false` i raz `null`. Przy 0.5 zwrócił trzy razy `false`, raz `null` i raz poprawne `true`. Przy 1.0 zwrócił trzy razy `null`, raz `false` i raz poprawne `true`.

Wzrost exact match z 90% do 92% odpowiada jednej dodatkowej poprawnej odpowiedzi na 50 prób; nie uzasadnia ogólnego wniosku o przewadze wyższej temperatury. Temperatura 0 nie zagwarantowała pełnej deterministyczności. Do ekstrakcji danych wybralibyśmy niską temperaturę i poprawę instrukcji. W zadaniach generowania treści można badać wyższą temperaturę, ale tego zastosowania nie testowano w tym laboratorium.

## EXP-04 — Structured output — JSON i walidacja

| Wariant | Poprawny JSON | Exact match |
|---------|---------------|-------------|
| Gemma — prompt | 0/10 | 0% |
| Gemma — json-schema | 10/10 | 70% |
| Qwen — prompt | Brak pomiaru EXP-04 | Brak pomiaru |
| Qwen — json-schema | Brak pomiaru | Brak pomiaru |
| Luna — prompt | 10/10 | 100% |
| Luna — json-schema | 10/10 | 90% |

**Wnioski:** Wymuszenie JSON Schema pozwoliło uzyskać 10/10 odpowiedzi Gemmy akceptowanych przez parser. Nie zapewniło poprawności treści: floor accuracy i elevator accuracy wyniosły po 80%, a exact match 70%. Sam opis formatu w prompcie nie wystarczył do uzyskania poprawnego wyjścia. Luna zwróciła poprawny JSON w obu wariantach (10/10). Sam prompt dał 100% exact match, a z JSON Schema 90%: w NOTE-010 model zwrócił `elevator = null` zamiast `true`. Różnica to jedna notatka w pojedynczej serii, więc nie dowodzi, że JSON Schema pogarsza poprawność; w serii EXP-01 ta sama notatka również była błędna, a wynik 100% w wariancie prompt pokazuje zmienność tej odpowiedzi. Dla Luny wymuszenie schematu nie było potrzebne do uzyskania poprawnego formatu. Pomiarów EXP-04 Qwena nie wykonano; wyniki zero-shot z EXP-01 nie zastępują osobnych pomiarów EXP-04.

## Najtrudniejsze notatki

| Notatka | Oczekiwany wynik | Zaobserwowany błąd |
|---------|-----------------|-------------------|
| NOTE-006: „Mieszkanie 15, domofon 15” | `floor = null`, `elevator = null` | Gemma z JSON Schema podała piętro 15, myląc numer mieszkania z piętrem |
| NOTE-009: „Drugie lub trzecie piętro” | `floor = null`, `elevator = null` | Gemma z JSON Schema podała `elevator = false`, dopowiadając brak windy |
| NOTE-010: „Winda nie działa, dostawa na 5p” | `floor = 5`, `elevator = true` | Gemma z JSON Schema podała `floor = null`, `elevator = false`; Luna w zero-shot i one-shot myliła obecność windy z jej sprawnością |

Największą trudność sprawiła NOTE-010. Wskazuje to na potrzebę jawnego rozróżnienia obecności i sprawności windy w instrukcji. Osobnym, niezależnym problemem był format wyjściowy Gemmy.

## Rekomendacja

Do dalszych prac wybralibyśmy **GPT-6 Luna z promptem few-shot i temperaturą 0**. W wykonanym pomiarze uzyskała 100% exact match oraz 10/10 odpowiedzi akceptowanych przez parser, przy średnim czasie 1106 ms. Qwen również uzyskał 100%, już w zero-shot, ale ze znacznie dłuższym czasem odpowiedzi; jest kandydatem do wariantu wymagającego działania lokalnego.

Przed wykorzystaniem rozwiązania należałoby potwierdzić wyniki na większym, niezależnym zbiorze oraz sprawdzić połączenie few-shot z JSON Schema. Takiego połączenia nie przetestowano w zapisanych pomiarach. Należy też uzupełnić brakujące eksperymenty Qwena (EXP-02, EXP-03, EXP-04), aby porównywać modele w tym samym zakresie.

Dowodem rekomendacji są zapisane metryki i logi, ale zakres wniosku ogranicza się do 10 różnych notatek. Powtórzenia EXP-03 nie zwiększają liczby unikalnych przypadków. Nie mierzono kosztów API, liczby tokenów, rozrzutu czasów ani wpływu rozgrzewki modeli lokalnych, dlatego nie można na podstawie tych danych oceniać kosztu rozwiązania i ogólnej przewagi wydajnościowej modeli.
