# Inventory — Bittrade Endpoints

> 本文書は **一覧（inventory）** です。
> 仕様判断・設計規範は **TopSpec（docs/topspec.md）** を正本とします。
> 公式 API 文書を正本とし、ここでは対応関係のみを管理します。

---

## Columns

| Scope | Category | Method | Path | EndpointId | Note |
| ----- | -------- | ------ | ---- | ---------- | ---- |

* **Scope**: public / private
* **Category**: MarketData / Trading / Account / History / Other
* **Method**: HTTP method（GET/POST/...）
* **Path**: API path（公式表記）
* **EndpointId**: 本リポジトリでの識別子
* **Note**: 任意（公式との差異・注意点など）

---

## Public

| Scope  | Category | Method | Path                 | EndpointId      | Note |
| ------ | -------- | ------ | -------------------- | --------------- | ---- |
| public | Other    | GET    | /v1/common/symbols   | GetSymbols      |      |
| public | Other    | GET    | /v1/common/currencys | GetCurrencys    |      |
| public | Other    | GET    | /v1/common/timestamp | GetTimestamp    |      |
| public | MarketData | GET    | /market/history/kline | GetHistoryKline |      |
| public | MarketData | GET    | /market/detail/merged | GetDetailMerged |      |
| public | MarketData | GET    | /market/tickers       | GetTickers      |      |
| public | MarketData | GET    | /market/depth         | GetDepth        |      |
| public | MarketData | GET    | /market/trade         | GetTrade        |      |
| public | MarketData | GET    | /market/history/trade | GetHistoryTrade |      |

---

## Private

| Scope   | Category | Method | Path                                           | EndpointId                            | Note |
| ------- | -------- | ------ | ---------------------------------------------- | ------------------------------------- | ---- |
| private | Account  | GET    | /v1/account/accounts                         | GetAccounts                           |      |
| private | Account  | GET    | /v1/account/accounts/{account-id}/balance    | GetAccountsBalanceByAccountId         |      |
| private | Trading  | POST   | /v1/order/orders/place                       | PostOrdersPlace                       |      |
| private | Trading  | GET    | /v1/order/openOrders                         | GetOpenOrders                         |      |
| private | Trading  | POST   | /v1/order/orders/{order-id}/submitcancel     | PostOrdersSubmitCancelByOrderId       |      |
| private | Trading  | POST   | /v1/order/orders/batchcancel                 | PostOrdersBatchCancel                 |      |
| private | Trading  | POST   | /v1/order/orders/batchCancelOpenOrders       | PostOrdersBatchCancelOpenOrders       |      |
| private | Trading  | GET    | /v1/order/orders/{order-id}                  | GetOrdersByOrderId                    |      |
| private | Trading  | GET    | /v1/order/orders                             | GetOrders                             |      |
| private | Trading  | GET    | /v1/order/orders/{order-id}/matchresults     | GetOrdersMatchResultsByOrderId        |      |
| private | Trading  | GET    | /v1/order/matchresults                       | GetMatchResults                       |      |
| private | Account  | POST   | /v1/dw/withdraw/api/create                   | PostWithdrawApiCreate                 |      |
| private | Account  | POST   | /v1/dw/withdraw-virtual/{withdraw-id}/cancel | PostWithdrawVirtualCancelByWithdrawId |      |
| private | Account  | GET    | /v1/query/deposit-withdraw                   | GetDepositWithdraw                    |      |
| private | Trading  | POST   | /v1/retail/order/place                       | PostOrderPlace                        |      |
| private | Trading  | GET    | /v1/retail/order/list                        | GetOrderList                          |      |
| private | Account  | GET    | /v1/retail/maintain/time                     | GetMaintainTime                       |      |

---

## Notes

* 本 inventory は **一覧のみ** を目的とし、層構造・責務・公開範囲の規範は記載しません。
* EndpointId の意味・命名・層対応は TopSpec を参照してください。
