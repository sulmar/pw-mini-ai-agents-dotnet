# Agent AI Cheat Sheet

## Fundamenty

- **Agent AI** — „cel → decyzja → działanie”
- **LLM** — „interpretuje i generuje”
- **Chatbot** — „prowadzi rozmowę”
- **Agent** — „działa”
- **Klasyczny program** — „reguły z góry”
- **Uczenie maszynowe** — „uczy się wzorców z danych”
- **Agent AI** — „decyzje w locie”
- **AI** — „tylko tam, gdzie reguły nie wystarczą”

## Prompt i kontekst

- **Prompt** — „instrukcja dla modelu”
- **System prompt** — „kim jesteś i jakie masz zasady”
- **User prompt** — „czego teraz chcę”
- **Kontekst** — „to, co model widzi teraz”
- **Historia** — „to, co wydarzyło się wcześniej”
- **Token** — „kawałek języka”
- **Context window** — „pamięć robocza modelu”
- **Structured output** — „odpowiedź w formacie, nie freestyle”
- **Halucynacja** — „brzmi pewnie, ale zmyśla”
- **„Nie wiem”** — „lepszy brak odpowiedzi niż fałszywa pewność”

## Tool Calling

- **Tool** — „co agent może zrobić?”
- **Schema** — „instrukcja obsługi narzędzia”
- **Tool calling** — „LLM wybiera, aplikacja wykonuje”
- **Pętla agentowa** — „pomyśl → użyj → sprawdź → powtórz”
- **Side effect** — „świat zmienił się naprawdę”

## MCP

- **MCP** — „USB-C dla narzędzi AI”
- **Host** — „miejsce pracy agenta”
- **Client** — „łącznik z MCP”
- **Server** — „dostawca możliwości”
- **Tools** — „zrób coś”
- **Resources** — „przeczytaj coś”
- **Prompts** — „podpowiedź, jak pracować”
- **Discovery** — „sprawdź, co jest dostępne”
- **Tool Calling ≠ MCP** — „wywołanie to nie protokół”
- **MCP ≠ Agent** — „dostęp do narzędzi to jeszcze nie samodzielność”

## Skills

- **Skill** — „procedura zamiast improwizacji”
- **Reference** — „wiedza, do której zaglądasz w razie potrzeby”

## Embeddings

- **Embedding** — „znaczenie zapisane liczbami”
- **Wektor** — „adres znaczenia”
- **Similarity** — „jak blisko są znaczenia”
- **Cosine similarity** — „czy wektory patrzą w tę samą stronę?”
- **Chunk** — „kawałek wiedzy do znalezienia”
- **Normalizacja** — „wspólna skala porównania”
- **Feature engineering** — „co mierzymy, tak porównujemy”

## Vector Search i RAG

- **Vector Search** — „szukaj po znaczeniu, nie po słowach”
- **Vector Database** — „magazyn znaczeń”
- **Retrieval** — „najpierw znajdź”
- **Generation** — „potem odpowiedz”
- **RAG (Retrieval-Augmented Generation)** — „znajdź → dołącz → odpowiedz”
- **Chunking** — „dobry podział, dobre wyszukiwanie”
- **Metadata** — „etykiety pomagające filtrować”
- **Źródła** — „odpowiedź z dowodem”
- **Aktualność** — „model zna przeszłość, RAG może znać dziś”

## Stan, pamięć i wiedza

- **Conversation history** — „co powiedzieliśmy”
- **Working state** — „nad czym teraz pracujemy”
- **Memory** — „co warto zachować na później”
- **Cache** — „co warto zachować na chwilę”
- **Knowledge** — „co system wie niezależnie od rozmowy”
- **Provenance** — „skąd to wiemy?”
- **Confidence** — „jak bardzo temu ufamy?”
- **Conflict resolution** — „co zrobić, gdy źródła się kłócą?”
- **Kontekst ≠ pamięć** — „widzi teraz ≠ zapamięta później”

## Multi-agent i A2A

- **Multi-agent** — „zespół specjalistów zamiast jednego omnibusa”
- **Delegowanie** — „daj zadanie właściwemu agentowi”
- **Orchestration** — „ktoś musi prowadzić orkiestrę”
- **Handoff** — „przekaż zadanie razem z kontekstem”
- **A2A** — „agent rozmawia z agentem”
- **MCP** — „agent korzysta z narzędzia”
- **Zasada** — „nowy agent tylko wtedy, gdy wnosi nową rolę”

## Bezpieczeństwo i ewaluacja

- **Evaluation** — „nie pytaj, czy działa — zmierz to”
- **Observability** — „widzimy, co się dzieje”
- **Tracing** — „ślad każdej decyzji”
- **Prompt injection** — „obca treść próbuje przejąć sterowanie”
- **Authorization** — „czy wolno?”
- **Permissions** — „co wolno?”
- **Least privilege** — „tylko tyle uprawnień, ile potrzeba”
- **Human-in-the-loop** — „człowiek przy ważnych decyzjach”
- **Audit** — „kto, co, kiedy i dlaczego”
- **Validation** — „nie ufaj — sprawdzaj”
- **Bezpieczny agent** — „czytaj szeroko, działaj wąsko”
- **Nieodwracalne działanie** — „najpierw potwierdzenie, potem wykonanie”

## Cały kurs w jednym ciągu

> **Prompt mówi, Tool robi, MCP łączy, Skill prowadzi, Embedding porównuje, RAG dostarcza wiedzę, Memory pamięta, A2A deleguje, Evaluation sprawdza, Security ogranicza.**
