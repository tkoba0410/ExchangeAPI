# BitTrade constants（公式明示）

このファイルは、公式ドキュメントに明示されている **固定値 / 定数的制約** を索引化する。
本文の複製はしない。

## Content-Type
- Value/Constraint: application/json
- AppliesTo: Request headers (POST endpoints using JSON body examples)
- Where (official): https://api-doc.bittrade.co.jp/
- Where (mirror): /local/doc-api/Bittrade/mirror/Rest API共通情報 – BitTrade API Reference.md#HTTP Request

## SignatureMethod
- Value/Constraint: HmacSHA256
- AppliesTo: Signed requests
- Where (official): https://api-doc.bittrade.co.jp/
- Where (mirror): /local/doc-api/Bittrade/mirror/Rest API共通情報 – BitTrade API Reference.md#署名処理

## SignatureVersion
- Value/Constraint: 2
- AppliesTo: Signed requests
- Where (official): https://api-doc.bittrade.co.jp/
- Where (mirror): /local/doc-api/Bittrade/mirror/Rest API共通情報 – BitTrade API Reference.md#署名処理
