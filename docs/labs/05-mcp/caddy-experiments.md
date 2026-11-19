# Zadania — Caddy MCP (90 minut)

Pracuj zgodnie z [instrukcją uruchomienia](README.md). Edytuj `src/Caddy.Mcp/CaddyTools.cs`. Klient `CaddyAdminClient` i rejestracja transportu są gotowe; nie musisz implementować protokołu MCP ani klienta HTTP.

## EXP-00 — Poznaj przepływ (15–25 min)

1. Odczytaj `caddy/caddy.json`. Znajdź port publiczny, port administracyjny, identyfikator handlera i adres upstreamu.
2. Wywołaj `GET http://127.0.0.1:2019/config/` przez curl lub Postman.
3. Uruchom MCP Inspector i odczytaj listę narzędzi.
4. Wywołaj `get_lab_info`, a następnie `get_configuration`. Drugie wywołanie powinno zgłosić TODO.
5. Znajdź w kodzie `[McpServerToolType]`, `[McpServerTool]` i `[Description]`. Zobacz opis oraz schemat argumentów `switch_backend` w Inspectorze.

**Do omówienia:** czym różnią się HTTP używane do Caddy i stdio używane do MCP? Który proces uruchamia serwer MCP?

## EXP-01 — Odczyt konfiguracji (25–40 min)

Uzupełnij `GetConfiguration` (TODO 1):

- wywołaj odpowiednią metodę `CaddyAdminClient`,
- przekaż `CancellationToken`,
- zwróć otrzymany JSON jako tekst, bez opakowywania go w dodatkowy opis.

Klient zwraca `Task<string>` — możesz przekazać ten Task bezpośrednio albo użyć `async`/`await`.

**Odbiór:** wynik `get_configuration` zawiera `lab-proxy`, port `8080` i upstream `127.0.0.1:5101`. Porównaj go z `GET /config/` przez curl. Przygotowana warstwa HTTP tłumaczy błędy połączenia na `McpException`.

## EXP-02 — Aktywny backend (40–50 min)

Uzupełnij `GetActiveBackend` (TODO 2). Wykorzystaj metodę klienta odczytującą upstreamy handlera `lab-proxy`.

**Odbiór:** narzędzie zwraca tablicę JSON, początkowo:

```json
[{"dial":"127.0.0.1:5101"}]
```

Nie parsuj całego drzewa konfiguracji, jeśli API udostępnia potrzebny fragment. „Aktywny backend” oznacza tutaj backend skonfigurowany w proxy; odczyt konfiguracji nie potwierdza, że aplikacja odpowiada.

## EXP-03 — Przełączanie ruchu (50–70 min)

Uzupełnij `SwitchBackend` (TODO 3):

1. Przyjmuj dokładnie `api-a` albo `api-b`.
2. Przypisz nazwom adresy `127.0.0.1:5101` i `127.0.0.1:5102`.
3. Dla innej wartości rzuć `McpException` z komunikatem wskazującym dozwolone nazwy. Walidacja musi nastąpić przed żądaniem HTTP.
4. Wywołaj `SetUpstreamsAsync`, przekazując adres i token anulowania.
5. Zwróć wynik operacji. API Caddy może zwrócić pustą odpowiedź; klient zamienia ją na `OK`.

Nie przyjmuj dowolnych adresów, nie wywołuj powłoki i nie edytuj pliku startowego. Zmieniasz listę upstreamów przez API działającego Caddy.

**Odbiór:** wywołaj `switch_backend` z argumentem:

```json
{"backend":"api-b"}
```

Następnie:

```sh
curl http://127.0.0.1:8080/
curl http://127.0.0.1:8080/api/products
```

Obie odpowiedzi muszą wskazywać `service: api-b`. `get_active_backend` ma teraz zwrócić `127.0.0.1:5102`. Bezpośredni port `5101` nadal zwraca `api-a`.

## EXP-04 — Błędy i cofnięcie (70–80 min)

| Próba | Oczekiwany wynik |
|---|---|
| `switch_backend` z `api-c` | Błąd MCP z dozwolonymi nazwami; upstream pozostaje bez zmian |
| `switch_backend` z `http://example.com` | Błąd walidacji; upstream pozostaje bez zmian |
| Zatrzymaj Caddy, wywołaj `get_configuration` | Czytelny błąd połączenia |
| Uruchom ponownie Caddy z plikiem startowym | Proxy wraca do `api-a` |
| Przełącz na api-b i zatrzymaj api-b | Przez proxy otrzymujesz HTTP 502; odczyt upstreamu nadal działa |
| Uruchom api-b, przełącz na api-a | Proxy ponownie zwraca `service: api-a` |

Rozróżnij błąd protokołu MCP, błąd narzędzia, błąd API administracyjnego oraz niedostępny backend. Gotowe TODO zgłaszają błędy narzędzia; protokół MCP i odkrywanie narzędzi pozostają sprawne.

## Odbiór i dyskusja (80–90 min)

Pokaż prowadzącemu:

- cztery narzędzia widoczne w Inspectorze,
- odczyt konfiguracji i upstreamu,
- przełączenie api-a → api-b → api-a potwierdzone żądaniami przez port 8080,
- odrzucenie nieznanej nazwy bez zmiany konfiguracji,
- czytelny błąd po wyłączeniu Caddy.

Odpowiedz: czy MCP wymaga LLM? Dlaczego opis narzędzia nie zastępuje walidacji w C#? Czy odczyt upstreamu jest testem zdrowia backendu? Dlaczego `Console.WriteLine` może zepsuć transport stdio?

## Dla chętnych

Dodaj `reset_backend` bez argumentów, przywracające api-a, albo osobne narzędzie sprawdzające `/health` obu przygotowanych backendów. Opisz różnicę między konfiguracją proxy a dostępnością aplikacji. Rozszerzenia wykonuj po zaliczeniu trzech podstawowych TODO.
