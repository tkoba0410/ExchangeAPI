# CommonSymbols（取引ペア情報）

本ファイルは endpoint 単位の **正本（contract note）** である。
公式ドキュメント本文の代替・複製を目的としない。正確かつ最新の情報は公式ドキュメントを正本とする。

- Official（参照）: https://api-doc.bittrade.co.jp/
- Mirror（非公開・参照用）: /local/doc-api/Bittrade/mirror/Rest API共通情報 – BitTrade API Reference.md

## Request
- Scope: Private
- ScopeBasis: Derived from heading containing "署名".
- Method: GET
- Path: /v1/common/symbols

### Headers
- なし

### Query
- なし

### Body
- なし

### Enumerations (if any)
- status: ok/error
- state: online/offline/suspend

## Response（fact）
正本は sample.json とする。取得条件・出典は sample.meta.md を参照。

## Notes
- なし
