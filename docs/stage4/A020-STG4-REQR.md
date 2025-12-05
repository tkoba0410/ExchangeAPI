# A020-STG4-REQR Stage4 要件定義（REST+WS 抽象確定）

## 1. 対象
- ExchangeAPI の抽象層を「REST+WS 抽象 API の正式確定」として整理するステージ
- 6 区分: Market / Trading / Account / Margin / Realtime / ExchangeInfo
- 取引所固有の実装・API 詳細は Stage5 以降に送る

## 2. ユースケース（代表）
1) Market: Ticker/OrderBook/Executions/Candlesticks をスナップショットで取得する  
2) Trading: 抽象 OrderRequest を送信し、OpenOrders を照会・Cancel する  
3) Account: 現物残高を取得し、取引可否を判断する  
4) Margin: 建玉（OpenPositions）と証拠金サマリ（Collateral）を取得する  
5) Realtime: WS で Ticker/OrderBook/Executions を購読し、更新を受け取る  
6) ExchangeInfo: 対象市場/機能の存在を確認する入口（スケルトン）

## 3. 機能要件
- 抽象インターフェース（REST）
  - `IMarketDataApi`: GetTicker / GetOrderBook / GetExecutions / ListCandlesticks
  - `ITradingApi`: SendOrder / CancelOrder / GetOpenOrders
  - `IAccountApi`: GetBalances
  - `IMarginAccountApi : IAccountApi`: GetOpenPositions / GetCollateral（Margin はここまでに限定）
- 抽象インターフェース（WS）
  - `IRealtimeMarketDataApi`: SubscribeTicker / SubscribeOrderBook / SubscribeExecutions
  - 購読の解除手段を必須とし、`IAsyncEnumerable<T>` + CancellationToken または `IDisposable` を返す形を想定
- ExchangeInfo
  - `IExchangeInfoApi`: 将来拡張用のエントリーポイント（スケルトンのみ、例: 対応市場一覧/機能フラグ/概略レートリミット）
- Candlestick 対応方針
  - 抽象では ListCandlesticks を正式サポートするが、取引所非対応（例: bitFlyer）は `NotImplemented` 相当で明示的に拒否する。
  - 初期の実装ターゲットは bittrade を想定する。
- ドメイン型
  - Ticker / OrderBook / Execution / OrderRequest / OrderResult / OpenOrder / Position / Collateral
  - OrderRequest は Stage3 の骨格を踏襲し、抽象 IF との整合性を確認する
- Raw API 方針
  - 抽象化できない機能（親注文・入出金など）は Raw として切り出し、Stage4 では触れないことを明記

## 4. 非機能要件
- REST と WS の責務を分離し、薄い API として境界を明確化する
- Stage3 までの公開 API との後方互換性を維持する
- 抽象の肥大化を避け、Margin は最小能力に限定する
- 将来のレート制御/リトライ/エラー分類はフック方針のみ示し、実装は Stage5 以降に任せる

## 5. 除外（Stage4 ではやらない）
- bitFlyer など特定取引所の HTTP/WS 実装・マッピング
- エラーコード分類の詳細実装、レートリミットや再接続ロジックの実装
- 親注文/入出金/履歴系など抽象化しない API の対応
- WebSocket の再接続や QoS 制御など運用ロジック
- ドキュメント生成パイプラインの整備（骨子のみ）
