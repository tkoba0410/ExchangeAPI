# Bittrade REST API 実装一覧（Raw 層・補助：分解ビュー）

本ドキュメントは、Raw-only 正本（`ApiMap`）を **命名規則の構成要素に分解**して可視化する補助ビューです。

* 目的：命名レビュー（Verb / Noun / ByCondition）と Request DTO 判断を機械的に行う
* 本表は補助であり、更新の正本は **Raw-only 正本**を優先する

---

## 命名分解の例（Bittrade）

| HTTP METHOD | Path | Verb | Noun | ByCondition | Method（合成） | 備考 |
| --- | --- | --- | --- | --- | --- | --- |
| GET | /market/detail/merged | Get | MergedTicker | BySymbol | GetMergedTickerAsync | `symbol` は必須 Query |
| GET | /market/depth | Get | Depth | BySymbol | GetDepthAsync | `type` は任意 Query |
| GET | /market/history/kline | Get | Klines | BySymbol | GetKlinesAsync | `period` と `size` を併用 |
| GET | /v1/common/symbols | Get | Symbols | - | GetSymbolsAsync | Public API |
| POST | /v1/order/orders/place | Create | Order | - | CreateOrderAsync | Body DTO を使用 |
| POST | /v1/order/orders/{order-id}/submitcancel | Cancel | Order | ByOrderId | CancelOrderAsync | Path で識別 |
| GET | /v1/order/openOrders | Get | OpenOrders | - | GetOpenOrdersAsync | `symbol` / `account-id` |
| POST | /v1/dw/withdraw/api/create | Create | Withdraw | - | CreateWithdrawAsync | Body DTO を使用 |
| POST | /v1/retail/order/place | Create | RetailOrder | - | CreateRetailOrderAsync | Retail 系の response 形式 |

---

## 注記（レビュー観点）

* **ByCondition は原則 Path 識別子のみ**。
  * Bittrade では `symbol` が必須 Query のため、補助的に `BySymbol` を記述します。
* Body を伴う API は **Create/Cancel/Update/Delete** の動詞を優先します。
* 命名の正本は `../../Raw/Naming.md` に従います。

---

> 補助ビューは「規則に当てたらこう見える」を提供する。正本は Raw-only ApiMap。
