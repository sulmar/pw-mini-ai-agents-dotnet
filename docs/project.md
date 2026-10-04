# Projekt semestralny

W ramach przedmiotu zespoły realizują własny projekt wykorzystujący agentów AI w .NET.

## Własny pomysł

Temat projektu jest proponowany przez zespół. Projekt powinien rozwiązywać konkretny problem i mieć jasno określonego użytkownika lub grupę użytkowników.

Pierwszym etapem projektu jest przygotowanie krótkiej **wizji projektu**, opisującej między innymi:

- problem, który projekt ma rozwiązywać,
- użytkowników rozwiązania,
- najważniejsze scenariusze użycia,
- proponowaną rolę agenta AI,
- zakres projektu.

Wizja wymaga zatwierdzenia przez prowadzącego przed rozpoczęciem dalszej realizacji projektu.

## Minimalne wymagania

Projekt musi być zrealizowany z wykorzystaniem platformy .NET i obejmować co najmniej:

- agenta AI oraz odpowiednio dobrany model,
- mechanizm tool calling,
- integrację z wykorzystaniem Model Context Protocol (MCP),
- mechanizm Retrieval-Augmented Generation (RAG),
- ewaluację działania agenta.

Sposób wykorzystania poszczególnych mechanizmów powinien wynikać z charakteru projektu. Nie powinny być one dodawane wyłącznie w celu formalnego spełnienia wymagania.

## Technologie

Zespół samodzielnie dobiera biblioteki, modele LLM, bazy danych oraz pozostałe technologie potrzebne do realizacji projektu.

Modele LLM mogą być uruchamiane lokalnie lub udostępniane poprzez API. Realizacja projektu nie wymaga korzystania z płatnych modeli ani usług.

Informacje o dostępnych i rekomendowanych modelach oraz sposobach dostępu do nich znajdują się w dokumencie [models.md](models.md).


## Realizacja

Projekt rozwijany jest etapowo w trakcie semestru wraz z poznawaniem kolejnych zagadnień podczas wykładów i laboratoriów.

Szczegółowe kryteria oceny oraz punktacja poszczególnych etapów znajdują się w dokumencie `grading.md`.

## Repozytorium projektu

Każdy zespół tworzy jedno repozytorium Git dla swojego projektu, np. w serwisie GitHub lub GitLab.

Repozytorium może być publiczne lub prywatne. W przypadku repozytorium prywatnego zespół zapewnia prowadzącemu dostęp do repozytorium na czas realizacji i oceny projektu.

Link do repozytorium należy przekazać prowadzącemu na początku realizacji projektu.

Repozytorium powinno zawierać:

- imiona i nazwiska wszystkich autorów projektu,
- numer grupy zajęciowej,
- kod źródłowy projektu,
- dokumentację projektu, w tym jego wizję,
- instrukcję uruchomienia rozwiązania,
- materiały związane z ewaluacją projektu.

### README.md

Główny plik `README.md` repozytorium powinien rozpoczynać się od informacji identyfikujących projekt w następującym formacie:

# Nazwa projektu

**Autorzy:**
- Imię Nazwisko
- Imię Nazwisko

**Grupa:** numer grupy zajęciowej

`README.md` powinien również zawierać krótki opis projektu oraz instrukcję jego uruchomienia.

Historia repozytorium powinna odzwierciedlać rozwój projektu w trakcie semestru.