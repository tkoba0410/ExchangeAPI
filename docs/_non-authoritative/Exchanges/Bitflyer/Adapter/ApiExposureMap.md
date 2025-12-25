# 非公式文書（参考資料）

> ⚠ 非公式文書（Non-Authoritative）
>
> 本ディレクトリ配下の文書は参考資料であり、公式仕様ではない。
> 本リポジトリにおける唯一の公式仕様（source of truth）は `docs/TopSpec.md` である。
>
> 内容が TopSpec と矛盾する場合、必ず TopSpec を正とする。

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

### 追加公開（今回のスコープ）

| Endpoint | 公開 | 抽象メソッド |
| --- | --- | --- |
| /v1/gethealth | ○ | `BitflyerMarketApi.GetHealthAsync` |
| /v1/getboardstate | ○ | `BitflyerMarketApi.GetBoardStateAsync` |
| /v1/me/gettradingcommission | ○ | `BitflyerAccountApi.GetTradingCommissionAsync` |

---

## 注記

* 抽象層で未露出の API は、意図的に Raw に限定されているものを含みます。
* 新規公開の判断は、ユースケースと互換性を優先して行います。
