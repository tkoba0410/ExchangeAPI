# Stage11（ゴール）

最終更新: 2026-03-27  
対象ブランチ: `stage11`

## 1. 位置づけ

本書は、Stage11 の背景、ゴール、非ゴール、adapter 設計原則だけを示す goal document である。  
library の設計正本は引き続き [`docs/spec.md`](docs/spec.md) に置き、  
CLI と MCP Server の adapter 固有方針は [`docs/cli.md`](docs/cli.md) と [`docs/mcp-server.md`](docs/mcp-server.md) に置く。

Stage11 は、Stage10 で固定した library foundation の上に、CLI と MCP Server を adapter として実装する段階である。  
基本方針は「まず取引所 API をそのまま使えること」であり、`Native` を主経路、`Protocol` を明示 opt-in の raw/debug 経路とする。  
`Unified` は広い吸収層ではなく、本当に意味同一性を固定できる少数 capability だけを持つ薄い層として扱う。

## 2. ゴール

- `Composition` の上に CLI adapter を実装する
- `Composition` の上に MCP Server adapter を実装する
- CLI と MCP Server の主経路を venue-specific `Native` に置く
- raw response / inspection / debug のために `Protocol` を明示 opt-in 経路として実装する
- `Unified` を、取引所横断で意味同一性を固定できる capability だけを持つ薄い層として定義する
- Stage11 で `Unified` を expose する場合は、状態確認、指値注文、成行注文、注文キャンセルに限定する
- library 正本と adapter 正本を分離したまま実装を進める

## 3. 非ゴール

- 広い `Unified` 層の実装
- venue 固有機能を `Unified` に押し込むこと
- `Unified` 未対応 capability を `Native` へ暗黙 fallback させること
- Stage10 で固定した `Protocol` / `Native` の責務を作り直すこと
- adapter から concrete endpoint / runtime / signer / transport を直接配線すること
- retry / rate limiting / circuit breaker の既定実装

## 4. 設計原則

- 基本は取引所 API であり、venue ごとの差分は隠さない
- `Native` は主経路、`Protocol` は明示 opt-in の raw/debug 経路とする
- `Unified` は convenience 層ではなく、意味同一性を固定できる capability だけを持つ薄い層とする
- CLI / MCP Server は `native` / `protocol` / `unified` を sibling surface として提示してよい
- `Unified` から `Native` への暗黙 fallback を禁止する
- adapter は `Composition` と public surface を利用し、library の内部実装事情を所有しない

## 5. 参照先

- [`stage10.md`](stage10.md)
  - Stage10 library foundation の goal document
- [`docs/spec.md`](docs/spec.md)
  - library 設計正本
- [`docs/cli.md`](docs/cli.md)
  - CLI adapter の正本
- [`docs/mcp-server.md`](docs/mcp-server.md)
  - MCP Server adapter の正本
