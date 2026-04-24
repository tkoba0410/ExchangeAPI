# Documentation Archive

最終更新: 2026-04-22  
位置づけ: アーカイブ案内

本ディレクトリには、過去 phase の計画文書、旧 draft、履歴参照用文書を置く。  
ここにある文書は履歴と判断経緯を残すためのものであり、現行の正本として扱わない。

## 1. 目的

- 過去文書を消さずに残す
- 現行正本との混在を避ける
- 履歴参照先を一箇所に寄せる

## 2. 現行正本ではないもの

- `plans/`
  - stage 単位の計画・キックオフ文書
- `drafts/`
  - 採用前の version draft
- `library-bootstrap-and-history.md`
  - 初期 bootstrap、実装順、DoD、流用判断などの履歴メモ
- `adapter-status-and-history.md`
  - adapter の completion、verification label、phase/status 記述の履歴
- `endpoint-history-and-examples.md`
  - venue endpoint 文書から切り出した実装順、初期ルール、代表 contract 例

## 3. 現行正本の参照先

- 文書体系ガイド: [`../docs-architecture.md`](../docs-architecture.md)
- 共通正本: [`../spec.md`](../spec.md)
- venue 台帳: [`../endpoints-bitflyer.md`](../endpoints-bitflyer.md), [`../endpoints-binance.md`](../endpoints-binance.md)
- adapter 正本: [`../cli.md`](../cli.md), [`../mcp-server.md`](../mcp-server.md)

## 4. 運用ルール

- 現行契約を更新する場合、archive 文書を直接修正先にしない
- archive 文書は必要なら注記やリンク修正だけ行ってよい
- archive から現行文書へ内容を移す場合は、移植先で再編集して採用する
