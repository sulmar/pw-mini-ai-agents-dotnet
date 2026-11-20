# Ciekawe znaleziska

Projekty, narzędzia i eksperymenty wokół agentów AI, które znaleźli studenci (i nie tylko).
Warto je przejrzeć, przetestować i zastanowić się, jak działają pod spodem.

> Masz coś ciekawego? Zgłoś to na zajęciach albo dodaj wpis przez pull request.

## Znaleziska

| Nazwa | Link | Opis |
|---|---|---|
| VibeOS | [vibeos.sh](https://vibeos.sh) | Eksperymentalny, open source'owy (MIT) „AI-natywny” pulpit w przeglądarce: prosisz o aplikację w języku naturalnym, a agent buduje ją wewnątrz kontenera z Linuksem. Projekt hackathonowy. Kod: [github.com/caffeinum/vibeOS](https://github.com/caffeinum/vibeOS). |
| TabFM | [Google Research Blog](https://research.google/blog/introducing-tabfm-a-zero-shot-foundation-model-for-tabular-data/) | Model fundamentowy Google Research do klasyfikacji i regresji na danych tabelarycznych w trybie zero-shot: całą tabelę (przykłady + wiersze do predykcji) dostaje jako kontekst i przewiduje w jednym przebiegu, bez trenowania na danym zbiorze (in-context learning). Trenowany wyłącznie na syntetycznych zbiorach danych. Kod: [github.com/google-research/tabfm](https://github.com/google-research/tabfm), wagi: [Hugging Face](https://huggingface.co/google/tabfm-1.0.0-pytorch). |
| PonyTail | [ponytail.dev](https://ponytail.dev) | Zestaw reguł dla agentów kodujących (plugin m.in. do Claude Code, Codex, Copilot, Gemini CLI), który ma skłonić agenta do napisania jak najmniej kodu, który nadal działa. Twórcy deklarują ok. 53% mniej linii kodu i niższy koszt przy zachowanej poprawności (własny benchmark). Kod: [github.com/DietrichGebert/ponytail](https://github.com/DietrichGebert/ponytail). |
