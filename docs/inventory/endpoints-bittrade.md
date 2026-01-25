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

- https://api.bittrade.co.jp/docs

---

## 並び順について

本 inventory の endpoint 一覧は、**公式 API 文書における記載順**を正とする。
可読性や実装都合を理由とした並び替えは行わない。

---

## Columns

| Scope | Category | Method | Path | CanonicalSourceUrl | EndpointId | PresentIn | Note |
| ----- | -------- | ------ | ---- | ------------------ | ---------- | ---------- | ---- |

* **Scope**: public / private
* **Category**: MarketData / Trading / Account / History / Other
* **Method**: HTTP method（GET/POST/...）
* **Path**: API path（公式表記）
* **CanonicalSourceUrl**: 公式 API 文書における当該 endpoint の正本 URL（アンカー `#...` を含める）
* **EndpointId**: 本リポジトリでの識別子
* **PresentIn**: 当該 endpoint が存在する層（Wire / Raw / Normalized / Contracts）。空集合は `None`。
* **Note**: 任意（公式との差異・注意点など）

Note 欄には、以下のような **事実関係（状況）** を記載してよい。

* 重複候補（duplicate candidate）
* 旧版・非推奨の可能性（obsolete candidate）
* 非機能の可能性（non-functional candidate）
* version 並立の事実

Note 欄には、採用可否・実装判断・設計判断を記載してはならない。

---

## Public

| Scope  | Category   | Method | Path                  | CanonicalSourceUrl              | EndpointId      | PresentIn             | Note |
| ------ | ---------- | ------ | --------------------- | ------------------------------- | --------------- | --------------------- | ---- |
| public | Other      | GET    | /v1/common/symbols    | https://api.bittrade.co.jp/docs | GetSymbols      | Wire, Raw, Normalized |      |
| public | Other      | GET    | /v1/common/currencys  | https://api.bittrade.co.jp/docs | GetCurrencys    | Wire, Raw, Normalized |      |
| public | Other      | GET    | /v1/common/timestamp  | https://api.bittrade.co.jp/docs | GetTimestamp    | Wire, Raw, Normalized |      |
| public | MarketData | GET    | /market/history/kline | https://api.bittrade.co.jp/docs | GetHistoryKline | Wire, Raw, Normalized |      |
| public | MarketData | GET    | /market/detail/merged | https://api.bittrade.co.jp/docs | GetDetailMerged | Wire, Raw, Normalized |      |
| public | MarketData | GET    | /market/tickers       | https://api.bittrade.co.jp/docs | GetTickers      | Wire, Raw, Normalized |      |
| public | MarketData | GET    | /market/depth         | https://api.bittrade.co.jp/docs | GetDepth        | Wire, Raw, Normalized |      |
| public | MarketData | GET    | /market/trade         | https://api.bittrade.co.jp/docs | GetTrade        | Wire, Raw, Normalized |      |
| public | MarketData | GET    | /market/history/trade | https://api.bittrade.co.jp/docs | GetHistoryTrade | Wire, Raw, Normalized |      |

---

## Private

| Scope   | Category | Method | Path                                         | CanonicalSourceUrl              | EndpointId                            | PresentIn             | Note |
| ------- | -------- | ------ | -------------------------------------------- | ------------------------------- | ------------------------------------- | --------------------- | ---- |
| private | Account  | GET    | /v1/account/accounts                         | https://api.bittrade.co.jp/docs | GetAccounts                           | Wire, Raw, Normalized |      |
| private | Account  | GET    | /v1/account/accounts/{account-id}/balance    | https://api.bittrade.co.jp/docs | GetAccountsBalanceByAccountId         | Wire, Raw, Normalized |      |
| private | Trading  | POST   | /v1/order/orders/place                       | https://api.bittrade.co.jp/docs | PostOrdersPlace                       | Wire, Raw, Normalized |      |
| private | Trading  | GET    | /v1/order/openOrders                         | https://api.bittrade.co.jp/docs | GetOpenOrders                         | Wire, Raw, Normalized |      |
| private | Trading  | POST   | /v1/order/orders/{order-id}/submitcancel     | https://api.bittrade.co.jp/docs | PostOrdersSubmitCancelByOrderId       | Wire, Raw, Normalized |      |
| private | Trading  | POST   | /v1/order/orders/batchcancel                 | https://api.bittrade.co.jp/docs | PostOrdersBatchCancel                 | Wire, Raw, Normalized |      |
| private | Trading  | POST   | /v1/order/orders/batchCancelOpenOrders       | https://api.bittrade.co.jp/docs | PostOrdersBatchCancelOpenOrders       | Wire, Raw, Normalized |      |
| private | Trading  | GET    | /v1/order/orders/{order-id}                  | https://api.bittrade.co.jp/docs | GetOrdersByOrderId                    | Wire, Raw, Normalized |      |
| private | Trading  | GET    | /v1/order/orders                             | https://api.bittrade.co.jp/docs | GetOrders                             | Wire, Raw, Normalized |      |
| private | Trading  | GET    | /v1/order/orders/{order-id}/matchresults     | https://api.bittrade.co.jp/docs | GetOrdersMatchResultsByOrderId        | Wire, Raw, Normalized |      |
| private | Trading  | GET    | /v1/order/matchresults                       | https://api.bittrade.co.jp/docs | GetMatchResults                       | Wire, Raw, Normalized |      |
| private | Account  | POST   | /v1/dw/withdraw/api/create                   | https://api.bittrade.co.jp/docs | PostWithdrawApiCreate                 | Wire, Raw, Normalized |      |
| private | Account  | POST   | /v1/dw/withdraw-virtual/{withdraw-id}/cancel | https://api.bittrade.co.jp/docs | PostWithdrawVirtualCancelByWithdrawId | Wire, Raw, Normalized |      |
| private | Account  | GET    | /v1/query/deposit-withdraw                   | https://api.bittrade.co.jp/docs | GetDepositWithdraw                    | Wire, Raw, Normalized |      |
| private | Trading  | POST   | /v1/retail/order/place                       | https://api.bittrade.co.jp/docs | PostOrderPlace                        | Wire, Raw, Normalized |      |
| private | Trading  | GET    | /v1/retail/order/list                        | https://api.bittrade.co.jp/docs | GetOrderList                          | Wire, Raw, Normalized |      |
| private | Account  | GET    | /v1/retail/maintain/time                     | https://api.bittrade.co.jp/docs | GetMaintainTime                       | Wire, Raw, Normalized |      |

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
