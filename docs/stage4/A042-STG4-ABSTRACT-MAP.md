# A042-STG4-ABSTRACT-MAP 抽象 API 対応表（Stage4）

Stage4 で確定する抽象インターフェースと代表 DTO を 6 区分で整理する。REST/WS を明確に分離し、Margin は拡張インターフェースで表現する。

| 区分 | 抽象インターフェース / メソッド | プロトコル | スコープ | 備考 |
| --- | --- | --- | --- | --- |
| Market | IMarketDataApi.GetTicker | REST | Public | Ticker スナップショット |
| Market | IMarketDataApi.GetOrderBook | REST | Public | OrderBook スナップショット |
| Market | IMarketDataApi.GetExecutions | REST | Public | 約定履歴（歩み値） |
| Trading | ITradingApi.SendOrder | REST | Private | OrderRequest → OrderResult |
| Trading | ITradingApi.CancelOrder | REST | Private | OrderId 指定キャンセル |
| Trading | ITradingApi.GetOpenOrders | REST | Private | OpenOrder 一覧取得 |
| Account | IAccountApi.GetBalances | REST | Private | 現物残高 |
| Margin | IMarginAccountApi.GetOpenPositions | REST | Private | 建玉一覧（Margin 拡張） |
| Margin | IMarginAccountApi.GetCollateral | REST | Private | 証拠金サマリ（Margin 拡張） |
| Realtime | IRealtimeMarketDataApi.SubscribeTicker | WS | Public | Ticker ストリーム購読 |
| Realtime | IRealtimeMarketDataApi.SubscribeOrderBook | WS | Public | OrderBook ストリーム購読 |
| Realtime | IRealtimeMarketDataApi.SubscribeExecutions | WS | Public | 約定ストリーム購読 |
| ExchangeInfo | IExchangeInfoApi | REST | Public | 市場/機能情報の入口（スケルトン: 市場一覧/機能フラグなど） |

備考:
- IMarginAccountApi は IAccountApi を継承し、Margin 能力を追加する。
- 親注文/入出金/履歴系など抽象化しない機能は Raw API として扱う。
- WS の購読は `IAsyncEnumerable<T>`（キャンセルは CancellationToken）または `IDisposable` を返す形を想定し、解除手段を必須とする。
- WS イベントの最小 DTO 例: `TickerTick { Bid, Ask, Last, Timestamp }`, `OrderBookDelta { Bids[], Asks[], Snapshot, Timestamp }`, `ExecutionTick { Side, Price, Size, Timestamp }`。
