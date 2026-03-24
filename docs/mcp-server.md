# MCP Server（adapter 設計補助文書）

最終更新: 2026-03-24  
対象ブランチ: `stage10`

## 1. 位置づけ

本書は、ExchangeAPI library の上に載る MCP Server adapter の設計補助文書である。  
library の設計正本は [`docs/spec.md`](/home/tkoba/dev/tkoba0410/ExchangeAPI/docs/spec.md) に置き、  
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

- MCP Server の主経路は `Unified` とする
- MCP Server は `Protocol` や `Native` を直接正本として使わない
- `Unified` の capability だけを tool として expose する
- venue 固有機能をそのまま tool に出したい場合は、別途方針を固定しない限り対象外とする

## 4. 公開面

- MCP Server は `Unified` capability を tool surface に写像する adapter である
- `Unified` 未対応の capability を MCP Server から見えない形で `Native` へ自動切り替えしてはならない
- tool 名、tool schema、permission model の具体値は別途固定する

## 5. 現行 phase

- 現行 phase では MCP Server 実装は行わず、library の設計と実装を優先する
- `Unified` 未実装の間、本書は将来方針だけを保持する

## 6. 未固定事項

- tool inventory
- tool naming
- input / output schema
- auth / permission model
- observability / tracing rule
