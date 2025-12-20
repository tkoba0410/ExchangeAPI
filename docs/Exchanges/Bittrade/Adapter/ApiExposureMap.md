# Bittrade 抽象層公開状況（補助ビュー）

本ドキュメントは、**Bittrade の Adapter/Facade が公開している API** を整理した補助ビューです。  
正本（Raw-only）は `../Raw/ApiMap.md` です。

* Raw-only の一覧は正本に集約し、ここでは **抽象層で公開している API と代表メソッド**を示します。
* 命名規則は `../../Raw/Naming.md`（Raw）および `../../Adapter/Naming.md`（Adapter）を参照してください。

---

## 公開 API 一覧（抽象層）

| 分類        | 概要                          | 代表メソッド例 |
| ----------- | ----------------------------- | -------------- |
| Market      | Ticker / OrderBook / Executions | `GetTickerAsync`, `GetOrderBookAsync`, `GetMarketExecutionsAsync` |
| Trading     | Place / Cancel / Orders / Status | `PlaceMarketOrderAsync`, `CancelOrderAsync`, `GetOrdersAsync`, `GetOrderAsync` |
| Account     | Balances                      | `GetBalancesAsync` |
| ExchangeInfo| Market metadata               | `GetExchangeInfoAsync` |
| Margin      | Not supported                 | `GetOpenPositionsAsync` / `GetCollateralAsync` -> `ExchangeFeatureNotSupportedException` |

---

## Raw エンドポイント別の公開状況

| Path | Adapter 露出 | 抽象メソッド |
| --- | --- | --- |
| /v1/common/symbols | ○ | `BittradeExchangeInfoApi.GetSymbolsAsync` |
| /v1/common/currencys | - | - |
| /v1/common/timestamp | ○ | `BittradeMarketDataApi.GetTimestampAsync` |
| /market/history/kline | - | - |
| /market/detail/merged | ○ | `BittradeMarketDataApi.GetTickerAsync` |
| /market/tickers | - | - |
| /market/depth | ○ | `BittradeMarketDataApi.GetOrderBookAsync` |
| /market/trade | ○ | `BittradeMarketDataApi.GetMarketExecutionsAsync` |
| /market/history/trade | - | - |
| /v1/retail/maintain/time | - | - |
| /v1/account/accounts | - | - |
| /v1/account/accounts/{account-id}/balance | ○ | `BittradeTradingApi.GetBalancesAsync` |
| /v1/order/orders/place | ○ | `BittradeTradingApi.PlaceLimitOrderAsync` / `PlaceMarketOrderAsync` |
| /v1/order/openOrders | ○ | `BittradeTradingApi.GetOrdersAsync` |
| /v1/order/orders/{order-id}/submitcancel | ○ | `BittradeTradingApi.CancelOrderAsync` |
| /v1/order/orders/batchcancel | - | - |
| /v1/order/orders/batchCancelOpenOrders | - | - |
| /v1/order/orders/{order-id} | ○ | `BittradeTradingApi.GetOrderAsync` |
| /v1/order/orders/{order-id}/matchresults | - | - |
| /v1/order/orders | - | - |
| /v1/order/matchresults | - | - |
| /v1/dw/withdraw/api/create | - | - |
| /v1/dw/withdraw-virtual/{withdraw-id}/cancel | - | - |
| /v1/query/deposit-withdraw | - | - |
| /v1/retail/order/place | - | - |
| /v1/retail/order/list | - | - |

---

## 注記

* 抽象層で未露出の API は、意図的に Raw に限定されているものを含みます。
* 新規公開の判断は、ユースケースと互換性を優先して行います。
