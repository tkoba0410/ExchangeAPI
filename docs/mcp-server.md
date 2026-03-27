# MCP Server（adapter 設計補助文書）

最終更新: 2026-03-27  
対象ブランチ: `stage11`

## 1. 位置づけ

本書は、ExchangeAPI library の上に載る MCP Server adapter の設計補助文書である。  
library の設計正本は [`docs/spec.md`](./spec.md) に置き、  
本書では MCP Server 固有の責務、依存、tool 公開方針を扱う。

## 2. 責務

MCP Server は以下を所有する。

- tool schema
- tool input / output の adapter 契約
- session / transport ごとの公開制御
- tool-level observability

MCP Server は以下を所有しない。

- venue 固有 endpoint 実装
- transport / signer / runtime
- `Protocol` / `Native` の正本定義

## 3. 依存規約

- 現行 phase の依存は `McpServer -> Composition` を基本とする
- MCP Server の主経路は venue-specific `Native` とする
- raw response や debug のために `Protocol` を明示 opt-in tool surface として持ってよい
- `Unified` を expose する場合は、薄い取引所横断 capability に限定する
- venue 固有機能は `native.<venue>.*` としてそのまま tool に出してよい
- MCP Server は concrete endpoint / runtime / signer / transport を直接配線しない

### 3.1 物理配置

- MCP Server project は `src/Adapters/McpServer/ExchangeApi.Adapters.McpServer.csproj` に置く
- MCP Server test project は `tests/Adapters/McpServer.Tests/ExchangeApi.Adapters.McpServer.Tests.csproj` に置く
- MCP Server は external adapter であり、`src/Exchanges/<Venue>/` 配下に置いてはならない
- MCP Server の direct project reference は venue ごとの `Composition` project に限定する
  - `src/Exchanges/Bitflyer/Composition/ExchangeApi.Exchanges.Bitflyer.Composition.csproj`
  - `src/Exchanges/Binance/Composition/ExchangeApi.Exchanges.Binance.Composition.csproj`
- MCP Server から `Native` / `Protocol` / `Vocabulary` project を直接参照してはならない

推奨フォルダ構成:

```text
src/Adapters/McpServer/
  ExchangeApi.Adapters.McpServer.csproj
  Program.cs
  Tools/
    Native/
      Bitflyer/
      Binance/
    Protocol/
      Bitflyer/
      Binance/
  Schema/
  Permissions/
  Observability/
  Infrastructure/
```

補足:

- `Tools/Native/<Venue>/` と `Tools/Protocol/<Venue>/` は tool surface の物理写像として扱う
- venue 横断で重複が固まるまでは、追加の common project を先行導入しない

## 4. 公開面

- MCP Server は library surface を tool surface に写像する adapter である
- `native.<venue>.*` は venue 固有機能の主入口である
- `protocol.<venue>.*` は raw/debug/inspection の明示入口である
- `unified.*` は薄い取引所横断 capability の入口である
- `Unified` 未対応の capability を MCP Server から見えない形で `Native` へ自動切り替えしてはならない
- tool 名、tool schema、permission model の具体値は別途固定する

## 5. 現行 phase

- Stage11 では MCP Server 実装を行う
- 初期実装は `native` 主経路と optional な `protocol` debug tool から始める
- `Unified` を expose する場合は、状態確認、指値注文、成行注文、注文キャンセルに限定する

## 6. 未固定事項

- tool inventory
- tool naming
- input / output schema
- auth / permission model
- observability / tracing rule
