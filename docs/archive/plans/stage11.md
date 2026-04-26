# Stage11（ゴール）

最終更新: 2026-03-27  
対象ブランチ: `stage11`

## 1. 位置づけ

本書は、Stage11 の背景、ゴール、非ゴール、adapter 設計原則だけを示す goal document である。  
library の設計正本は引き続き [`docs/spec.md`](docs/spec.md) に置き、  
CLI と MCP Server の adapter 固有方針は [`docs/cli.md`](docs/cli.md) と [`docs/mcp-server.md`](docs/mcp-server.md) に置く。

Stage11 は、Stage10 で固定した library foundation の上に、CLI と MCP Server を adapter として実装する段階である。  
基本方針は「まず取引所 API をそのまま使えること」であり、CLI では `Native` を主経路、`Protocol` を明示 opt-in の raw/debug 経路とする。  
MCP Server は `Composition` の上に載る tool adapter として扱い、tool surface の正本は [`docs/mcp-server.md`](docs/mcp-server.md) に置く。  
`Unified` は広い吸収層ではなく、本当に意味同一性を固定できる少数 capability だけを持つ薄い層として扱う。  
ただし Stage11 の完了条件は CLI / MCP Server adapter の成立であり、`Unified` 実装は必須条件ではない。

## 2. ゴール

- `Composition` の上に CLI adapter を実装する
- `Composition` の上に MCP Server adapter を実装する
- CLI の主経路を venue-specific `Native` に置く
- CLI の raw response / inspection / debug のために `Protocol` を明示 opt-in 経路として実装する
- MCP Server の主経路を `Composition` 上の tool surface に置く
- 他 repo で実装する自動 Bot が、ExchangeAPI Library または MCP Server の成果物を利用できる状態にする
- CLI は自動 Bot の実行面ではなく、人間向けの運用・検証・デバッグ成果物として利用できる状態にする
- `Unified` を導入する場合の境界条件を、取引所横断で意味同一性を固定できる capability に限定して定義する
- Stage11 で `Unified` を expose する場合は、状態確認、指値注文、成行注文、注文キャンセルに限定する
- library 正本と adapter 正本を分離したまま実装を進める
- Stage11 の完了条件を、CLI / MCP Server adapter の実装と各 adapter 正本への準拠として固定する

## 2.1 完了条件

Stage11 は、少なくとも以下を満たした時点で完了とみなす。

- CLI adapter が `Composition` の上で実装され、主経路を `Native`、raw/debug を明示 opt-in の `Protocol` とする
- CLI adapter が [`docs/cli.md`](docs/cli.md) で固定した command tree、入出力契約、安全制約、exit code を満たす
- MCP Server adapter が `Composition` の上で実装され、[`docs/mcp-server.md`](docs/mcp-server.md) で固定した tool surface と adapter 契約を満たす
- ExchangeAPI Library が他 repo から `ProjectReference` または local NuGet feed を通じて利用可能である
- MCP Server adapter が実行可能成果物として build 可能であり、他 repo の自動 Bot から利用可能である
- CLI adapter が実行可能成果物として build 可能であり、人間向けの運用・検証・デバッグ用途で利用可能である
- adapter が library の内部実装へ直接依存せず、Stage10 で固定した `Protocol` / `Native` / `Composition` の責務境界を維持する

`Unified` は Stage11 で検討または限定公開してよいが、未実装であっても上記完了条件を満たす限り Stage11 は完了とみなす。

## 2.2 検証水準

Stage11 の完了判定は、次の 2 水準に分けて扱う。

- `Engineering Complete`
  - main solution と live solution の restore / build / test 導線が repo 単体で成立する
  - CLI / MCP Server / library の正本が整合し、通常 test が成功する
  - live test は opt-in 未設定で skip されていてよい
- `Live Verified`
  - `docs/spec.md` で定義した live test opt-in 条件を満たし、対象 live test を明示実行して確認済みである

Stage11 の完了条件は `Engineering Complete` を基準とする。  
`Live Verified` は confidence を追加する検証水準であり、opt-in 未実行を failure 扱いにしない。

## 3. 非ゴール

- 広い `Unified` 層の実装
- `Unified` 実装を Stage11 完了の必須条件にすること
- venue 固有機能を `Unified` に押し込むこと
- `Unified` 未対応 capability を `Native` へ暗黙 fallback させること
- Stage10 で固定した `Protocol` / `Native` の責務を作り直すこと
- adapter から concrete endpoint / runtime / signer / transport を直接配線すること
- retry / rate limiting / circuit breaker の既定実装

## 4. 設計原則

- 基本は取引所 API であり、venue ごとの差分は隠さない
- CLI では `Native` を主経路、`Protocol` を明示 opt-in の raw/debug 経路とする
- MCP Server は `Composition` 上の tool adapter として扱い、`Protocol` を sibling surface として要求しない
- `Unified` は convenience 層ではなく、意味同一性を固定できる capability だけを持つ薄い層とする
- CLI は `native` / `protocol` を基本の sibling surface とし、`unified` は optional surface として提示してよい
- `Unified` から `Native` への暗黙 fallback を禁止する
- adapter は `Composition` と public surface を利用し、library の内部実装事情を所有しない

## 5. 参照先

- [`docs/spec.md`](docs/spec.md)
  - library 設計正本
- [`docs/cli.md`](docs/cli.md)
  - CLI adapter の正本
- [`docs/mcp-server.md`](docs/mcp-server.md)
  - MCP Server adapter の正本
