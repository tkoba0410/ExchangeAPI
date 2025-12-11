# A040-STG4-ARCB Stage4 API マッピング（REST+WS 抽象の枠組み）

Stage4 では「抽象 API の入口」を整理し、具体的な取引所マッピングは Stage5 以降に送る。ここでは 6 区分ごとに、代表的なマッピング例（bitFlyer 参考）と抽象範囲を示す。

## 1. REST: Market Data
- `IMarketDataApi.GetTicker` → 例: `/v1/getticker`
- `IMarketDataApi.GetOrderBook` → 例: `/v1/getboard`
- `IMarketDataApi.GetMarketExecutions` → 例: `/v1/getexecutions`（市場約定）
抽象はスナップショット取得に限定し、履歴/特殊チャネルは Raw に逃がす。

## 2. REST: Trading
- `ITradingApi.SendOrder` → 例: `/v1/me/sendchildorder`（MARKET/LIMIT/STOP/STOP_LIMIT を想定）
- `ITradingApi.CancelOrder` → 例: `/v1/me/cancelchildorder`
- `ITradingApi.GetOpenOrders` → 例: `/v1/me/getchildorders?state=ACTIVE`
親注文/複合注文などは抽象外（Raw）。

## 3. REST: Account（Spot）
- `IAccountApi.GetBalances` → 例: `/v1/me/getbalance`
- `IAccountApi.GetAccountExecutions` → 例: `/v1/me/getexecutions`（口座約定）
入出金や残高履歴は Stage4 抽象外。

## 4. REST: Margin（Account の拡張能力）
- `IMarginAccountApi.GetOpenPositions` → 例: `/v1/me/getpositions`
- `IMarginAccountApi.GetCollateral` → 例: `/v1/me/getcollateral`
Margin の抽象は「建玉・証拠金サマリ」に限定し、詳細な口座/履歴系は Raw とする。

## 5. WebSocket: Realtime Market Data
- `IRealtimeMarketDataApi.SubscribeTicker` → 例: WS `ticker`
- `IRealtimeMarketDataApi.SubscribeOrderBook` → 例: WS `board`
- `IRealtimeMarketDataApi.SubscribeExecutions` → 例: WS `executions`
再接続や QoS は Stage5+ の実装・運用で扱う。

## 6. ExchangeInfo
- `IExchangeInfoApi` は市場/機能の有無を返す入口のみ配置（詳細仕様は Stage5+ で拡張）。

## 7. DTO/Domain の指針
- Domain は価格/サイズ/日時/ID/サイドなど共通概念に寄せ、取引所固有項目は Adapter 側に閉じ込める。
- OrderRequest は Stage3 の骨格を踏襲しつつ、Trading 抽象と整合するパラメータ（price/trigger/time_in_force 等）を保持する。
- 抽象化が難しい項目は DTO/Raw API で扱い、抽象を肥大化させない。
