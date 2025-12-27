# OrderOrdersBatchcancel（注文の一括キャンセル）

本ファイルは endpoint 単位の **正本（contract note）** である。
公式ドキュメント本文の代替・複製を目的としない。正確かつ最新の情報は公式ドキュメントを正本とする。

- Official（参照）: https://api-doc.bittrade.co.jp/
- Mirror（非公開・参照用）: /local/doc-api/Bittrade/mirror/Rest API共通情報 – BitTrade API Reference.md

## Request
- Scope: Unclassified
- ScopeBasis: No explicit basis found; placed under Unclassified.
- Method: POST
- Path: /v1/order/orders/batchcancel

### Headers
- Content-Type: application/json

### Query
- order-ids
- client-order-ids

### Body
- なし

### Enumerations (if any)
- なし

## Response（fact）
正本は sample.json とする。取得条件・出典は sample.meta.md を参照。

## Notes
- なし
