# CLI（adapter 設計補助文書）

最終更新: 2026-03-27  
対象ブランチ: `stage11`

## 1. 位置づけ

本書は、ExchangeAPI library の上に載る CLI adapter の設計補助文書である。  
library の設計正本は [`docs/spec.md`](./spec.md) に置き、  
本書では CLI 固有の責務、依存、公開面を扱う。

## 2. 責務

CLI は以下を所有する。

- request parsing
- env / config 読み込み
- output formatting
- exit code
- confirmation prompt
- write safety の UX

CLI は以下を所有しない。

- endpoint 実装
- transport / signer / runtime
- native contract / protocol contract の定義
- exchange 固有 business rule

## 3. 依存規約

- 現行 phase の依存は `Cli -> Composition` を基本とする
- CLI の主経路は `Native` とする
- raw response や debug のために `Protocol` を明示 opt-in 経路として持ってよい
- `Unified` を expose する場合でも、薄い取引所横断 capability に限定する
- CLI は `Native` / `Protocol` / optional な `Unified` を sibling surface として提示してよい
- CLI は concrete endpoint / runtime / signer / transport を直接配線しない

## 4. 公開面

- CLI は library surface を command tree に写像する adapter である
- `native` は venue 固有機能の入口である
- `protocol` は raw/debug/inspection の明示入口である
- `unified` は薄い取引所横断 capability の入口である
- `Unified` 未対応の capability を CLI から見えない形で `Native` へ自動切り替えしてはならない
- venue 固有機能が必要な場合は、利用者が明示的に `native` を選ぶ

## 5. 現行 phase

- Stage11 では CLI 実装を行う
- 初期実装は `native` 主経路と optional な `protocol` debug path から始める
- `Unified` を expose する場合は、状態確認、指値注文、成行注文、注文キャンセルに限定する
- command tree、global option、output format、exit code は Stage11 で実装対象に限って固定する

## 6. 未固定事項

- command tree
- subcommand naming
- auth / config 読み込み順序
- `json` / `table` / `raw` の出力契約
- write command の `--confirm` / `--yes` 規約
- exit code の具体値
