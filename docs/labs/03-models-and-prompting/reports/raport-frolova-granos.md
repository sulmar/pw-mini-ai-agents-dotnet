# Raport — Laboratorium 03

**Zespół:** Ulyana Frolova, Anna Granos
**Data:** 2026-10-22  

## Środowisko

| Parametr | Wartość |
|----------|---------|
| Wersja Ollama | 0.40.8 |
| Sprzęt (CPU/GPU, RAM) | Apple M3, 16 GB RAM |
| Liczba dostępnych notatek | 170 |

## EXP-00 — Sposób oceny wyników

Odpowiedź modelu porównujemy z ręcznie oznaczonym wynikiem oczekiwanym (`data/examples.csv`, 10 notatek). Odpowiedź, która nie jest poprawnym JSON z polami `floor` (integer | null) i `elevator` (boolean | null), liczymy jako błędną w każdej metryce.

| Metryka | Definicja |
|---------|-----------|
| Poprawny JSON | Liczba odpowiedzi, które dało się sparsować i zwalidować (wymagane pola i typy) |
| Floor accuracy | Odsetek notatek z poprawnie rozpoznanym piętrem (`floor` równe oczekiwanemu, w tym `null`) |
| Elevator accuracy | Odsetek notatek z poprawnie rozpoznaną informacją o windzie (`true` / `false` / `null`) |
| Exact match | Odsetek notatek, dla których oba pola są poprawne jednocześnie |
| Średni czas [ms] | Średni czas odpowiedzi modelu na jedną notatkę |
| Powtarzalne notatki (EXP-03) | Liczba notatek, dla których wszystkie powtórzenia dały identyczną odpowiedź |

Uzasadnienie: exact match mówi, czy agent może bezpiecznie użyć wyniku, ale osobne metryki dla pól pokazują, *gdzie* model się myli (np. gemma3:4b rozpoznaje piętra w 80%, a windę tylko w 50%). `null` traktujemy jako pełnoprawną wartość — „nie wiem” jest poprawną odpowiedzią, a dopowiadanie brakujących danych jest błędem. Oddzielne liczenie poprawnego JSON pozwala odróżnić błąd formatu od błędu treści, a czas — ocenić koszt jakości.

## EXP-01 — Porównanie modeli

| Model | Poprawny JSON | Floor accuracy | Elevator accuracy | Exact match | Średni czas [ms] |
|-------|---------------|----------------|-------------------|-------------|------------------|
| gemma3:4b | 10/10 | 80% | 50% | 40% | 1862 |
| qwen3:4b | 10/10 | 90% | 100% | 90% | 63766 |

**Exact match** oznacza odsetek notatek, dla których model poprawnie rozpoznał jednocześnie oba pola: `floor` i `elevator`.

**Wnioski:**

- gemma3:4b ma problem przede wszystkim z polem `elevator` — gdy notatka nie wspomina o windzie, model wpisuje `false` zamiast `null` (NOTE-003, NOTE-004, NOTE-009), czyli dopowiada brakujące dane. Błędnie interpretuje też „winda tylko do 3” (NOTE-007) i „Winda nie działa” (NOTE-010, gdzie nie rozpoznał też piętra „5p”).
- qwen3:4b pomylił się tylko w NOTE-006 („Mieszkanie 15, domofon 15”), gdzie numer mieszkania potraktował jako piętro. Ten sam błąd popełniła gemma3:4b.
- Lepsza jakość qwen3:4b ma wysoką cenę: średni czas odpowiedzi ~64 s wobec ~1,9 s dla gemma3:4b (ok. 34× wolniej), bo qwen3 to model „myślący”, który przed odpowiedzią generuje rozumowanie.
- Większy/nowszy model nie zawsze jest lepszy — liczy się kompromis jakość/czas. qwen3:4b jest wyraźnie dokładniejszy, ale zbyt wolny dla agenta odpowiadającego na bieżąco; gemma3:4b jest szybka, ale wymaga lepszego promptu (EXP-02) i wymuszenia formatu (EXP-04).

## EXP-02 — Prompting

Model: gemma3:4b, temperature 0. One-shot — 1 przykład (EX-001 „butle są na 1 piętrze” → `floor=1, elevator=null`), few-shot — 4 przykłady z `data/prompt-examples.csv`.

| Wariant | Floor accuracy | Elevator accuracy | Exact match | Średni czas [ms] |
|---------|----------------|-------------------|-------------|------------------|
| zero-shot | 80% | 50% | 40% | 1862 |
| one-shot | 90% | 90% | 80% | 1512 |
| few-shot | 80% | 90% | 70% | 2123 |

Zmiany względem zero-shot:

| Notatka | zero-shot | one-shot | few-shot | Oczekiwano |
|---------|-----------|----------|----------|------------|
| NOTE-003 „Parter, wejście od podwórza” | ✗ elevator=false | ✓ | ✓ | 0, null |
| NOTE-004 „IV piętro, lokal 12” | ✗ elevator=false | ✓ | ✓ | 4, null |
| NOTE-007 „4 piętro, winda tylko do 3” | ✗ elevator=false | ✓ | ✓ | 4, true |
| NOTE-009 „Drugie lub trzecie piętro” | ✗ elevator=false | ✓ | ✗ floor=3 | null, null |
| NOTE-010 „Winda nie działa, dostawa na 5p” | ✗ floor=null, elevator=false | ✗ elevator=false | ✗ elevator=false | 5, true |
| NOTE-006 „Mieszkanie 15, domofon 15” | ✗ floor=15 | ✗ floor=15 | ✗ floor=15 | null, null |

**Wnioski:**

- Dodanie przykładów wyraźnie poprawiło wyniki: exact match wzrósł z 40% (zero-shot) do 80% (one-shot) i 70% (few-shot). Wynik zero-shot jest identyczny jak w EXP-01 (ten sam prompt, temperature 0).
- Dopiero po dodaniu przykładów poprawnie rozpoznane zostały NOTE-003, NOTE-004, NOTE-007 i NOTE-009. Główny zysk dotyczy pola `elevator`: już jeden przykład z `elevator: null` nauczył model, że brak informacji o windzie oznacza `null`, a nie `false` (elevator accuracy 50% → 90%). Przykład pokazuje regułę skuteczniej niż jej opis słowny w prompcie.
- Więcej przykładów nie zawsze znaczy lepiej: few-shot wypadł gorzej niż one-shot, bo w NOTE-009 („Drugie lub trzecie piętro”) model zaczął zgadywać `floor=3` zamiast `null`. Wszystkie przykłady few-shot mają jednoznaczne piętro, więc model mógł nauczyć się, że piętro „zawsze trzeba podać”.
- Żaden wariant nie poradził sobie z NOTE-006 (numer mieszkania potraktowany jako piętro) ani z elevator w NOTE-010 („Winda nie działa” → `false`). Wśród przykładów nie ma takich przypadków — przykłady pomagają tylko na typach błędów, które pokrywają. Aby je naprawić, trzeba dodać przykład „Mieszkanie 12” → `floor=null` i „Winda nie działa” → `elevator=true`.
- Przykłady praktycznie nie zwiększają czasu odpowiedzi (1,5–2,1 s), więc one-shot/few-shot to tani sposób na poprawę jakości małego modelu.

## EXP-03 — Temperature

Model: gemma3:4b, prompt zero-shot, każda notatka przetwarzana 5 razy (50 odpowiedzi na temperaturę).

| Temperature | Floor accuracy | Elevator accuracy | Exact match | Powtarzalne notatki | Średni czas [ms] |
|-------------|----------------|-------------------|-------------|---------------------|------------------|
| 0.0 | 80% | 50% | 40% | 10/10 | 1375 |
| 0.5 | 80% | 52% | 42% | 9/10 | 1811 |
| 1.0 | 80% | 62% | 52% | 9/10 | 1719 |

Notatki z niepowtarzalnymi odpowiedziami:

| Temperature | Notatka | Odpowiedzi (5 prób) | Oczekiwano |
|-------------|---------|---------------------|------------|
| 0.5 | NOTE-007 „4 piętro, winda tylko do 3” | 4× elevator=false, 1× elevator=true | 4, true |
| 1.0 | NOTE-003 „Parter, wejście od podwórza” | 4× elevator=false, 1× elevator=null | 0, null |

Dodatkowo przy temperature 1.0 NOTE-007 dała 5× poprawną odpowiedź (`elevator=true`), podczas gdy przy 0.0 — 5× błędną.

**Wnioski:**

- Przy temperature 0 model jest w pełni powtarzalny: 10/10 notatek dało 5 identycznych odpowiedzi, a wyniki są takie same jak w EXP-01 i zero-shot w EXP-02. Błędy też są powtarzalne — dzięki temu można je zdiagnozować i poprawić promptem (jak w EXP-02).
- Wraz ze wzrostem temperatury pojawiają się różne odpowiedzi na tę samą notatkę (9/10 powtarzalnych przy 0.5 i 1.0). Zmienność dotyczy wyłącznie niepewnych przypadków (pole `elevator` w NOTE-003, NOTE-007); notatki jednoznaczne pozostają stabilne.
- Wyższy exact match przy temperature 1.0 (52% vs 40%) nie oznacza, że wyższa temperatura jest lepsza: wynika głównie z tego, że w NOTE-007 model „wylosował” poprawną odpowiedź. Na 10 notatkach i 5 powtórzeniach to różnica kilku odpowiedzi — przy kolejnym uruchomieniu może być odwrotnie. Losowość nie naprawia systematycznych błędów (NOTE-004, 006, 009, 010 są błędne przy każdej temperaturze).
- Temperatura nie wpływa na poprawność formatu (50/50 poprawnych JSON) ani istotnie na czas odpowiedzi.
- Dla agenta, który ekstrahuje dane, wybieramy temperature 0 — liczy się powtarzalność i możliwość testowania. Wyższą temperaturę (np. 0.7–1.0) stosujemy dla agenta generującego treści (opisy, wiadomości), gdzie różnorodność jest zaletą.

## EXP-04 — Structured output — JSON i walidacja

Model: gemma3:4b, prompt zero-shot, temperature 0.

| Wariant | Poprawny JSON | Floor accuracy | Elevator accuracy | Exact match | Średni czas [ms] |
|---------|---------------|----------------|-------------------|-------------|------------------|
| prompt | 10/10 | 80% | 50% | 40% | 1276 |
| json-schema | 10/10 | 80% | 60% | 50% | 1165 |

**Wnioski:**

- Oba warianty zwróciły 10/10 poprawnych JSON. Wariant `prompt` nie daje jednak gwarancji formatu — format jest tylko prośbą w treści promptu i model może go nie zachować (np. opakować JSON w blok Markdown ` ```json ... ``` `). Tryb `json-schema` wymusza format na poziomie generowania — model nie może zwrócić niczego poza obiektem zgodnym ze schematem.
- Poprawny składniowo JSON nie oznacza poprawnej odpowiedzi: przy 10/10 poprawnych JSON exact match wynosi tylko 40–50%. Schemat gwarantuje strukturę i typy (`floor`: integer | null, `elevator`: boolean | null), ale nie semantykę — model nadal wpisuje `floor=15` dla numeru mieszkania czy `elevator=false` przy braku informacji.
- `json-schema` poprawił jedną notatkę (NOTE-003: `elevator=null` zamiast `false`), prawdopodobnie dlatego, że schemat jawnie dopuszcza `null` jako wartość. Pozostałe błędy są identyczne jak w wariancie `prompt`.
- Dlatego agent potrzebuje dwóch poziomów kontroli: wymuszenia formatu (JSON Schema) oraz walidacji/ewaluacji treści (metryki z EXP-00, zbiór testowy z etykietami).

## Najtrudniejsze notatki

| Notatka | Typowa błędna odpowiedź | Przyczyna |
|---------|-------------------------|-----------|
| NOTE-006 „Mieszkanie 15, domofon 15” | `floor=15` | Błąd we wszystkich eksperymentach i obu modelach. Model traktuje każdą liczbę w notatce jako piętro; brak przykładu, że numer mieszkania/domofonu to nie piętro. |
| NOTE-010 „Winda nie działa, dostawa na 5p” | `floor=null, elevator=false` | Przyjęta interpretacja jest nieintuicyjna („nie działa” → `true`, bo badamy obecność windy). gemma3:4b nie rozpoznała też skrótu „5p” w tym kontekście (poprawiło się dopiero z przykładami). Poprawnie tylko qwen3:4b. |
| NOTE-007 „4 piętro, winda tylko do 3” | `elevator=false` | Winda istnieje, ale nie dojeżdża na piętro docelowe — model interpretuje to jako brak windy. Odpowiedź zmienia się przy wyższej temperaturze, co pokazuje niepewność modelu. |
| NOTE-003, NOTE-004, NOTE-009 (brak informacji o windzie) | `elevator=false` | gemma3:4b w zero-shot dopowiada brakujące dane: brak wzmianki o windzie traktuje jak brak windy. Naprawia to już jeden przykład z `elevator: null`. |
| NOTE-009 „Drugie lub trzecie piętro” | `floor=3` (few-shot) | Niejednoznaczne piętro — przy przykładach z samymi jednoznacznymi piętrami model zaczyna zgadywać. |

## Rekomendacja

Rekomendujemy **gemma3:4b z promptem one-shot (lub few-shot uzupełnionym o trudne przypadki), temperature 0 i trybem JSON Schema**.

Dowody:

- **Jakość:** one-shot podniósł exact match gemma3:4b z 40% do 80% (EXP-02) — blisko qwen3:4b (90%, EXP-01).
- **Czas:** gemma3:4b odpowiada w ~1,5 s, qwen3:4b w ~64 s (EXP-01). Dla agenta działającego na bieżąco to nieakceptowalne.
- **Powtarzalność:** temperature 0 daje 10/10 powtarzalnych notatek (EXP-03), co pozwala testować agenta i porównywać wersje promptu.
- **Format:** JSON Schema gwarantuje odpowiedź, którą aplikacja może przetworzyć, i dodatkowo poprawił exact match z 40% do 50% (EXP-04).

Ograniczenia i dalsze kroki: zbiór testowy ma tylko 10 notatek, więc różnice rzędu 10 p.p. to pojedyncze odpowiedzi. Przed wdrożeniem należy oznaczyć więcej prawdziwych notatek z `data/notes.csv` (170) i na nich potwierdzić wynik, a do przykładów dodać przypadki „numer mieszkania ≠ piętro” i „winda nie działa → `elevator: true`”. qwen3:4b warto rozważyć tylko do przetwarzania wsadowego (offline), gdzie czas nie ma znaczenia.
