# Inventory — Bittrade Endpoints

> 本文書は **一覧（inventory）** である。
> 技術規範・共通方針は **TopSpec（docs/topspec.md）** を正本とする。
> 公式 API 文書を最上位の正本とし、ここでは対応関係と命名規約を管理する。

## Normative Scope

本書は、EndpointId および対応する事実（Method, Path, Scope 等）を列挙する **Normative Inventory** である。

以下は本書の対象外とする。

* 通称、代表名、便宜的名称
* ナビゲーション目的の分類語
* EndpointId の命名規範や導出・派生規則そのもの
* 実装上の補助的な識別子

これらは、本書の内容から直接または間接に導出されてはならない。

※ Get / Send / Cancel 等の接頭辞は、EndpointId の構文要素であり、
  便宜的名称・通称・代表名には該当しない。

本文書は、TopSpec に基づき決定された EndpointId の一覧を記録するものであり、
命名規範や派生規則そのものを定義するものではない。

## EndpointId ルール（Bittrade）

- EndpointId は **取引所スコープ**の識別子とする。
- Endpoint の区別や衝突回避のため、
  HTTP Method を表す語（Get / Post 等）を
  prefix として用いることを許容する。
- 命名は、公式 API の操作単位を優先して表現する。
- 本文書に記載された EndpointId が、
  Bittrade における正本である。

## Canonical Source（Entrypoint）

- https://api-doc.bittrade.co.jp/

---

## 並び順について

本 inventory の endpoint 一覧は、**公式 API 文書における記載順**を正とする。
可読性や実装都合を理由とした並び替えは行わない。

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
| public | GET | /v1/common/currencys | https://api-doc.bittrade.co.jp/#3cb389c6a0 | GetCurrencys | Wire, Raw, Normalized |  | GetCurrencysRequest | GetCurrencysResponse |
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

### EndpointId 導出手順（参考）

以下は、本 inventory に記載された EndpointId を導出する際に用いた手順の一例である。
この手順自体は **設計規範ではない**（衝突時は TopSpec を優先する）。

1. Path 先頭の `/` を除去する

2. 先頭セグメントが version（例：`v1`）である場合は除去する

3. その後の **先頭セグメントを 1 つ除去する**（取引所仕様上の prefix）

4. 残りを `/` で分割し、空要素を除外する

5. 各セグメントを TopSpec が定める一般単語境界に基づいて分割する

   * `{...}` 形式の path parameter は、Path 上からは除去する
   * path parameter が存在した場合は、parameter 名を PascalCase 化し、`By<ParameterName>` を EndpointId 末尾に付与する

6. 分割された各単語を PascalCase 化し、連結する

7. HTTP Method を PascalCase 化し、EndpointId の **先頭**に付与する（例：`GET`→`Get`、`POST`→`Post`）
