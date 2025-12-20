# Bittrade Raw API — Requests / Responses（索引）

本ドキュメントは、Bittrade Raw API で使用する **Request / Response DTO** を一覧化した索引です。
命名は prefix-less とし、衝突は namespace で回避します。

> 命名規則は `../../Raw/Naming.md` を参照。

---

## 共通フォーマット

### CommonResponse

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| status | string | yes | `ok` / `error` |
| data | object / list / scalar | no | 正常系のデータ |
| err-code | string | no | エラーコード |
| err-msg | string | no | エラーメッセージ |

### RetailResponse

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| code | int | yes | HTTP Status Code |
| data | object / scalar | no | 正常系のデータ |
| message | string | no | メッセージ |
| success | bool | no | 成功可否 |

---

## Public API

### GetSymbolsResponse

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| status | string | yes | `ok` / `error` |
| data | list | yes | 取引ペア一覧 |

### GetCurrenciesResponse

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| status | string | yes | `ok` / `error` |
| data | list | yes | 通貨コード一覧 |

### GetTimestampResponse

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| status | string | yes | `ok` / `error` |
| data | long | yes | Unix ms |

### GetKlinesRequest

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| symbol | string | yes | 例: `btcjpy` |
| period | string | yes | 例: `1day` |
| size | int | no | 件数 |

### GetKlinesResponse

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| status | string | yes | `ok` / `error` |
| ts | long | yes | Unix ms |
| data | list | yes | ローソク足配列 |

### GetMergedTickerRequest

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| symbol | string | yes | 例: `btcjpy` |

### GetMergedTickerResponse

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| status | string | yes | `ok` / `error` |
| tick | object | yes | 価格/出来高など |

### GetTickersResponse

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| status | string | yes | `ok` / `error` |
| data | list | yes | 全取引ペアの相場 |

### GetDepthRequest

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| symbol | string | yes | 例: `btcjpy` |
| type | string | no | 例: `step0` / `step1` |

### GetDepthResponse

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| status | string | yes | `ok` / `error` |
| tick | object | yes | 板情報 |

### GetTradesRequest

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| symbol | string | yes | 例: `btcjpy` |

### GetTradesResponse

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| status | string | yes | `ok` / `error` |
| tick | object | yes | 取引配列 |

### GetTradeHistoryRequest

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| symbol | string | yes | 例: `btcjpy` |

### GetTradeHistoryResponse

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| status | string | yes | `ok` / `error` |
| data | list | yes | 取引履歴 |

### GetRetailMaintainTimeResponse

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| code | int | yes | HTTP Status Code |
| data.start_time | string | yes | `HH:mm:ss` |
| data.end_time | string | yes | `HH:mm:ss` |
| data.ts | long | yes | Unix ms |
| data.state | int | yes | 状態 |

---

## Private API (Auth)

### GetAccountsResponse

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| status | string | yes | `ok` / `error` |
| data | list | yes | 口座一覧 |

### GetAccountBalanceRequest

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| account-id | string | yes | Path パラメータ |

### GetAccountBalanceResponse

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| status | string | yes | `ok` / `error` |
| data | object | yes | 口座残高 |

### CreateOrderRequest

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| account-id | string | yes | 口座ID |
| symbol | string | yes | 例: `btcjpy` |
| type | string | yes | 例: `buy-limit` / `sell-market` |
| amount | string | yes | 数量 |
| price | string | no | 指値時のみ |
| source | string | no | 例: `api` |

### CreateOrderResponse

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| status | string | yes | `ok` / `error` |
| data | long | no | 注文ID |

### GetOpenOrdersRequest

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| symbol | string | yes | 例: `btcjpy` |
| account-id | string | yes | 口座ID |

### GetOpenOrdersResponse

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| status | string | yes | `ok` / `error` |
| data | list | yes | 未約定注文一覧 |

### CancelOrderRequest

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| order-id | string | yes | Path パラメータ |

### CancelOrderResponse

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| status | string | yes | `ok` / `error` |
| data | long | no | 取消結果ID |

### CancelOrdersRequest

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| order-ids | list | yes | 注文ID配列 |

### CancelOpenOrdersRequest

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| account-id | string | no | 条件指定 |
| symbol | string | no | 条件指定 |
| side | string | no | `buy` / `sell` |
| size | string | no | 条件指定 |
| price | string | no | 条件指定 |
| created-at | long | no | 条件指定 |

### GetOrderRequest

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| order-id | string | yes | Path パラメータ |

### GetOrderMatchResultsRequest

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| order-id | string | yes | Path パラメータ |

### GetOrdersRequest

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| symbol | string | yes | 取引ペア |
| states | string | yes | 注文状態（カンマ区切り） |
| start-date | string | no | `YYYY-MM-DD` |
| end-date | string | no | `YYYY-MM-DD` |
| from | long | no | ページング |
| direct | string | no | `prev` / `next` |
| size | int | no | 件数 |

### GetMatchResultsRequest

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| symbol | string | no | 取引ペア |
| types | string | no | `buy-market`, `sell-limit` 等 |
| start-date | string | no | `YYYY-MM-DD` |
| end-date | string | no | `YYYY-MM-DD` |
| from | long | no | ページング |
| direct | string | no | `prev` / `next` |
| size | int | no | 件数 |

### CreateWithdrawRequest

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| address | string | yes | 出金先アドレス |
| amount | string | yes | 出金量 |
| currency | string | yes | 通貨コード |
| fee | string | no | 手数料 |
| addr-tag | string | no | メモ/タグ |

### CreateWithdrawResponse

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| status | string | yes | `ok` / `error` |
| data | long | no | 出金記録ID |

### CancelWithdrawRequest

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| withdraw-id | string | yes | Path パラメータ |

### CancelWithdrawResponse

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| status | string | yes | `ok` / `error` |
| data | long | no | 出金記録ID |

### GetDepositWithdrawsRequest

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| type | string | yes | `deposit` / `withdraw` |
| currency | string | no | 通貨コード |
| from | long | no | ページング |
| size | int | no | 件数 |
| direct | string | no | `prev` / `next` |

### GetDepositWithdrawsResponse

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| status | string | yes | `ok` / `error` |
| data | list | yes | 入出金記録 |

### CreateRetailOrderRequest

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| symbol | string | yes | 取引通貨 |
| type | int | yes | 1: buy, 2: sell |
| price | string | no | 価格 |
| amount | string | no | 数量 |
| cash_amount | string | no | 金額 |

### CreateRetailOrderResponse

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| code | int | yes | HTTP Status Code |
| data | long | no | 注文ID |
| message | string | no | メッセージ |
| success | bool | no | 成功可否 |

### GetRetailOrdersRequest

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| direct | int | yes | 1: 正方向 / 2: 逆方向 |
| status | int | no | 状態 |
| start_time | long | no | Unix ms |
| end_time | long | no | Unix ms |

### GetRetailOrdersResponse

| Field | Type | Required | Notes |
| --- | --- | --- | --- |
| code | int | yes | HTTP Status Code |
| data | list | no | 注文履歴 |
| message | string | no | メッセージ |
| success | bool | no | 成功可否 |

---

> Requests は “DTO の入口”。ApiMap は “Endpoint の入口”。
