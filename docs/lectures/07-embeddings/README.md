# Tworzenie agentów AI w .NET

## Wykład 07 — Embeddings

> Materiały w przygotowaniu.

## Wymagania

Do uruchomienia notebooka [`src/embedding-csharp-notebook.ipynb`](src/embedding-csharp-notebook.ipynb) potrzebujesz:

| Wymaganie | Do czego? |
|---|---|
| [.NET SDK 10](https://dotnet.microsoft.com/download) | Kernel .NET (C#). Pakiety Plotly.NET wymagają .NET 10. |
| [Visual Studio Code](https://code.visualstudio.com/) z rozszerzeniem [Polyglot Notebooks](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.dotnet-interactive-vscode) | Uruchamianie notebooka `.ipynb` z kodem C#. |
| [Ollama](https://ollama.com/) z modelem `embeddinggemma` (ok. 622 MB) | Lokalne embeddingi w sekcjach 8–11. Klucz API nie jest potrzebny. |
| Dostęp do internetu przy pierwszym uruchomieniu | Pobranie pakietów NuGet: `Plotly.NET.CSharp`, `Plotly.NET.Interactive`, `UMAP`. |

### Przygotowanie

1. Zainstaluj rozszerzenie Polyglot Notebooks:

   ```bash
   code --install-extension ms-dotnettools.dotnet-interactive-vscode
   ```

2. Pobierz model embeddingów:

   ```bash
   ollama pull embeddinggemma
   ```

3. Upewnij się, że Ollama działa w tle (aplikacja Ollama albo `ollama serve` w osobnym terminalu). Możesz to sprawdzić:

   ```bash
   curl http://localhost:11434/api/version
   ```

### Uruchomienie

1. Otwórz notebook w VS Code.
2. Wybierz kernel **.NET (C#)**.
3. Uruchamiaj komórki po kolei (**Shift + Enter**) albo wszystkie naraz (**Run All**).

Kolejność komórek ma znaczenie: późniejsze sekcje korzystają ze zmiennych i funkcji zdefiniowanych wcześniej (np. sekcja 11 używa funkcji `CosinusDemo` z sekcji 8).

Sekcje 1–7 działają bez Ollamy. Sekcje 8–11 łączą się z Ollamą pod adresem `http://localhost:11434`.
