# bitFlyer 抽象層公開状況（補助ビュー）

本ドキュメントは、**bitFlyer の Adapter/Facade が公開している API** を整理した補助ビューです。  
正本（Raw-only）は `../Raw/ApiMap.md` です。

* Raw-only の一覧は正本に集約し、ここでは **抽象層で公開している API と代表メソッド**を示します。
* 命名規則は `../../Raw/Naming.md`（Raw）および `../../Adapter/Naming.md`（Adapter）を参照してください。

---

## 公開 API 一覧（抽象層）

| 分類        | 概要                          | 代表メソッド例 |
| ----------- | ----------------------------- | -------------- |
| Market      | Ticker / OrderBook / Executions | `GetTickerAsync`, `GetOrderBookAsync`, `GetMarketExecutionsAsync` |
| Trading     | Place / Cancel / Orders / Status | `PlaceMarketOrderAsync`, `CancelOrderAsync`, `GetOrdersAsync`, `GetOrderAsync` |
| Account     | Balances / AccountExecutions  | `GetBalancesAsync`, `GetAccountExecutionsAsync` |
| Margin      | Positions / Collateral        | `GetOpenPositionsAsync`, `GetCollateralAsync` |
| ExchangeInfo| Market metadata               | `GetExchangeInfoAsync` |

---

## 注記

* 抽象層で未露出の API は、意図的に Raw に限定されているものを含みます。
* 新規公開の判断は、ユースケースと互換性を優先して行います。
