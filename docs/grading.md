# Zasady oceniania

W ramach przedmiotu można uzyskać maksymalnie **100 punktów**:

| Element | Punkty |
|---|---:|
| Kolokwium | 30 |
| Projekt | 70 |
| **Razem** | **100** |

## Kolokwium – 30 punktów

Kolokwium sprawdza znajomość i rozumienie ogólnych pojęć, mechanizmów i zasad związanych z tworzeniem agentów AI.

Kolokwium nie sprawdza znajomości składni języka C#, API ani szczegółów konkretnego frameworka lub biblioteki wykorzystywanej podczas zajęć.

Do zaliczenia kolokwium wymagane jest uzyskanie co najmniej **15 punktów**.

## Projekt – 70 punktów

Projekt jest realizowany zespołowo i rozwijany etapowo przez cały semestr.

| Etap | Zakres | Punkty |
|---|---|---:|
| 1. Wizja projektu | problem, użytkownicy, scenariusze, rola agenta, zakres | 5 |
| 2. Agent i model | działający agent, dobór modelu, instrukcje i kontekst | 5 |
| 3. Tool calling | wykorzystanie narzędzi przez agenta | 10 |
| 4. MCP | integracja z wykorzystaniem Model Context Protocol | 10 |
| 5. RAG | wykorzystanie zewnętrznej wiedzy przez agenta | 10 |
| 6. Integracja rozwiązania | spójne połączenie elementów projektu | 10 |
| 7. Ewaluacja | ocena jakości działania agenta | 10 |
| 8. Prezentacja i obrona | demonstracja rozwiązania i znajomość projektu | 10 |
| **Razem** | | **70** |

### 1. Wizja projektu – 5 pkt

Zespół przedstawia własny pomysł na projekt w postaci krótkiej wizji.

Oceniane są:

- jasno określony problem,
- wskazanie użytkowników,
- najważniejsze scenariusze użycia,
- uzasadnienie roli agenta AI,
- realistyczny zakres projektu.

Wizja projektu wymaga zatwierdzenia przez prowadzącego.

### 2. Agent i model – 5 pkt

Zespół przygotowuje pierwszą działającą wersję agenta.

Oceniane są:

- uruchomienie agenta,
- świadomy dobór modelu,
- przygotowanie instrukcji agenta,
- wykorzystanie odpowiedniego kontekstu,
- umiejętność uzasadnienia przyjętych rozwiązań.

### 3. Tool calling – 10 pkt

Agent wykorzystuje co najmniej jedno narzędzie realizujące operację wynikającą z charakteru projektu.

Oceniane są:

- poprawna implementacja narzędzia,
- poprawne przekazywanie parametrów i wyników,
- właściwe wykorzystanie narzędzia przez agenta,
- obsługa sytuacji błędnych,
- uzasadnienie zastosowania tool calling w projekcie.

### 4. MCP – 10 pkt

Projekt wykorzystuje Model Context Protocol.

Może to być własny serwer MCP lub istniejący serwer odpowiedni do charakteru projektu.

Oceniane są:

- poprawna integracja,
- sens wykorzystania MCP w projekcie,
- znajomość roli MCP w architekturze rozwiązania,
- umiejętność wyjaśnienia różnicy pomiędzy MCP a bezpośrednim udostępnieniem narzędzi agentowi.

### 5. RAG – 10 pkt

Projekt wykorzystuje mechanizm Retrieval-Augmented Generation.

Oceniane są:

- przygotowanie źródła wiedzy,
- indeksowanie lub inny mechanizm umożliwiający wyszukiwanie,
- wyszukiwanie informacji odpowiednich do zapytania,
- przekazywanie znalezionych informacji jako kontekstu modelu,
- wykorzystanie uzyskanego kontekstu podczas generowania odpowiedzi.

Technologia realizacji RAG nie jest narzucona.

### 6. Integracja rozwiązania – 10 pkt

Elementy projektu powinny tworzyć spójne rozwiązanie odpowiadające wizji projektu.

Oceniane są:

- współpraca poszczególnych elementów,
- przepływ działania agenta,
- obsługa błędów i sytuacji nietypowych,
- jakość rozwiązania jako całości,
- zgodność końcowego rozwiązania z założeniami projektu.

Tool calling, MCP i RAG powinny wynikać z charakteru projektu, a nie stanowić niezależnych demonstracji dodanych wyłącznie w celu spełnienia wymagań.

### 7. Ewaluacja – 10 pkt

Zespół powinien wykazać, że potrafi ocenić jakość działania swojego agenta.

Oceniane są:

- przygotowanie scenariuszy testowych,
- uwzględnienie przypadków typowych i problematycznych,
- sposób oceny odpowiedzi i zachowania agenta,
- przedstawienie wyników,
- identyfikacja ograniczeń rozwiązania.

Zespół powinien potrafić odpowiedzieć na pytanie:

**„Skąd wiemy, że nasz agent działa dobrze?”**

### 8. Prezentacja i obrona – 10 pkt

Na zakończenie projektu zespół prezentuje działające rozwiązanie.

Prezentacja powinna obejmować:

- krótkie przypomnienie problemu,
- demonstrację działania,
- przedstawienie architektury,
- najważniejsze decyzje projektowe,
- wyniki ewaluacji,
- ograniczenia i możliwe kierunki dalszego rozwoju.

Każdy członek zespołu powinien znać rozwiązanie i potrafić wyjaśnić wskazane przez prowadzącego elementy.

Prowadzący może poprosić studenta o:

- wyjaśnienie fragmentu kodu,
- wyjaśnienie działania wybranego mechanizmu,
- uzasadnienie decyzji projektowej,
- opisanie sposobu wykorzystania AI podczas realizacji projektu,
- zaproponowanie lub wykonanie niewielkiej modyfikacji rozwiązania.

Punkty za projekt mogą zostać zróżnicowane pomiędzy członków zespołu na podstawie ich udziału w realizacji oraz indywidualnej znajomości rozwiązania.

## Ocena końcowa

Warunkiem zaliczenia przedmiotu jest:

- spełnienie wymagań dotyczących obecności,
- uzyskanie co najmniej **15 z 30 punktów z kolokwium**,
- zaliczenie projektu,
- uzyskanie łącznie co najmniej **51 punktów**.

| Punkty | Ocena |
|---:|:---:|
| 0–50 | 2,0 |
| 51–60 | 3,0 |
| 61–70 | 3,5 |
| 71–80 | 4,0 |
| 81–90 | 4,5 |
| 91–100 | 5,0 |

## Korzystanie z AI

Podczas realizacji projektu korzystanie z narzędzi AI jest dozwolone.

Student odpowiada za przedstawiane rozwiązanie niezależnie od tego, czy jego elementy zostały przygotowane samodzielnie, z pomocą narzędzi AI, czy w inny dozwolony sposób.

**Oceniany jest efekt pracy oraz rozumienie przedstawionego rozwiązania.**

Brak umiejętności wyjaśnienia rozwiązania lub uzasadnienia podjętych decyzji może skutkować obniżeniem punktacji projektu.