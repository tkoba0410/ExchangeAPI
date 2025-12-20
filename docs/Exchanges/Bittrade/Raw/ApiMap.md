# Bittrade REST API 実装一覧（Raw 層・正本）

本ドキュメントは、**Bittrade REST API** に対して、
現在 **Raw 層で実装済みの HTTP エンドポイント**を、
**Raw 層の要素のみに限定して**一覧化した正本ドキュメントです。

* 表は **仕様書の提示順**に沿って整理します
* 本表は **Raw 層の事実のみ**を扱います
* 抽象層（Adapter / Facade）での公開状況は本表の責務外です

> Raw 層は公式 API の鏡像であり、未露出 API も意図的に保持されます。

---

## API 実装対応表（Raw only）

| HTTP METHOD | Path | 種別 | Raw | Raw メソッド名 | 主要パラメータ |
| --- | --- | --- | --- | --- | --- |
| GET | /v1/common/symbols | Public | ○ | GetSymbolsAsync | - |
| GET | /v1/common/currencys | Public | × | GetCurrenciesAsync | - |
| GET | /v1/common/timestamp | Public | × | GetTimestampAsync | - |
| GET | /market/history/kline | Public | × | GetKlinesAsync | symbol, period, size |
| GET | /market/detail/merged | Public | ○ | GetMergedTickerAsync | symbol |
| GET | /market/tickers | Public | × | GetTickersAsync | - |
| GET | /market/depth | Public | ○ | GetDepthAsync | symbol, type |
| GET | /market/trade | Public | ○ | GetTradesAsync | symbol |
| GET | /market/history/trade | Public | × | GetTradeHistoryAsync | symbol |
| GET | /v1/retail/maintain/time | Public | × | GetRetailMaintainTimeAsync | - |
| GET | /v1/account/accounts | Private(Auth) | ○ | GetAccountsAsync | - |
| GET | /v1/account/accounts/{account-id}/balance | Private(Auth) | ○ | GetAccountBalanceAsync | account-id |
| POST | /v1/order/orders/place | Private(Auth) | ○ | CreateOrderAsync | account-id, symbol, type, amount, price? |
| GET | /v1/order/openOrders | Private(Auth) | ○ | GetOpenOrdersAsync | symbol, account-id |
| POST | /v1/order/orders/{order-id}/submitcancel | Private(Auth) | ○ | CancelOrderAsync | order-id |
| POST | /v1/order/orders/batchcancel | Private(Auth) | × | CancelOrdersAsync | order-ids[] |
| POST | /v1/order/orders/batchCancelOpenOrders | Private(Auth) | × | CancelOpenOrdersAsync | account-id, symbol, side?, size?, price?, created-at? |
| GET | /v1/order/orders/{order-id} | Private(Auth) | ○ | GetOrderAsync | order-id |
| GET | /v1/order/orders/{order-id}/matchresults | Private(Auth) | × | GetOrderMatchResultsAsync | order-id |
| GET | /v1/order/orders | Private(Auth) | × | GetOrdersAsync | symbol, states, start-date, end-date, from, direct, size |
| GET | /v1/order/matchresults | Private(Auth) | × | GetMatchResultsAsync | symbol, types, start-date, end-date, from, direct, size |
| POST | /v1/dw/withdraw/api/create | Private(Auth) | × | CreateWithdrawAsync | address, amount, currency, fee?, addr-tag? |
| POST | /v1/dw/withdraw-virtual/{withdraw-id}/cancel | Private(Auth) | × | CancelWithdrawAsync | withdraw-id |
| GET | /v1/query/deposit-withdraw | Private(Auth) | × | GetDepositWithdrawsAsync | type, currency, from, size, direct |
| POST | /v1/retail/order/place | Private(Auth) | × | CreateRetailOrderAsync | symbol, type, price?, amount?, cash_amount? |
| GET | /v1/retail/order/list | Private(Auth) | × | GetRetailOrdersAsync | direct, status?, start_time?, end_time? |

---

## 注記

* 本表は Raw 層の **唯一の正本**です
* 抽象層での公開状況は Adapter 側ドキュメントを参照してください
* Raw API の命名および Request DTO は `../../Raw/Naming.md` の規則に従います

---

> Raw first. Faithful mapping. No abstraction.
