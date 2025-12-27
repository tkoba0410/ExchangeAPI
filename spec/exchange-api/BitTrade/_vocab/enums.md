# BitTrade enums（公式明示）

このファイルは、公式ドキュメントに明示されている **列挙値（有限集合）** を索引化する。
本文の複製はしない。

## OrderType
- Values: buy-market | sell-market | buy-limit | sell-limit | buy-ioc | sell-ioc | buy-limit-maker | sell-limit-maker
- Where (official): https://api-doc.bittrade.co.jp/ （注文タイプ）
- Where (mirror): /local/doc-api/Bittrade/mirror/Rest API共通情報 – BitTrade API Reference.md#注文タイプ
- Notes:

## OrderStatus
- Values: created | submitted | partial-filled | partial-canceled | filled | canceled | canceling
- Where (official): https://api-doc.bittrade.co.jp/ （注文ステータス）
- Where (mirror): /local/doc-api/Bittrade/mirror/Rest API共通情報 – BitTrade API Reference.md#注文ステータス
- Notes:

## OrderStateCode
- Values: order-state | -1 | 1 | 3 | 4 | 5 | 6 | 7 | 10
- Where (official): https://api-doc.bittrade.co.jp/ （order-state対応表）
- Where (mirror): /local/doc-api/Bittrade/mirror/Rest API共通情報 – BitTrade API Reference.md#order-state対応表
- Notes:

## WithdrawStatus
- Values: submitted | reexamine | canceled | pass | reject | pre-transfer | wallet-transfer | wallet-reject | confirmed | confirm-error | repealed
- Where (official): https://api-doc.bittrade.co.jp/ （暗号資産出金ステータスの定義）
- Where (mirror): /local/doc-api/Bittrade/mirror/Rest API共通情報 – BitTrade API Reference.md#暗号資産出金ステータスの定義
- Notes:

## DepositStatus
- Values: unknown | confirming | confirmed | safe | orphan
- Where (official): https://api-doc.bittrade.co.jp/ （暗号資産入金ステータスの定義）
- Where (mirror): /local/doc-api/Bittrade/mirror/Rest API共通情報 – BitTrade API Reference.md#暗号資産入金ステータスの定義
- Notes:
