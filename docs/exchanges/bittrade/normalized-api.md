# Bittrade API 一覧（Public / Private × 種類別）

本書は `ExchangeApi.Exchanges.Bittrade.Wire.Constants.BittradeConstants.Paths` を正本とし、
Bittrade の API を **Public / Private** に分け、さらに **種類（Market / Common / Account / Order / Retail / Finance）** ごとに整理した一覧である。

* 各行は **1 API = 1 行**
* 列項目は **HTTP Method / Path / Normalized API 名** のみ
* Normalized API 名は **Bitflyer 最終確定ルール**を適用

  * METHOD は命名に使用しない
  * `/v1` / `/v1/me` は除外
  * 残りパスのみ使用
  * 単語境界は細かく分割（PascalCase）

---

## Public API

### Market

| Method | Path                 | API 名                          |
| ------ | -------------------- | ------------------------------ |
| GET    | market/detail/merged | GetMarketDetailMergedCallAsync |
| GET    | market/depth         | GetMarketDepthCallAsync        |
| GET    | market/trade         | GetMarketTradeCallAsync        |
| GET    | market/history/kline | GetMarketHistoryKlineCallAsync |
| GET    | market/tickers       | GetMarketTickersCallAsync      |
| GET    | market/history/trade | GetMarketHistoryTradeCallAsync |

---

### Common

| Method | Path                    | API 名                          |
| ------ | ----------------------- | ------------------------------ |
| GET    | v1/common/timestamp     | GetCommonTimestampCallAsync    |
| GET    | v1/common/symbols       | GetCommonSymbolsCallAsync      |
| GET    | v1/common/currencys     | GetCommonCurrenciesCallAsync   |
| GET    | v1/retail/maintain/time | GetRetailMaintainTimeCallAsync |

---

## Private API

### Account

| Method | Path                | API 名                       |
| ------ | ------------------- | --------------------------- |
| GET    | v1/account/accounts | GetAccountAccountsCallAsync |

---

### Order

| Method | Path                                  | API 名                                         |
| ------ | ------------------------------------- | --------------------------------------------- |
| GET    | v1/order/openOrders                   | GetOrderOpenOrdersCallAsync                   |
| GET    | v1/order/orders                       | GetOrderOrdersCallAsync                       |
| GET    | v1/order/matchresults                 | GetOrderMatchResultsCallAsync                 |
| POST   | v1/order/orders/place                 | PostOrderOrdersPlaceCallAsync                 |
| POST   | v1/order/orders/batchcancel           | PostOrderOrdersBatchCancelCallAsync           |
| POST   | v1/order/orders/batchCancelOpenOrders | PostOrderOrdersBatchCancelOpenOrdersCallAsync |

---

### Retail

| Method | Path                  | API 名                         |
| ------ | --------------------- | ----------------------------- |
| GET    | v1/retail/order/list  | GetRetailOrderListCallAsync   |
| POST   | v1/retail/order/place | PostRetailOrderPlaceCallAsync |

---

### Finance（Deposit / Withdraw）

| Method | Path                      | API 名                            |
| ------ | ------------------------- | -------------------------------- |
| GET    | v1/query/deposit-withdraw | GetQueryDepositWithdrawCallAsync |
| POST   | v1/dw/withdraw/api/create | PostDwWithdrawApiCreateCallAsync |
| POST   | v1/dw/withdraw-virtual    | PostDwWithdrawVirtualCallAsync   |

---

## 注記

* Public / Private の区分は **公式ドキュメントおよび API 構造**に基づく
* HTTP Method は Wire 層実装を正とする
* Normalized API 名は衝突回避・利便性調整を行っていない
* 次工程で Capability / Facade / Client への割当を検討する
