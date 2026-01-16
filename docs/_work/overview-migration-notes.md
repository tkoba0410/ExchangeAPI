# Overview 移動・削除候補（洗い出し）

## TopSpec → Overview

- [MOVE] docs/topspec.md :: 7 > 7.1 > 7.1.2 Normalized API の正規形
  - 抜粋: "未対応機能は『NoOp 実装』を返し、Call を NotSupported として失敗させる"
  - 理由: NotSupported の意味・扱いは利用者保証/分岐規則に該当

- [MOVE] docs/topspec.md :: 7 > 7.1 > 7.1.2 Normalized API の正規形
  - 抜粋: "NotSupported の最小表現は次に固定する"
  - 理由: NotSupported 表現は利用者契約（例外分岐禁止）に直結

- [KEEP] docs/topspec.md :: 7 > 7.1 > 7.1.1 Raw API の正規形
  - 抜粋: "Raw の bundle は Sub-API のみを公開する"
  - 理由: 実装構造（Canon）であり Overview ではない

- [KEEP] docs/topspec.md :: 4.2 Raw > RawJson / ClosedSet に関する判断
  - 抜粋: "RawJson の保持有無や ClosedSet の拡張可否は inventory に記録"
  - 理由: レイヤ責務/運用境界であり利用者保証ではない

## Contracts → Overview / Delete

- [MOVE] docs/contracts/contracts.md :: 5 > 5.1 NotSupported（未対応機能）の最小表現
  - 抜粋: "NotSupported は例外ではなく Call の失敗として表現"
  - 理由: 利用者が分岐/期待を判断する契約に該当

- [KEEP] docs/contracts/contracts.md :: 4 API 返却形式（Call-only）
  - 抜粋: "公開 API は Call を唯一の返却形式とする"
  - 理由: 形状/意味論の規範であり Overview ではない

- [KEEP] docs/contracts/contracts.md :: 10 Page / Cursor / Limit 契約
  - 抜粋: "Page は1回の取得結果、Cursor は opaque"
  - 理由: 型・意味論の規範であり Overview ではない

- [DELETE] docs/contracts/contracts.md :: 8.1 DTO 命名（例行）
  - 抜粋: "例：OrderSnapshot, ExecutionHistoryItem"
  - 理由: 例示は型規範の必須要件ではなく曖昧化要因

- [DELETE] docs/contracts/contracts.md :: 8.2 プロパティ命名（例行）
  - 抜粋: "例：Id, Url"
  - 理由: 例示は型規範の必須要件ではなく曖昧化要因
