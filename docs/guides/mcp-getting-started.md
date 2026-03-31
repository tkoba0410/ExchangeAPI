# MCP Getting Started

## 1. 目的

ExchangeAPI MCP Server を起動し、最小の tool call を 1 回成功させる。

MCP Server は read / evaluate-only であり、実発注しない。

## 2. 前提

- `dotnet` SDK `10.0`
- ExchangeAPI repo を checkout 済み
- stdio で起動できる MCP client、または JSON-RPC を直接流せる shell

## 3. 導入

MCP Server executable を local publish する。

```bash
bash scripts/publish-mcp-local.sh
```

生成先:

```text
local/publish/mcp/linux-x64/exchangeapi-mcp
```

MCP client に登録する場合の最小例:

```json
{
  "mcpServers": {
    "exchangeapi": {
      "command": "/absolute/path/to/ExchangeAPI/local/publish/mcp/linux-x64/exchangeapi-mcp"
    }
  }
}
```

## 4. 最小例

shell から直接 `list_markets` を呼ぶ。

```bash
printf '%s\n' \
  '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2025-11-25","capabilities":{},"clientInfo":{"name":"manual","version":"1"}}}' \
  '{"jsonrpc":"2.0","id":2,"method":"tools/call","params":{"name":"list_markets","arguments":{}}}' \
  | ./local/publish/mcp/linux-x64/exchangeapi-mcp
```

## 5. 動作確認

成功時は `stdout` に JSON-RPC response が出て、`markets` 配列に venue / symbol / capabilities が含まれる。

例:

```json
{
  "jsonrpc": "2.0",
  "id": 2,
  "result": {
    "structuredContent": {
      "markets": [
        {
          "venue": "bitflyer",
          "symbol": "BTC_JPY"
        }
      ]
    }
  }
}
```

tool contract の詳細は [`../mcp-server.md`](../mcp-server.md) を参照する。
