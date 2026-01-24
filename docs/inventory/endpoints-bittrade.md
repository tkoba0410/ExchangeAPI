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

---

## 並び順について

本 inventory の endpoint 一覧は、**公式 API 文書における記載順**を正とする。
可読性や実装都合を理由とした並び替えは行わない。

---

## Columns

| Scope | Category | Method | Path | EndpointId | RequiredIn | Note |
| ----- | -------- | ------ | ---- | ---------- | ---------- | ---- |

* **Scope**: public / private
* **Category**: MarketData / Trading / Account / History / Other
* **Method**: HTTP method（GET/POST/...）
* **Path**: API path（公式表記）
* **EndpointId**: 本リポジトリでの識別子
* **RequiredIn**: 当該 endpoint を提供する層（Wire / Raw / Normalized / Contracts）
* **Note**: 任意（公式との差異・注意点など）

---

## Public

| Scope  | Category   | Method | Path                  | EndpointId      | RequiredIn | Note |
| ------ | ---------- | ------ | --------------------- | --------------- | ---------- | ---- |
| public | Other      | GET    | /v1/common/symbols    | GetSymbols      |            |      |
| public | Other      | GET    | /v1/common/currencys  | GetCurrencys    |            |      |
| public | Other      | GET    | /v1/common/timestamp  | GetTimestamp    |            |      |
| public | MarketData | GET    | /market/history/kline | GetHistoryKline |            |      |
| public | MarketData | GET    | /market/detail/merged | GetDetailMerged |            |      |
| public | MarketData | GET    | /market/tickers       | GetTickers      |            |      |
| public | MarketData | GET    | /market/depth         | GetDepth        |            |      |
| public | MarketData | GET    | /market/trade         | GetTrade        |            |      |
| public | MarketData | GET    | /market/history/trade | GetHistoryTrade |            |      |

---

## Private

| Scope   | Category | Method | Path                                         | EndpointId                            | RequiredIn | Note |
| ------- | -------- | ------ | -------------------------------------------- | ------------------------------------- | ---------- | ---- |
| private | Account  | GET    | /v1/account/accounts                         | GetAccounts                           |            |      |
| private | Account  | GET    | /v1/account/accounts/{account-id}/balance    | GetAccountsBalanceByAccountId         |            |      |
| private | Trading  | POST   | /v1/order/orders/place                       | PostOrdersPlace                       |            |      |
| private | Trading  | GET    | /v1/order/openOrders                         | GetOpenOrders                         |            |      |
| private | Trading  | POST   | /v1/order/orders/{order-id}/submitcancel     | PostOrdersSubmitCancelByOrderId       |            |      |
| private | Trading  | POST   | /v1/order/orders/batchcancel                 | PostOrdersBatchCancel                 |            |      |
| private | Trading  | POST   | /v1/order/orders/batchCancelOpenOrders       | PostOrdersBatchCancelOpenOrders       |            |      |
| private | Trading  | GET    | /v1/order/orders/{order-id}                  | GetOrdersByOrderId                    |            |      |
| private | Trading  | GET    | /v1/order/orders                             | GetOrders                             |            |      |
| private | Trading  | GET    | /v1/order/orders/{order-id}/matchresults     | GetOrdersMatchResultsByOrderId        |            |      |
| private | Trading  | GET    | /v1/order/matchresults                       | GetMatchResults                       |            |      |
| private | Account  | POST   | /v1/dw/withdraw/api/create                   | PostWithdrawApiCreate                 |            |      |
| private | Account  | POST   | /v1/dw/withdraw-virtual/{withdraw-id}/cancel | PostWithdrawVirtualCancelByWithdrawId |            |      |
| private | Account  | GET    | /v1/query/deposit-withdraw                   | GetDepositWithdraw                    |            |      |
| private | Trading  | POST   | /v1/retail/order/place                       | PostOrderPlace                        |            |      |
| private | Trading  | GET    | /v1/retail/order/list                        | GetOrderList                          |            |      |
| private | Account  | GET    | /v1/retail/maintain/time                     | GetMaintainTime                       |            |      |

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
