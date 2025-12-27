# MeSendparentorder（新規の親注文を出す（特殊注文））

本ファイルは endpoint 単位の **正本（contract note）** である。
公式ドキュメント本文の代替・複製を目的としない。正確かつ最新の情報は公式ドキュメントを正本とする。

- Official（参照）: https://lightning.bitflyer.com/docs/api
- Mirror（非公開・参照用）: /local/doc-api/Bitflyer/mirror/ビットコイン取引所【bitFlyer Lightning】.md

## Request
- Scope: Private
- Method: POST
- Path: /v1/me/sendparentorder

### Aliases
なし

### Query
なし

### Body
- "IFD"
- "IFDOCO"
- "LIMIT"
- "MARKET"
- "OCO"
- "SIMPLE"
- "STOP"
- "STOP_LIMIT"
- "TRAIL"
- condition_type
- minute_to_expire
- offset
- order_method
- parameters
- price
- product_code
- side
- size
- time_in_force
- trigger_price

## Response（fact）
正本は sample.json とする。取得条件・出典は sample.meta.md を参照。

## Notes
なし
