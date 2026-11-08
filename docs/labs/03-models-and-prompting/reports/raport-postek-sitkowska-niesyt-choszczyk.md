# Raport — Laboratorium 03

**Zespół:**  Wiktoria Postek, Aleksandra Sitkowska, Konrad Niesyt, Aleksandra Choszczyk

**Data:**  28.10.2026

## Środowisko

| Parametr | Wartość |
|----------|---------|
| modele |DeepSeek-V3.1  oraz bielik_11b |
| Sprzęt (CPU/GPU, RAM) | nie dotyczy, eksperyment poprzez API |
| Liczba dostępnych notatek | 172 |

## EXP-00 — Sposób oceny wyników

Jak oceniamy poprawność odpowiedzi modelu? Jakie metryki przyjmujemy i dlaczego?

Poprawność odpowiedzi modelu oceniamy poprzez porównanie
wyekstraktowanych przez niego danych z wzorcem
zawartym w pliku z danymi.


| Metryka | Definicja |
|---------|-----------|
| Floor accuracy|	Odsetek poprawnie rozpoznanych pięter|
|Elevator accuracy|	Odsetek poprawnie rozpoznanych informacji o windzie|
|Exact match|	Odsetek notatek, dla których oba pola są poprawne| 
|Średni czas [ms] | Średni czas odpowiedzi modelu
|Poprawny JSON| Liczba odpowiedzi w poprawnym formacie JSON

**Dlaczego te metryki?**

Floor accuracy - Pozwala ocenić, jak dobrze model radzi sobie z wyciąganiem danych
liczbowych i określaniem parteru (0) lub piwnic (-1).

Elevator accuracy - Weryfikuje zdolność modelowania niuansów 
logicznych i detekcji obecności infrastruktury w tekście.

Exact match - Pokazuje, w ilu przypadkach model był całkowicie poprawny.

Średni czas - Pokazuje średni czas odpowiedzi modelu na jedno zapytanie.

Poprawny JSON - Pokazuje w ilu przypadkach model zwracał poprawny format odpowiedzi. 

## EXP-01 — Porównanie modeli

| Model | Floor accuracy | Elevator accuracy | Exact match | Średni czas [ms] | Poprawny JSON |
|-------|----------------|-------------------|-------------|------------------|---------------|
| DeepSeek-V3.1| 0.78 | 0.91| 0.78|339 |158/172|
| bielik_11b| 0.13 | 0.13| 0.13|509 |22/172|

**Exact match** oznacza odsetek notatek, dla których model poprawnie rozpoznał jednocześnie oba pola: `floor` i `elevator`.

**Wnioski:**

Model DeepSeek-V3.1 wykazuje dość wysoką skuteczność ekstrakcji
dla pięter (78%) oraz bardzo wysoką skuteczność ekstrakcji dla wind (91%).
Ponadto średni czas czas odpowiedzi jest bardzo niski (339 ms).
 Poprawność formatu odpowiedzi jest rowniez na wysokim poziomie ( 92%)


Model bielik_11b wykazuje baedzo niską poprawność formatu odpowiedzi tylko 13%, 
To skutkuje równie niską skutecznością ekstrakcji dla pięter i wind.

**Dalsze eksperymenty beędą wykonywane tylko dla modelu DeepSeek-V3.1.**
 
## EXP-02 — Prompting

| Wariant | Floor accuracy | Elevator accuracy | Exact match | Średni czas [ms] | Poprawny JSON |
|---------|----------------|-------------------|-------------|------------------|---------------|
| zero-shot | 0.78 | 0.91 | 0.78 | 380 | 158/172 |
| one-shot | 0.83 | 0.99 | 0.82 | 408 | 171/172 |
| few-shot | 0.86 | 0.99 | 0.85 | 534 | 171/172 |

**Wnioski:**

Dla kolejnych wariantów skuteczność ekstrakcji dla pięter i wind oraz liczba zwracanych poprawnie odpowiedzi zwiększała się, co spowodowało, ze wyniki Exact match tez sie zwiekszyły.

Wiekszy skok jakościowy nastapił z warintu zero-shot and one-shot z 78% do 82% dopasowania całkowicie poprawnej odpowiedzi (Exact match). Wiązało sie to z wydłuzeniem średniego czasu o około 28 ms. 

Pomiędzy one-shot a few-shot nie było juz takiego skoku jakości, tylko poprawność rozpoznawania piętra się zwiększyła. Natomiast średni czas zwiększył się az o 126ms.

Wynika z tego ze one-shot to najlepszy wariant łączący krótszy czas odpowiedzi i wysoką poprawność.

## EXP-03 — Temperature

| Temperature | Exact match | Powtarzalne notatki |
|-------------|-------------|---------------------|
| 0.0 | 0.78 | 171/172 |
| 0.5 | 0.8 | 134/172 |
| 1.0 | 0.76 | 109/172 |

**Wnioski:**

Przy temperaturze 1, nastąpił spadek zarówno poprawności odpowiedzi jak i ich powtarzalności.

Przy podniesieniu temperatury do 0.5 model uzyskał najwyzszy stopien poprawności. Niewielka losowość pozwoliła modelowina na określenie niejednoznaczych przypadków i określenie dla nich poprawnych odpowiedzi. Ten zysk był jednak związany ze spadkiem powtarzalność notatek.


## EXP-04 — Structured output — JSON i walidacja

**json-mode** - natywny tryb API 

| Wariant | Poprawny JSON | Exact match |
|---------|---------------|-------------|
| prompt | 158/172 | 0.78 |
| json-mode | 171/172 | 0.85 |

**Wnioski:**

Wariant oparty na natywnym trybie API okazał się skuteczniejszy niz wariant z promptem. Wiązało się to ze wiekszą poprawnością zwracanych odpowiedzi JSON.

Dlatego jesli to jest mozliwe powinno się korzystać w json-mode uzywając modelu DeepSeek-V3.1.

## Najtrudniejsze notatki

Które notatki model najczęściej interpretował błędnie i dlaczego?

prompt:

Dla wariantu z promptem. model najczęściej mylił się w notatkch które miały treść, NULL. Rozpoznawał on poprawnie ich zawartość, ale zwracał błędny format.

✗ NOTE-085 "NULL" => Niepoprawny JSON: ```json
{
  "floor": null,
  "elevator": null
}


json-mode:

Dla wariantu z json-mode, model najczeciej mylił się kiedy opisywany był parter, nie rozpoznawał parteru jako pietro 0, tylko wskazywał wartość null. Podobnie było gdy notatka zawierała informacje o kilku piętrach. Wtedy model równiez zwracał wartość null.


np:

✗ NOTE-154 "Wjazd od ul. Radiowej przez bramę nr 4 i w prawo. 2 budynek po prawej. dostawy na 3,2, 1 piętro oraz na -1" => floor=null, elevator=null (oczekiwano: floor=3, elevator=null)


✗ NOTE-086 "Budynek E parter i I piętro pok. 1110 oraz budynek A, parter i 1 piętro na korytarzu" => floor=null, elevator=null (oczekiwano: floor=0, elevator=null)



## Rekomendacja

Wstępnie do projektu wybieramy model DeepSeek-V3.1 uzywając metody one-shot z temperaturą 0. Jezeli to bedzie mozliwe spróbujemy uzywać natywnego json-mode, ale przewidujemy tez uzycie promptów.

Nasz wybór motywowany jest wynikami eksperymantów. 

 - EXP-01 uwidocznił wiekszą dokładność modelu DeepSeek-V3.1 w porownaniu z bielik_11b.

 - Wnioski z EXP-02 potwerdziły, ze wariant one-shot najefektywniej łączy poprawność odpowiedzi ze średnim czasem ich generowania.

 - W naszym porjekcie wazna jest poprawnosc i powtarzalność wyników, dlatego po wynikach EXP-03 wybraliśmy temperaturę 0.

 - EXP-04 udowodnił, ze uzywanie json-mode  wieksza poprawność formatu zwracanych odpowiedzi. Z tego powodu, w mozliwych przypadkach sprobujemy uzywac tej metody.