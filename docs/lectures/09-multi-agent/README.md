# Tworzenie agentów AI w .NET

## Wykład 09 — Architektura agentów AI

> Materiały w przygotowaniu.

## Opis

Wykład pokazuje, jak przejść od jednego agenta do zespołu wyspecjalizowanych agentów:

- **Od jednego agenta do zespołu** — zadanie dzielimy między role: Programistę, Testera i Dokumentalistę. Zyskujemy specjalizację i możliwość pracy równoległej.
- **Anatomia systemu wieloagentowego** — każdy agent ma własną rolę, instrukcje, narzędzia i kontekst, np. Programista tworzy kod, a Tester szuka błędów.
- **Wzorce współpracy** — sekwencja (A → B → C), praca równoległa z wynikiem scalanym na końcu oraz koordynator, który rozdziela zadania między specjalistów.
- **Komunikacja między agentami** — Koordynator przekazuje wymagania Programiście, Programista przekazuje kod Testerowi, a Tester zwraca raport z testów do Programisty.
- **Shelly Lab: programista i tester** — Programista pisze skrypt, Tester uruchamia go w symulatorze. Błędy wracają do poprawy; zaliczone testy dają gotowy skrypt, który trafia na urządzenie Shelly po potwierdzeniu.
- **Wspólny cel, różne role** — zespół agentów planuje, wykonuje i sprawdza wynik. Każdy agent odpowiada za swoją część zadania.

## Najważniejsza myśl

**Multi-agent nie zawsze oznacza lepsze rezultaty.** Rozbijanie zadania na wielu agentów ma sens przede wszystkim wtedy, gdy poszczególne zadania są od siebie niezależne i mogą być realizowane równolegle. W przeciwnym razie koszty koordynacji, komunikacji i synchronizacji mogą przewyższyć potencjalne korzyści.

**Wybór między jednym agentem a architekturą multi-agent to decyzja inżynieryjna**, która powinna wynikać z charakteru zadania, a nie z założenia, że więcej agentów automatycznie oznacza lepszą jakość czy wydajność.
