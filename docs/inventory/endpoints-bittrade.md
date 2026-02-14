# Inventory — Bittrade Endpoints

> 本文書は **inventory（事実一覧）** です。
> 公式 API 文書を参照し、Method / Path / CanonicalSourceUrl / EndpointId / PresentIn の対応関係のみを記録します。
> 本文書は判断規則を定義しません。

## Canonical Source（Entrypoint）

- https://api-doc.bittrade.co.jp/

---

## 並び順について

本 inventory の endpoint 一覧は、公式 API 文書の記載順を基準として記録している。

---

## Columns

| Scope | Method | Path | CanonicalSourceUrl | EndpointId | PresentIn | Note | RequestType | ResponseType |
| ----- | ------ | ---- | ------------------ | ---------- | --------- | ---- | ----------- | ------------ |

* **Scope**: public / private
* **Method**: HTTP method（GET/POST/...）
* **Path**: API path（公式表記）
* **CanonicalSourceUrl**: 公式 API 文書における当該 endpoint の正本 URL。Bittrade については、公式 HTML 文書に実在する `id` 属性をフラグメント（`#...`）として付与した URL を用いる。
* **EndpointId**: 本リポジトリでの識別子
* **PresentIn**: 当該 endpoint が存在する層（Wire / Raw / Normalized / Contracts）。空集合は `None`。
* **Note**: 任意（公式との差異・注意点など）
* **RequestType**: 取引所側の正準 Request 型名（存在しない場合は `None`）
* **ResponseType**: 取引所側の正準 Response 型名（存在しない場合は `None`）

Note 欄には、以下のような **事実関係（状況）** を記載してよい。

* 重複候補（duplicate candidate）
* 旧版・非推奨の可能性（obsolete candidate）
* 非機能の可能性（non-functional candidate）
* version 並立の事実

Note 欄には、採用可否・実装判断・設計判断を記載してはならない。

---

## Public

| Scope | Method | Path | CanonicalSourceUrl | EndpointId | PresentIn | Note | RequestType | ResponseType |
| ------ | ------ | --------------------- | ------------------------------- | --------------- | --------------------- | ---- | ----------- | ------------ |
| public | GET | /v1/common/symbols | https://api-doc.bittrade.co.jp/#384f6851b3 | GetSymbols | Wire, Raw, Normalized |  | GetSymbolsRequest | GetSymbolsResponse |
| public | GET | /v1/common/currencys | https://api-doc.bittrade.co.jp/#3cb389c6a0 | GetCurrencies | Wire, Raw, Normalized |  | GetCurrenciesRequest | GetCurrenciesResponse |
| public | GET | /v1/common/timestamp | https://api-doc.bittrade.co.jp/#de96e45aa6 | GetTimestamp | Wire, Raw, Normalized |  | GetTimestampRequest | GetTimestampResponse |
| public | GET | /market/history/kline | https://api-doc.bittrade.co.jp/#ed8d1d68d7 | GetHistoryKline | Wire, Raw, Normalized |  | GetHistoryKlineRequest | GetHistoryKlineResponse |
| public | GET | /market/detail/merged | https://api-doc.bittrade.co.jp/#83bc409c24 | GetDetailMerged | Wire, Raw, Normalized |  | GetDetailMergedRequest | GetDetailMergedResponse |
| public | GET | /market/tickers | https://api-doc.bittrade.co.jp/#024e7e4d2e | GetTickers | Wire, Raw, Normalized |  | GetTickersRequest | GetTickersResponse |
| public | GET | /market/depth | https://api-doc.bittrade.co.jp/#91377eb7d7 | GetDepth | Wire, Raw, Normalized |  | GetDepthRequest | GetDepthResponse |
| public | GET | /market/trade | https://api-doc.bittrade.co.jp/#15f00772c5 | GetTrade | Wire, Raw, Normalized |  | GetTradeRequest | GetTradeResponse |
| public | GET | /market/history/trade | https://api-doc.bittrade.co.jp/#15f00772c5 | GetHistoryTrade | Wire, Raw, Normalized |  | GetHistoryTradeRequest | GetHistoryTradeResponse |

---

## Private

| Scope | Method | Path | CanonicalSourceUrl | EndpointId | PresentIn | Note | RequestType | ResponseType |
| ------- | ------ | -------------------------------------------- | ------------------------------- | ------------------------------------- | --------------------- | ---- | ----------- | ------------ |
| private | GET | /v1/account/accounts | https://api-doc.bittrade.co.jp/#eda1f800b0 | GetAccounts | Wire, Raw, Normalized |  | GetAccountsRequest | GetAccountsResponse |
| private | GET | /v1/account/accounts/{account-id}/balance | https://api-doc.bittrade.co.jp/#c617e5c5d4 | GetAccountsBalanceByAccountId | Wire, Raw, Normalized |  | GetAccountsBalanceByAccountIdRequest | GetAccountsBalanceByAccountIdResponse |
| private | POST | /v1/order/orders/place | https://api-doc.bittrade.co.jp/#bea621a911 | PostOrdersPlace | Wire, Raw, Normalized |  | PostOrdersPlaceRequest | PostOrdersPlaceResponse |
| private | GET | /v1/order/openOrders | https://api-doc.bittrade.co.jp/#c9b851ba3b | GetOpenOrders | Wire, Raw, Normalized |  | GetOpenOrdersRequest | GetOpenOrdersResponse |
| private | POST | /v1/order/orders/{order-id}/submitcancel | https://api-doc.bittrade.co.jp/#75e116b1eb | PostOrdersSubmitCancelByOrderId | Wire, Raw, Normalized |  | PostOrdersSubmitCancelByOrderIdRequest | PostOrdersSubmitCancelByOrderIdResponse |
| private | POST | /v1/order/orders/batchcancel | https://api-doc.bittrade.co.jp/#7fd8579ed8 | PostOrdersBatchCancel | Wire, Raw, Normalized |  | PostOrdersBatchCancelRequest | PostOrdersBatchCancelResponse |
| private | POST | /v1/order/orders/batchCancelOpenOrders | https://api-doc.bittrade.co.jp/#eb810ec4c9 | PostOrdersBatchCancelOpenOrders | Wire, Raw, Normalized |  | PostOrdersBatchCancelOpenOrdersRequest | PostOrdersBatchCancelOpenOrdersResponse |
| private | GET | /v1/order/orders/{order-id} | https://api-doc.bittrade.co.jp/#1b7a9b2d17 | GetOrdersByOrderId | Wire, Raw, Normalized |  | GetOrdersByOrderIdRequest | GetOrdersByOrderIdResponse |
| private | GET | /v1/order/orders | https://api-doc.bittrade.co.jp/#7f3b90d8ef | GetOrders | Wire, Raw, Normalized |  | GetOrdersRequest | GetOrdersResponse |
| private | GET | /v1/order/orders/{order-id}/matchresults | https://api-doc.bittrade.co.jp/#2e3f5b1a3b | GetOrdersMatchResultsByOrderId | Wire, Raw, Normalized |  | GetOrdersMatchResultsByOrderIdRequest | GetOrdersMatchResultsByOrderIdResponse |
| private | GET | /v1/order/matchresults | https://api-doc.bittrade.co.jp/#2d2f47dc2e | GetMatchResults | Wire, Raw, Normalized |  | GetMatchResultsRequest | GetMatchResultsResponse |
| private | POST | /v1/dw/withdraw/api/create | https://api-doc.bittrade.co.jp/#5b3ccd3202 | PostWithdrawApiCreate | Wire, Raw, Normalized |  | PostWithdrawApiCreateRequest | PostWithdrawApiCreateResponse |
| private | POST | /v1/dw/withdraw-virtual/{address-id}/create | https://api-doc.bittrade.co.jp/#83a34edb53 | PostWithdrawVirtualByAddressIdCreate | Wire, Raw, Normalized |  | PostWithdrawVirtualByAddressIdCreateRequest | PostWithdrawVirtualByAddressIdCreateResponse |
| private | POST | /v1/dw/withdraw-virtual/{withdraw-id}/cancel | https://api-doc.bittrade.co.jp/#53a0f43b78 | PostWithdrawVirtualByWithdrawIdCancel | Wire, Raw, Normalized |  | PostWithdrawVirtualByWithdrawIdCancelRequest | PostWithdrawVirtualByWithdrawIdCancelResponse |
| private | POST | /v1/dw/withdraw-virtual/{withdraw-id}/place | https://api-doc.bittrade.co.jp/#8db2c2bd10 | PostWithdrawVirtualByWithdrawIdPlace | Wire, Raw, Normalized |  | PostWithdrawVirtualByWithdrawIdPlaceRequest | PostWithdrawVirtualByWithdrawIdPlaceResponse |
| private | GET | /v1/dw/withdraw-virtual/addresses | https://api-doc.bittrade.co.jp/#2d0b76b1b3 | GetWithdrawVirtualAddresses | Wire, Raw, Normalized |  | GetWithdrawVirtualAddressesRequest | GetWithdrawVirtualAddressesResponse |
| private | GET | /v1/query/deposit-withdraw | https://api-doc.bittrade.co.jp/#0091062ee7 | GetDepositWithdraw | Wire, Raw, Normalized |  | GetDepositWithdrawRequest | GetDepositWithdrawResponse |
| private | POST | /v1/retail/order/place | https://api-doc.bittrade.co.jp/#d7bd4f7428 | PostRetailOrderPlace | Wire, Raw, Normalized |  | PostRetailOrderPlaceRequest | PostRetailOrderPlaceResponse |
| private | GET | /v1/retail/order/list | https://api-doc.bittrade.co.jp/#19f52c5bd6 | GetRetailOrderList | Wire, Raw, Normalized |  | GetRetailOrderListRequest | GetRetailOrderListResponse |
| private | GET | /v1/retail/order/detail/{orderId} | https://api-doc.bittrade.co.jp/#13a5e9b4c2 | GetRetailOrderDetailByOrderId | Wire, Raw, Normalized |  | GetRetailOrderDetailByOrderIdRequest | GetRetailOrderDetailByOrderIdResponse |
| private | POST | /v1/retail/order/cancel/{order-id} | https://api-doc.bittrade.co.jp/#0d2bdc0c1c | PostRetailOrderCancelByOrderId | Wire, Raw, Normalized |  | PostRetailOrderCancelByOrderIdRequest | PostRetailOrderCancelByOrderIdResponse |
| private | GET | /v1/retail/account/balance | https://api-doc.bittrade.co.jp/#a3a6b4e0e3 | GetRetailAccountBalance | Wire, Raw, Normalized |  | GetRetailAccountBalanceRequest | GetRetailAccountBalanceResponse |
| private | POST | /v1/retail/order/history | https://api-doc.bittrade.co.jp/#19f52c5bd6 | PostRetailOrderHistory | Wire, Raw, Normalized |  | PostRetailOrderHistoryRequest | PostRetailOrderHistoryResponse |
| private | POST | /v1/retail/order/detail | https://api-doc.bittrade.co.jp/#19f52c5bd6 | PostRetailOrderDetail | Wire, Raw, Normalized |  | PostRetailOrderDetailRequest | PostRetailOrderDetailResponse |
| private | POST | /v1/retail/order/create | https://api-doc.bittrade.co.jp/#19f52c5bd6 | PostRetailOrderCreate | Wire, Raw, Normalized |  | PostRetailOrderCreateRequest | PostRetailOrderCreateResponse |

---

## Aliases（任意）

本 inventory の `EndpointId` 列に alias を記載してはならない。
alias を記録する場合は、本セクションに `EndpointId` との対応として記載する。

| EndpointId | Alias | Notes |
|---|---|---|

## Notes（参考・非規範）

* 本 inventory は **一覧のみ** を目的とする。
* EndpointId の意味・命名・層対応は TopSpec を参照する。
