# Roadmap kursu

> Od modelu językowego do systemu agentowego działającego w rzeczywistym świecie.

---

## 01. Agent AI
### Co właściwie budujemy?

- LLM, chatbot, agent
- Cel, kontekst, decyzja, działanie
- Agent a klasyczny program
- Klasyczne algorytmy, Machine Learning, GenAI
- Gdzie AI ma sens, a gdzie go nie potrzebujemy
- TUP Lab — rzeczywisty system agentowy

➡️ **Jak powiedzieć modelowi, czego od niego oczekujemy?**

---

## 02. Prompt & Context
### Jak agent rozumie nasze oczekiwania?

- System prompt i user prompt
- Context i historia rozmowy
- Tokeny i context window
- Structured output
- Halucynacje
- „Nie wiem” jako poprawna odpowiedź

➡️ **Jak umożliwić agentowi zdobywanie informacji i wykonywanie działań?**

---

## 03. Tool Calling
### Jak agent zaczyna działać?

- Function / Tool Calling
- Definicja i schema narzędzia
- Wybór narzędzia przez LLM
- Wykonanie przez aplikację
- Wynik i pętla agentowa
- Side effects

> **LLM proponuje działanie. Aplikacja je wykonuje.**

➡️ **Jak udostępniać możliwości różnym agentom i aplikacjom?**

---

## 04. MCP
### Jak agent uzyskuje dostęp do możliwości systemów?

- Model Context Protocol
- Host, Client, Server
- Tools, Resources, Prompts
- Discovery
- MCP Inspector
- MCP w .NET

> **Tool Calling ≠ MCP ≠ Agent**

➡️ **Dostęp do narzędzi nie mówi jeszcze agentowi, jak wykonać zadanie.**

---

## 05. Skills
### Skąd agent wie, jak wykonać zadanie?

- Tool — **co mogę zrobić?**
- Skill — **jak mam to zrobić?**
- Procedury
- Workflow
- Wiedza domenowa
- Zadania wieloetapowe

➡️ **Jak reprezentować znaczenie i podobieństwo?**

---

## 06. Embeddings
### Co właściwie oznacza „podobne”?

- Token i chunk
- Embedding
- Wektor
- Similarity
- Cosine similarity
- Feature engineering
- Normalizacja
- Embedding nie musi reprezentować tekstu

➡️ **Jak znaleźć potrzebną wiedzę w dużym zbiorze danych?**

---

## 07. Vector Search & RAG
### Skąd agent bierze wiedzę?

- Vector Search
- Vector Database
- Redis
- Retrieval
- Chunking
- Metadata
- RAG
- Źródła, aktualność i cytowanie

Materiały kursu mogą stać się bazą wiedzy:

- Markdown
- dokumentacja
- transkrypcje wykładów
- przykłady
- linki z mapy kursu

➡️ **Co agent powinien pamiętać?**

---

## 08. State, Memory & Knowledge
### Jak agent zachowuje kontekst i zdobywa wiedzę?

- Conversation history
- Working state
- Memory
- Cache
- Persistent knowledge
- Provenance
- Confidence
- Conflict resolution

➡️ **Czy jeden agent musi robić wszystko?**

---

## 09. A2A & Multi-Agent Systems
### Jak agenci współpracują?

- Agenci specjalistyczni
- Delegowanie zadań
- Orchestration
- Handoff
- Agent-to-Agent
- A2A vs MCP

> **Czy naprawdę potrzebujemy kolejnego agenta?**

➡️ **Skąd wiemy, że możemy temu systemowi zaufać?**

---

## 10. Security, Evaluation & Responsibility
### Czy agent działa dobrze i bezpiecznie?

- Evaluation
- Observability i tracing
- Prompt injection
- Authorization
- Permissions
- Least privilege
- Human-in-the-loop
- Audit
- Validation
- Side effects

> **Nie uczymy się tylko, jak dawać agentowi możliwości.  
> Uczymy się również, których możliwości mu nie dawać.**

---

## Cała droga

```text
Agent
  ↓
Prompt & Context
  ↓
Tool Calling
  ↓
MCP
  ↓
Skills
  ↓
Embeddings
  ↓
Vector Search & RAG
  ↓
State & Memory
  ↓
A2A & Multi-Agent
  ↓
Security & Evaluation
```

---

> **Nie pytamy: „Jak użyć tutaj AI?”**  
> **Pytamy: „Która część problemu rzeczywiście wymaga AI?”**
