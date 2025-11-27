# Entry Guide

利用者向けの導入ガイドです。Stage4 時点の機能（bitFlyer向け Ticker/発注/キャンセル/一覧系）を対象としています。

## 1. 対応範囲（Stage4）
- 取引所: bitFlyer
- Public: Ticker, Board
- Private: 残高、証拠金、ポジション、約定、オープン注文、発注（MARKET/LIMIT/STOP/STOP_LIMIT）、キャンセル/全キャンセル
- WebSocket: 未実装（将来 Stage6 で検討）

## 2. 抽象インターフェース（主要）
- `IExchangeClient`: Market + Account + Trading をまとめた入口
- `IExchangeTradingClient`: 発注/キャンセル/ポジション/約定/オープン注文
- `IExchangeAccountClient`: 残高/証拠金
- DTO: `Ticker`, `Board`, `Balance`, `Collateral`, `Position`, `Execution`, `OpenOrder`, `OrderRequest/Result`

## 3. セットアップ
1) .NET 10+ 環境でリポジトリを取得・ビルド  
2) DI 登録（Transport → RestClient → bitFlyer API → IExchangeClient）  
3) Private API 利用時は API キー/シークレットを設定  
   - 署名は BitflyerPrivateApi 内で RestClient に委譲（RestClient 側の設定参照）

## 4. 典型的な呼び出し
- Ticker: `GetTickerAsync("BTC/JPY")`
- Board: `GetBoardAsync("BTC/JPY")`
- 残高: `GetBalancesAsync()`
- 証拠金: `GetCollateralAsync()`
- 発注: `PlaceOrderAsync(new OrderRequest(...))`
  - MARKET / LIMIT / STOP（成行） / STOP_LIMIT（指値付き）をサポート
  - `TimeInForce`, `MinuteToExpire` を指定可能
- キャンセル: `CancelOrderAsync(productCode, orderId)` / `CancelAllOrdersAsync(productCode)`
- オープン注文一覧: `ListOpenOrdersAsync(productCode)`
- ポジション/約定一覧: `ListPositionsAsync(productCode)`, `ListExecutionsAsync(productCode)`

## 5. エラーと例外
- 未サポートシンボル: `SymbolNotSupportedException`
- HTTP/取引所エラー: `ExchangeApiException`（`StatusCode`, `ExchangeErrorCode`, `ErrorCategory` を参照）
- STOP 系のパラメータ不足や不正値は `ArgumentException`

## 6. 利用上の注意
- Ticker の bid/ask は価格のみ（サイズが必要なら Board を使用）。LTP は直近約定価格であり、その時刻を表すものではない。
- STOP_LIMIT は `Price` と `TriggerPrice` の両方が必要。`Price` を省略すると STOP（成行）となる。
- product_code は bitFlyer 仕様に合わせる（例: `BTC_JPY`, `FX_BTC_JPY`）。抽象シンボルは `BTC/JPY`。

## 7. 参考ドキュメント
- 抽象 API 対応表: `docs/stage4/A042-STG4-ABSTRACT-MAP.md`
- DTO マッピング（Ticker 例）: `docs/stage4/DTO-Ticker-MAP.md`
- 動作確認メモ: `docs/stage4/A070-STG4-OPS.md`
- Stage 概要: `docs/STAGES-OVERVIEW.md`

## 8. 次ステップ（今後の拡張）
- 親注文（sendparentorder）の抽象化
- WebSocket (ticker/board/executions) の追加
- 複数取引所対応時の DTO マッピング拡充
