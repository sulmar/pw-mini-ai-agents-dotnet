# Zestawienie raportów — Laboratorium 03

Zadanie: ekstrakcja z notatek kierowców pól `floor` (piętro) i `elevator` (czy jest winda) do JSON.

## Oddane raporty

Zespoły projektowe według [projects/readme.md](../../../../projects/readme.md). `x` oznacza, że zespół oddał raport z lab03.

| Grupa | Zespół | Studenci | Raport z modeli | Link |
|---|---|---|---|---|
| 1 | Lemat | Kacper Piotrowski, Igor Ptak | x | [raport-piotrowski-ptak.pdf](raport-piotrowski-ptak.pdf) |
| 1 | DuDi | Barbara Gawlik, Jakub Gronkiewicz, Aleksandra Dmitruk | | |
| 1, 2 | ai-rental-assistant | Michał Chrostowski, Bartosz Dźwigała, Tomasz Stolarczyk | x | [raport-mchrostowski-bdzwigala-tstolarczyk.md](raport-mchrostowski-bdzwigala-tstolarczyk.md) |
| 2 | MealBrAIn | Patrycja Borkowska, Maria Deperas, Oliwia Partyka | x | [raport-borkowska-deperas-partyka.md](raport-borkowska-deperas-partyka.md) |
| 2, 3 | JuniorQuest | Martyna Budrewicz, Paweł Nowak | | |
| 2 | cinema-ai-assistant | Natalia Bakalarska, Zofia Cymerys, Helena Gala | x | [raport-bakalarska-cymerys-gala.md](raport-bakalarska-cymerys-gala.md) |
| 2, 4 | TrAIningRound | Jan Kaszycki, Wiktor Osior, Piotr Wójcik | x | [raport-kaszycki-osior/report.md](raport-kaszycki-osior/report.md) (w raporcie tylko Kaszycki i Osior) |
| 2, 3 | Graphora | Anna Granos, Ulyana Frolova | x | [raport-frolova-granos.md](raport-frolova-granos.md) |
| 2 | steam-ai-advisor | Patryk Chachulski, Jakub Jaworski | x | [raport-jaworski-chachulski.md](raport-jaworski-chachulski.md) |
| 3 | PaperPulse | Kazimierz Krześniak, Mateusz Kuprewicz | częściowo | Krześniak w [raport-MMK.md](raport-MMK.md); brak raportu Kuprewicza |
| 3 | Greedy Student | Daniel Nowakowski, Michał Mueller, Jakub Motyka, Jakub Mendel | częściowo | Mueller i Motyka w [raport-MMK.md](raport-MMK.md); brak raportu Nowakowskiego i Mendla |
| 3 | czytAI-i-sluchAI-ai | Michał Obrębski, Tymon Piwowarski | | |
| 3 | SimWall | Mateusz Rabantek, Karol Wójcik, Julian Sowiński | x | [raport-wojcik-rabantek-sowinski.md](raport-wojcik-rabantek-sowinski.md) |
| 3 | Hair | Michał Mueller, Jakub Motyka, Kazimierz Krześniak | x | [raport-MMK.md](raport-MMK.md) |
| 3 | LoreKeeper | Maciej Mazurek, Ewelina Karpińska | x | [raport-karpinska-mazurek.md](raport-karpinska-mazurek.md) |
| 4 | AlignRoute | Kamila Wachulec, Mateusz Rutkowski, Konrad Typa, Ziemowit Jakuczun | | |
| 4 | Coffer | Bartłomiej Sokołowski, Konstanty Sobczyński, Krzysztof Wójtowicz, Krzysztof Wojczakowski | | |
| 4 | calendarAI | Mikołaj Zawrotniak, Maciej Wełpa, Jędrzej Kwaśny | x | [raport-calendarai.md](raport-calendarai.md) (skład niepodany w raporcie) |
| ? | — (brak w projects/readme.md) | Igor Kowalik, Aleksander Sergiej, Wiktor Szymonek | x | [raport-ikowalik-wszymonek-asergiej.md](raport-ikowalik-wszymonek-asergiej.md) |
| ? | — (brak w projects/readme.md) | Hubert Zalewski, Paweł Zakrzewski | x | [raport-zalewski-zakrzewski.pdf](raport-zalewski-zakrzewski.pdf) |
| ? | — (brak w projects/readme.md) | Wiktoria Postek, Aleksandra Sitkowska, Konrad Niesyt, Aleksandra Choszczyk | x | [raport-postek-sitkowska-niesyt-choszczyk.md](raport-postek-sitkowska-niesyt-choszczyk.md) |
| ? | — (brak w projects/readme.md) | Adam Bagiński | x | [raport-baginski.md](raport-baginski.md) |

Podsumowanie: z 18 zespołów projektowych raport oddało 11. Dwa kolejne (PaperPulse, Greedy Student) są pokryte tylko częściowo, przez członków zespołu Hair. Brak raportu: DuDi, JuniorQuest, czytAI-i-sluchAI-ai, AlignRoute, Coffer. Do tego są 4 raporty od zespołów, których nie ma jeszcze w `projects/readme.md`.

## Zespoły

| Zespół | Raport | Środowisko | Modele |
|---|---|---|---|
| Ewelina Karpińska, Maciej Mazurek | [raport-karpinska-mazurek.md](raport-karpinska-mazurek.md) | Ollama 0.40.9, ARM, 32 GB RAM (CPU) | llama3.2:1b, qwen2.5:1.5b, gemma2:2b |
| Michał Mueller, Kazimierz Krześniak, Jakub Motyka | [raport-MMK.md](raport-MMK.md) | API PCSS | Bielik 11B, DeepSeek-V3.1, DeepSeek-V4-Flash, Qwen3.5-397B-A17B |
| Igor Kowalik, Aleksander Sergiej, Wiktor Szymonek | [raport-ikowalik-wszymonek-asergiej.md](raport-ikowalik-wszymonek-asergiej.md) | OpenRouter | Xiaomi mimo-v2.6-flash, Mistral large-4.0 |
| Hubert Zalewski, Paweł Zakrzewski | [raport-zalewski-zakrzewski.pdf](raport-zalewski-zakrzewski.pdf) | Ollama 0.40.13, i5-12400F + RTX 3070, 16 GB RAM | gemma3:4b, llama3.2:3b, qwen3:4b |
| Wiktoria Postek, Aleksandra Sitkowska, Konrad Niesyt, Aleksandra Choszczyk | [raport-postek-sitkowska-niesyt-choszczyk.md](raport-postek-sitkowska-niesyt-choszczyk.md) | API PCSS | DeepSeek-V3.1, Bielik 11B |
| Kacper Piotrowski G., Igor Ptak | [raport-piotrowski-ptak.pdf](raport-piotrowski-ptak.pdf) | API PCSS, własny skrypt w Pythonie | DeepSeek-V4-Flash, GLM-5.3-Flash |
| Adam Bagiński | [raport-baginski.md](raport-baginski.md) | Ollama 0.40.8, Intel Core Ultra 9 185H, 31 GB RAM + OpenAI API | gemma3:4b, qwen3:4b, GPT-6 Luna |
| Natalia Bakalarska, Zofia Cymerys, Helena Gala (cinema-ai-assistant) | [raport-bakalarska-cymerys-gala.md](raport-bakalarska-cymerys-gala.md) | Ollama 0.40.12, Core Ultra 9 275HX + RTX 5070 Ti 12 GB, 64 GB RAM | gemma3:4b, mistral, qwen3:4b |
| Michał Chrostowski, Bartosz Dźwigała, Tomasz Stolarczyk (ai-rental-assistant) | [raport-mchrostowski-bdzwigala-tstolarczyk.md](raport-mchrostowski-bdzwigala-tstolarczyk.md) | Ollama 0.40.12, i7-9700F + GTX 1660 Ti 6 GB, 16 GB RAM + API PCSS | gemma3:4b, qwen3:4b, deepseek-r1:8b, DeepSeek-V4-Flash, GLM-5.3-Flash, Mistral-Small-3.2:24b |
| Patrycja Borkowska, Maria Deperas, Oliwia Partyka (MealBrAIn) | [raport-borkowska-deperas-partyka.md](raport-borkowska-deperas-partyka.md) | Ollama na 3 komputerach: K1 RTX 5070 Ti / 64 GB, K2 Iris Xe / 16 GB, K3 Radeon 780M / 16 GB | gemma3:4b, qwen3:4b (K1), smollm2:1.7b, granite3.3:2b (K2), phi4-mini, llama3.2 (K3) |
| Ulyana Frolova, Anna Granos (Graphora) | [raport-frolova-granos.md](raport-frolova-granos.md) | Ollama 0.40.8, Apple M3, 16 GB RAM | gemma3:4b, qwen3:4b |
| Karol Wójcik, Mateusz Rabantek, Julian Sowiński (SimWall) | [raport-wojcik-rabantek-sowinski.md](raport-wojcik-rabantek-sowinski.md) | Ollama: Apple M2 16 GB oraz PC z GPU 16 GB VRAM; Gemini API | qwen3:4b, deepseek-r1, gemma4:12b, gemini-3.5-flash-lite, gemini-3.8-flash |
| Jakub Jaworski, Patryk Chachulski (steam-ai-advisor) | [raport-jaworski-chachulski.md](raport-jaworski-chachulski.md) | Ollama, Ryzen 5 7535HS + Radeon 660M (iGPU), 32 GB RAM | gemma3:4b, qwen3:4b, llama3.2:3b |
| calendarAI (skład niepodany w raporcie; commit: Mikołaj Zawrotniak) | [raport-calendarai.md](raport-calendarai.md) | Ollama, Apple M3, 16 GB RAM | llama3.2:3b, qwen2.5:3b, deepseek-r1:7b |
| Jan Kaszycki, Wiktor Osior (TrAIningRound) | [raport-kaszycki-osior/report.md](raport-kaszycki-osior/report.md) | Ollama 0.40.3, i7-9750HF + GTX 1660 Ti 6 GB, 31 GB RAM + zdalny llama.cpp | **inne zadanie** — patrz [sekcja niżej](#traininground--wariant-z-własnym-zadaniem) |

Większość zespołów testowała na 10 notatkach z `examples.csv` — tam różnica 10 p.p. to jedna notatka. Dwa zespoły (Postek i in., Piotrowski/Ptak) użyły pełnego zbioru 172 notatek (39 unikalnych treści, 103 rekordy bez żadnej informacji).

## EXP-01 — Porównanie modeli

| Zespół | Model | Floor | Elevator | Exact match | Średni czas [ms] |
|---|---|---|---|---|---|
| Karpińska/Mazurek | llama3.2:1b | 50% | 30% | 20% | 1664 |
| Karpińska/Mazurek | qwen2.5:1.5b | 70% | 30% | 20% | 1324 |
| Karpińska/Mazurek | gemma2:2b | 80% | 70% | 60% | 4010 |
| MealBrAIn (K2) | smollm2:1.7b | 60% | 30% | 20% | 2187 |
| MealBrAIn (K2) | granite3.3:2b | 60% | 50% | 50% | 1691 |
| MealBrAIn (K3) | phi4-mini | 80% | 90% | 70% | 2168 |
| MealBrAIn (K3) | llama3.2 | 80% | 50% | 40% | 1715 |
| Zalewski/Zakrzewski | llama3.2:3b | 80% | 50% | 40% | 136 |
| Jaworski/Chachulski | llama3.2:3b | 50% | 40% | 20% | 1470 |
| calendarAI | llama3.2:3b | 60% | 50% | 30% | 663 |
| calendarAI | qwen2.5:3b | 60% | 60% | 40% | 767 |
| Zalewski/Zakrzewski | gemma3:4b | 80% | 70% | 60% | 501 |
| Bagiński | gemma3:4b | 0%¹ | 0%¹ | 0%¹ | 4929 |
| Bakalarska/Cymerys/Gala | gemma3:4b | 90% | 80% | 70% | 940 |
| Chrostowski/Dźwigała/Stolarczyk | gemma3:4b | 90% | 70% | 60% | 1070 |
| MealBrAIn (K1) | gemma3:4b | 90% | 80% | 70% | 2309 |
| Frolova/Granos | gemma3:4b | 80% | 50% | 40% | 1862 |
| Jaworski/Chachulski | gemma3:4b | 0%¹ | 0%¹ | 0%¹ | 3720 |
| Zalewski/Zakrzewski | **qwen3:4b** | **100%** | **100%** | **100%** | 8519 |
| Bagiński | **qwen3:4b** | **100%** | **100%** | **100%** | 78243 |
| Bakalarska/Cymerys/Gala | **qwen3:4b** | **100%** | **100%** | **100%** | 21224 |
| Chrostowski/Dźwigała/Stolarczyk | **qwen3:4b** | **100%** | **100%** | **100%** | 13758 |
| MealBrAIn (K1) | **qwen3:4b** | **100%** | **100%** | **100%** | 111451 |
| Frolova/Granos | qwen3:4b | 90% | 100% | 90% | 63766 |
| Wójcik/Rabantek/Sowiński | **qwen3:4b** | **100%** | **100%** | **100%** | 37762 |
| Jaworski/Chachulski | **qwen3:4b** | **100%** | **100%** | **100%** | 59239 |
| Bakalarska/Cymerys/Gala | mistral | 70% | 70% | 50% | 12165 |
| calendarAI | deepseek-r1:7b | 0%¹ | 0%¹ | 0%¹ | 13732 |
| Chrostowski/Dźwigała/Stolarczyk | deepseek-r1:8b | 100% | 60% | 60% | 30586 |
| Wójcik/Rabantek/Sowiński | deepseek-r1 | 60% | 60% | 50% | 39381 |
| Wójcik/Rabantek/Sowiński | **gemma4:12b** (GPU 16 GB) | **100%** | **100%** | **100%** | 1990 |
| MMK | Bielik 11B | 20% | 30% | 20% | 1998 |
| Postek i in. (172) | Bielik 11B | 13% | 13% | 13% | 509 |
| Chrostowski/Dźwigała/Stolarczyk | Mistral-Small-3.2:24b (PCSS) | 90% | 80% | 70% | 559 |
| MMK | DeepSeek-V4-Flash | 70% | 70% | 60% | 213 |
| Chrostowski/Dźwigała/Stolarczyk | DeepSeek-V4-Flash | 80% | 80% | 60% | 257 |
| Piotrowski/Ptak (172)² | DeepSeek-V4-Flash | 99,4% | 100% | 99,4% | ~560 |
| MMK | DeepSeek-V3.1 | 100% | 80% | 80% | 683–953 |
| Postek i in. (172) | DeepSeek-V3.1 | 78% | 91% | 78% | 339 |
| Chrostowski/Dźwigała/Stolarczyk | **GLM-5.3-Flash** | **100%** | **100%** | **100%** | 1346 |
| Piotrowski/Ptak (172)² | **GLM-5.3-Flash** | **100%** | **100%** | **100%** | ~1160 |
| MMK | Qwen3.5-397B-A17B | 100% | 90% | 90% | 12734 |
| Kowalik/Sergiej/Szymonek | Xiaomi mimo-v2.6-flash | 40% | 40% | 40% | 15486 |
| Kowalik/Sergiej/Szymonek | Mistral large-4.0 | 80% | 70% | 70% | 11360 |
| Wójcik/Rabantek/Sowiński | gemini-3.5-flash-lite | 100% | 80% | 80% | 594 |
| Wójcik/Rabantek/Sowiński | gemini-3.8-flash | 100% | 90% | 90% | 38241 |
| Bagiński | GPT-6 Luna | 100% | 90% | 90% | 1975 |

¹ Model opakowywał JSON w markdown (albo, przy deepseek-r1, dodawał rozumowanie) — parser odrzucił wszystkie odpowiedzi.
² Inna konfiguracja niż u pozostałych: few-shot + tryb JSON + szczegółowe reguły w prompcie systemowym.

Uwaga: zespoły różnie liczyły niepoprawny JSON — część uznawała go za błędną odpowiedź (Kowalik/Sergiej/Szymonek, Zalewski/Zakrzewski, Bagiński, Postek i in., Jaworski/Chachulski, Frolova/Granos), a część obsługiwała blok markdown w parserze (np. Chrostowski/Dźwigała/Stolarczyk). Dlatego ta sama gemma3:4b ma u różnych zespołów 0% albo 60–70%. U Postek i in. Bielik zwrócił poprawny JSON tylko w 22/172 przypadkach, DeepSeek-V3.1 w 158/172.

## EXP-02 — Prompting (exact match)

| Zespół | Model | zero-shot | one-shot | few-shot |
|---|---|---|---|---|
| Karpińska/Mazurek | llama3.2:1b | 20% | 10% | 20% |
| Karpińska/Mazurek | qwen2.5:1.5b | 20% | 10% | 10% |
| Karpińska/Mazurek | gemma2:2b | 60% | 60% | **80%** |
| MealBrAIn (K2) | granite3.3:2b | 50% | **60%** | 40% |
| MealBrAIn (K3) | phi4-mini | **70%** | 60% | 40% |
| Zalewski/Zakrzewski | llama3.2:3b | 40% | **50%** | 20% |
| Jaworski/Chachulski | llama3.2:3b | 20% | **50%** | 40% |
| calendarAI | llama3.2:3b | 30% | — | **40%** |
| calendarAI | qwen2.5:3b | 40% | — | **50%** |
| Zalewski/Zakrzewski | gemma3:4b | 60% | **90%** | 70% |
| Bakalarska/Cymerys/Gala | gemma3:4b | 70% | **90%** | 70% |
| Chrostowski/Dźwigała/Stolarczyk | gemma3:4b | 60% | **80%** | 70% |
| Frolova/Granos | gemma3:4b | 40% | **80%** | 70% |
| Bagiński | gemma3:4b | 0%¹ | 0%¹ | 0%¹ |
| Jaworski/Chachulski | gemma3:4b | 0%¹ | 0%¹ | 0%¹ |
| Zalewski/Zakrzewski | qwen3:4b | 100% | 100% | 100% |
| Bakalarska/Cymerys/Gala | qwen3:4b | 100% | 100% | 100% |
| Chrostowski/Dźwigała/Stolarczyk | qwen3:4b | 100% | 100% | 100% |
| MealBrAIn (K1)³ | — | 100% | 100% | 100% |
| Jaworski/Chachulski | qwen3:4b | 100% | — | 100% |
| Bakalarska/Cymerys/Gala | mistral | 50% | **70%** | 60% |
| Wójcik/Rabantek/Sowiński | deepseek-r1 | 50% | **70%** | **70%** |
| Wójcik/Rabantek/Sowiński | gemma4:12b | 100% | 100% | 100% |
| Chrostowski/Dźwigała/Stolarczyk | Mistral-Small-3.2:24b | 70% | 70% | **80%** |
| MMK | DeepSeek-V4-Flash | 57% | **75%** | **75%** |
| Chrostowski/Dźwigała/Stolarczyk | DeepSeek-V4-Flash | 60% | 70% | **80%** |
| Piotrowski/Ptak (172) | DeepSeek-V4-Flash | **100%** | 98,8% | 99,4% |
| Postek i in. (172) | DeepSeek-V3.1 | 78% | 82% | **85%** |
| Chrostowski/Dźwigała/Stolarczyk | GLM-5.3-Flash | 100% | 100% | 100% |
| Piotrowski/Ptak (172) | GLM-5.3-Flash | 100% | 100% | 100% |
| Kowalik/Sergiej/Szymonek | Mistral large-4.0 | 90% | **100%** | **100%** |
| Wójcik/Rabantek/Sowiński | gemini-3.5-flash-lite | 80% | 80% | 80% |
| Bagiński | GPT-6 Luna | 90% | 90% | **100%** |

³ Raport nie podaje, który model z K1 badano w EXP-02–04; wyniki 100% odpowiadają qwen3:4b z EXP-01.
„—” — wariant nie został wykonany.

## EXP-03 — Temperature (exact match)

| Zespół | Model | 0.0 | 0.5 | 1.0 |
|---|---|---|---|---|
| Karpińska/Mazurek | llama3.2:1b | 20% | 36% | 26% |
| MealBrAIn (K2) | granite3.3:2b | 50% | 50% | 52% |
| MealBrAIn (K3) | phi4-mini | **70%** | 68% | 64% |
| Zalewski/Zakrzewski | llama3.2:3b | 40% | 40% | 28% |
| Jaworski/Chachulski | llama3.2:3b | 20% | 24% | 32% |
| calendarAI | llama3.2:3b | 30% | 24% | 36% |
| calendarAI | qwen2.5:3b | **40%** | 38% | 30% |
| Zalewski/Zakrzewski | gemma3:4b | 60% | 60% | 62% |
| Bakalarska/Cymerys/Gala | gemma3:4b | 70% | 70% | 68% |
| Chrostowski/Dźwigała/Stolarczyk | gemma3:4b | 60% | 60% | 66% |
| Frolova/Granos | gemma3:4b | 40% | 42% | 52% |
| Bagiński | gemma3:4b | 0%¹ | 0%¹ | 0%¹ |
| Jaworski/Chachulski | gemma3:4b | 0%¹ | 0%¹ | 0%¹ |
| Zalewski/Zakrzewski | qwen3:4b | 100% | 100% | 100% |
| Bakalarska/Cymerys/Gala | qwen3:4b | 100% | 100% | 100% |
| MealBrAIn (K1)³ | — | 100% | 100% | 100% |
| Bakalarska/Cymerys/Gala | mistral | **50%** | 36% | 30% |
| Wójcik/Rabantek/Sowiński⁴ | gemma4:12b | 100% | 100% | 100% |
| Chrostowski/Dźwigała/Stolarczyk | Mistral-Small-3.2:24b | **70%** | 64% | 64% |
| MMK | DeepSeek-V4-Flash | 62% | 67% | 61% |
| Chrostowski/Dźwigała/Stolarczyk | DeepSeek-V4-Flash | 60% | 68% | 72% |
| Piotrowski/Ptak (172) | DeepSeek-V4-Flash | 99,4% | 99,4% | 98,3% |
| Postek i in. (172) | DeepSeek-V3.1 | 78% | **80%** | 76% |
| Chrostowski/Dźwigała/Stolarczyk | GLM-5.3-Flash | 100% | 100% | 100% |
| Piotrowski/Ptak (172) | GLM-5.3-Flash | 100% | 100% | 100% |
| Kowalik/Sergiej/Szymonek | Mistral large-4.0 | **84%** | 78% | 64% |
| Wójcik/Rabantek/Sowiński⁴ | gemini-3.5-flash-lite | 80% | 80% | 80% |
| Bagiński | GPT-6 Luna | 90% | 92% | 92% |

⁴ Środkowa wartość to 0.2 zamiast 0.5.

Przy 0.0 wyniki są (prawie) powtarzalne we wszystkich zespołach; przy 1.0 rośnie wariancja i pojawiają się niepoprawne JSON-y. Najwyraźniej widać to w dwóch raportach:
- u Postek i in. powtarzalne notatki spadają ze 171/172 do 134/172 i 109/172;
- u Bakalarskiej/Cymerys/Gali mistral spada z 10/10 do 2/10 i 3/10.

Bagiński zauważył, że nawet T=0 nie gwarantuje pełnego determinizmu (NOTE-010 dawała raz `false`, raz `null`). GLM-5.3-Flash, qwen3:4b, gemma4:12b i gemini-3.5-flash-lite były w pełni powtarzalne nawet przy T=1.0. Pojedyncze „zyski” przy wyższej temperaturze (Frolova/Granos, Chrostowski i in., Jaworski/Chachulski) zespoły same tłumaczą jako przypadkowe trafienie niepewnej notatki, np. NOTE-007.

## EXP-04 — Structured output

| Zespół | Model | Poprawny JSON: prompt → json-schema | Exact match: prompt → json-schema |
|---|---|---|---|
| Karpińska/Mazurek | qwen2.5:1.5b | 0/10 → 10/10 | 20% → 20% |
| Karpińska/Mazurek | gemma2:2b | 0/10 → 10/10 | 60% → 60% |
| MealBrAIn (K2) | granite3.3:2b | 10/10 → 10/10 | 50% → 50% |
| MealBrAIn (K3) | phi4-mini | 10/10 → 10/10 | 70% → 60% |
| Zalewski/Zakrzewski | llama3.2:3b | 10/10 → 10/10 | 40% → 40% |
| Jaworski/Chachulski | llama3.2:3b | 8/10 → 10/10 | 20% → 30% |
| calendarAI | llama3.2:3b | 9/10 → 10/10 | 30% → 30% |
| calendarAI | qwen2.5:3b | 10/10 → 10/10 | 40% → 40% |
| Zalewski/Zakrzewski | gemma3:4b | 10/10 → 10/10 | 60% → 70% |
| Bagiński | gemma3:4b | 0/10 → 10/10 | 0% → **70%** |
| Jaworski/Chachulski | gemma3:4b | 0/10 → 10/10 | 0% → **60%** |
| Bakalarska/Cymerys/Gala | gemma3:4b | 10/10 → 10/10 | 70% → 70% |
| Chrostowski/Dźwigała/Stolarczyk | gemma3:4b | 10/10 → 10/10 | 60% → 70% |
| Frolova/Granos | gemma3:4b | 10/10 → 10/10 | 40% → 50% |
| Zalewski/Zakrzewski | qwen3:4b | 10/10 → 10/10 | 100% → 100% |
| Bakalarska/Cymerys/Gala | qwen3:4b | 10/10 → 10/10 | 100% → 100% |
| MealBrAIn (K1)³ | — | 10/10 → 10/10 | 100% → 100% |
| Bakalarska/Cymerys/Gala | mistral | 10/10 → 10/10 | 70% → 50% |
| Chrostowski/Dźwigała/Stolarczyk | deepseek-r1:8b | 10/10 → 10/10 | 60% → 60% |
| Wójcik/Rabantek/Sowiński | gemma4:12b | 0/10 → 10/10 | 0% → **100%** |
| Chrostowski/Dźwigała/Stolarczyk | Mistral-Small-3.2:24b | 10/10 → 10/10 | 70% → 70% |
| MMK | DeepSeek-V4-Flash | 9/10 → 10/10 | 70% → 70% |
| Chrostowski/Dźwigała/Stolarczyk | DeepSeek-V4-Flash | 10/10 → 10/10 | 60% → 60% |
| Piotrowski/Ptak (172) | DeepSeek-V4-Flash | 100% → 100% | 99,4% → 99,4% |
| Postek i in. (172)⁵ | DeepSeek-V3.1 | 158/172 → 171/172 | 78% → **85%** |
| Chrostowski/Dźwigała/Stolarczyk | GLM-5.3-Flash | 10/10 → 10/10 | 100% → 100% |
| Piotrowski/Ptak (172) | GLM-5.3-Flash | 100% → 100% | 100% → 100% |
| Kowalik/Sergiej/Szymonek | Mistral large-4.0 | 70% → 100% | 70% → 100% |
| Wójcik/Rabantek/Sowiński | gemini-3.5-flash-lite | 9/10 → 10/10 | 70% → 80% |
| Bagiński | GPT-6 Luna | 10/10 → 10/10 | 100% → 90% |

⁵ json-mode (`response_format: json_object`) zamiast json-schema.

## Najtrudniejsze notatki (powtarzają się w raportach)

- **„Winda nie działa, dostawa na 5p”** (NOTE-010) to najczęściej zgłaszany problem, opisuje go prawie każdy zespół. Modele zwracają `elevator=false` lub `null`, bo mylą stan techniczny z obecnością windy. Mylą się nawet GPT-6 Luna i modele Gemini. Według Chrostowskiego i in. 4 z 6 modeli dawały tu `false`.
- **„4 piętro, winda tylko do 3”** (NOTE-007): winda istnieje, ale nie dojeżdża na docelowe piętro, więc model odpowiada `elevator=false`. Zgłosiło to 7 zespołów. Przy wyższej temperaturze odpowiedź zaczyna się losowo zmieniać.
- **„Mieszkanie 15, domofon 15”** (NOTE-006): numer mieszkania traktowany jako piętro (`floor=15`). Zgłosiło to 6 zespołów. U Frolovej/Granos ten błąd wystąpił we wszystkich eksperymentach, nawet u qwen3:4b. Pokrewne pomyłki:
  - „IV piętro, lokal 12” → `floor=12` (granite3.3:2b);
  - „2 PIĘTRO 13kluczyk0000” → `floor=3` przy T=1.0 (Piotrowski/Ptak).
- **„Drugie lub trzecie piętro”** (NOTE-009): model wybiera jedną wartość zamiast `null` albo dopowiada `elevator=false` (7 zespołów). Few-shot potrafi to pogorszyć, bo wszystkie przykłady mają jednoznaczne piętro (Frolova/Granos).
- **Brak informacji o windzie** (NOTE-003/004/009): model „dopowiada” `false` zamiast `null`. Według Frolovej/Granos wystarcza już jeden przykład z `elevator: null`.
- **Liczebniki słowne i rzymskie** („Trzecie piętro”, „IV piętro”): DeepSeek-V4-Flash zwracał piętro o jeden niżej (Chrostowski i in.).
- **„Winda na końcu korytarza”** (NOTE-005): model zgaduje piętro (`floor=0`) i pomija windę (MealBrAIn).
- **„Samochód z windą”**: winda w pojeździe, nie w budynku. Bez jawnej reguły model zwraca `true` albo `false` zamiast `null` (Piotrowski/Ptak).
- **Kilka pięter / parter** („dostawy na 3, 2, 1 piętro oraz na -1”, „parter i I piętro”): zespoły przyjęły **różne wzorce**. Postek i in. oczekiwali pierwszego piętra (3 / 0), Piotrowski/Ptak wartości `null`. Przed oceną modelu trzeba więc jednoznacznie ustalić reguły etykietowania.
- **Format:** odpowiedź opakowana w ```` ```json ```` łamie parser. Dotyczy to gemma2/gemma3/gemma4, qwen2.5, Bielika, DeepSeek-V3.1 i deepseek-r1 (rozumowanie przed JSON-em).

## Wspólne wnioski

1. **json-schema / json-mode rozwiązuje problem formatu**: u każdego zespołu dał ~100% poprawnych JSON-ów. Trafność rośnie tylko tam, gdzie wcześniej JSON był liczony jako błąd:
   - gemma4:12b: 0% → 100%;
   - gemma3:4b: 0% → 60–70%;
   - DeepSeek-V3.1: 78% → 85%.

   Dla modeli, które już zwracają czysty JSON, schemat nie poprawia, a czasem lekko obniża trafność (mistral 70% → 50%, phi4-mini 70% → 60%, Luna 100% → 90%). Daje za to gwarancję formatu zamiast samego wysokiego prawdopodobieństwa. **Poprawny JSON ≠ poprawna odpowiedź** — ten wniosek pojawia się niemal w każdym raporcie.
2. **One-shot to najczęściej najlepszy wariant dla małych modeli.** gemma3:4b zyskała 20–40 p.p. u czterech zespołów (np. 40% → 80% u Frolovej/Granos, 70% → 90% u Bakalarskiej i in.). Few-shot bywa gorszy od one-shot, a dla phi4-mini nawet od zero-shot. Frolova/Granos wyjaśniają dlaczego: przykłady naprawiają tylko te typy błędów, które pokazują, a jednorodne przykłady uczą zgadywania. Piotrowski/Ptak pokazali, że **przy szczegółowych regułach w prompcie systemowym zero-shot jest już bezbłędny**. Jakość instrukcji > liczba przykładów.
3. **Temperatura 0.0** to bezpieczny wybór do ekstrakcji: najwyższa powtarzalność, podobna lub lepsza trafność. Pojedyncze „zyski” przy 0.5–1.0 to 1–2 notatki kosztem spadku powtarzalności. Powtarzalność nie gwarantuje jednak poprawności: przy T=0 model konsekwentnie popełnia te same błędy (MealBrAIn, Frolova/Granos). Plus jest taki, że da się je zdiagnozować i poprawić promptem.
4. **Wielkość modelu nie przesądza**:
   - **qwen3:4b** (tryb myślenia) osiągnął 100% u 7 z 8 zespołów, które go testowały (90% u Frolovej/Granos). Ceną jest czas: od 8,5 s (RTX 3070) do 111 s na notatkę (zależnie od sprzętu).
   - **deepseek-r1** (7b/8b) wypadł słabo (0–60%) mimo większego rozmiaru.
   - **Bielik 11B** przez API dał 13–20% u dwóch zespołów, głównie przez zły format.
5. **Najlepszy kompromis jakość/czas:**
   - **GLM-5.3-Flash** (PCSS): 100% u dwóch niezależnych zespołów przy ~1,2 s, pełna powtarzalność nawet przy T=1.0;
   - lokalnie **gemma4:12b** na GPU z 16 GB VRAM: 100% w ~2 s (po włączeniu json-schema).
6. Modele najczęściej zgadują zamiast zwrócić `null`, a liczby w notatce (numer mieszkania, lokalu, kod domofonu) biorą za piętro. Warto pokazać to jawnie w regułach albo przykładach.
7. **Mały zbiór testowy ogranicza wnioski**: różnice 1–3 notatek mogą się odwrócić na innym zbiorze. Reguły pisane ze znajomością danych testowych dają optymistyczne wyniki (Piotrowski/Ptak, Bagiński, Frolova/Granos).

## Rekomendacje zespołów

| Zespół | Rekomendacja |
|---|---|
| Karpińska/Mazurek | gemma2:2b + few-shot + json-schema, temperatura 0.0 |
| MMK | DeepSeek-V3.1 |
| Kowalik/Sergiej/Szymonek | Mistral large-4.0 + json-schema, temperatura 0.0 (opcjonalnie few-shot) |
| Zalewski/Zakrzewski | qwen3:4b + zero-shot + json-schema, temperatura 0; dla szybkości gemma3:4b + one-shot |
| Postek i in. | DeepSeek-V3.1 + one-shot, temperatura 0, json-mode gdy dostępny |
| Piotrowski/Ptak | DeepSeek-V4-Flash + zero-shot ze szczegółowymi regułami + json-schema, temperatura 0; GLM-5.3-Flash gdy liczy się tylko dokładność |
| Bagiński | GPT-6 Luna + few-shot, temperatura 0; qwen3:4b jako wariant lokalny |
| Bakalarska/Cymerys/Gala | qwen3:4b tam, gdzie liczy się precyzja, a nie czas; gemma3:4b do większości zastosowań; mistral odrzucony |
| Chrostowski/Dźwigała/Stolarczyk | GLM-5.3-Flash (PCSS) + few-shot z przypadkami brzegowymi + json-schema, temperatura 0; zapas: Mistral-Small-3.2:24b, lokalnie qwen3:4b |
| MealBrAIn | brak sekcji z rekomendacją w raporcie |
| Frolova/Granos | gemma3:4b + one-shot (lub few-shot z trudnymi przypadkami) + json-schema, temperatura 0 |
| Wójcik/Rabantek/Sowiński | lokalnie gemma4:12b + json-schema; gemini-3.5-flash-lite jako darmowa opcja zdalna |
| Jaworski/Chachulski | gemma3:4b (sekcja rekomendacji w raporcie urwana) |
| calendarAI | qwen2.5:3b + few-shot + json-schema, temperatura 0 (do potwierdzenia w jednym połączonym eksperymencie) |

## TrAIningRound — wariant z własnym zadaniem

Jan Kaszycki i Wiktor Osior zamiast notatek kierowców przebadali zadanie ze swojego projektu: odczyt intencji z wiadomości na czacie (`mode`, `rating_min`/`rating_max`, `topic`) na 25 wiadomościach. Dodali też własny EXP-05: układanie contestu z puli 20 zadań dla 5 grup uczestników. Pełny raport: [raport-kaszycki-osior/report.md](raport-kaszycki-osior/report.md). Szczegóły: [exp05-contesty.md](raport-kaszycki-osior/exp05-contesty.md), [opis-eksperymentow.md](raport-kaszycki-osior/opis-eksperymentow.md). Kod i logi: [WiktorOsior/algobot-ai-agents-mini](https://github.com/WiktorOsior/algobot-ai-agents-mini/tree/main/experiments/lab03-models-and-prompting).

| Model | Exact match (EXP-01) | Średni czas [ms] | Dobry contest (EXP-05) |
|---|---|---|---|
| gemma3:4b | 32% | 3741 | 0/5 |
| qwen3:4b-instruct | 72% | 1220 | 2/5 |
| qwen2.5-coder:3b | 84% | 939 | 0/5 |
| gemma4:e4b | 88% | 14968 | 2/5 |
| gemma-4-31b (zdalnie, llama.cpp) | **100%** | 9145 | **4/5** |

Wnioski zgodne z resztą grupy:
- **Prompt:** przy dobrze opisanych regułach przykłady nie pomagają. Few-shot wprowadził nowe błędy, bo model skopiował wyjątek z przykładu („Div 1” → `1900..null`) jako regułę.
- **Temperatura:** T=0 daje 84% i 25/25 powtarzalnych odpowiedzi, a T=1.0 już tylko 60% i 8/25.
- **Format:** JSON Schema nic nie zmienił dla qwen2.5-coder, ale zostaje jako zabezpieczenie.

Wniosek dodatkowy: twarde reguły (zadania już rozwiązane, zakres ratingu) trzeba sprawdzać w kodzie albo narzędziem, bo nawet duży model raz wyszedł poza zakres, a uzasadnienia modeli brzmią przekonująco, ale bywają fałszywe.

Rekomendacja zespołu: do odczytu intencji qwen2.5-coder:3b + zero-shot + json-schema, temperatura 0; do układania contestów większy model (gemma-4-31b).
