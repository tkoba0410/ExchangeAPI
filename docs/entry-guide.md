# Entry Guide

利用者向けの導入ガイドです。Stage5 時点での対応範囲は bitFlyer / REST のみで、Market/Trading/Account/Margin/ExchangeInfo の抽象 API を実装済み（Realtime は未実装）。

## 1. 対応範囲（Stage5）
- 取引所: bitFlyer
- Market: Ticker / Board / Executions（Candles は未サポート）
- Trading: MARKET / LIMIT / STOP、キャンセル（単体・全件）、ポーリング
- Account/Margin: 残高・建玉・証拠金・約定取得
- ExchangeInfo: BTC/JPY の最小数量・価格刻みなど
- WebSocket: 未実装（REST only）

## 2. 抽象インターフェース（主要）
- Stage5 で利用できるメソッド: `IMarketDataApi`（Ticker/Board/Executions）、`ITradingApi`（Send/Cancel/OpenOrders/Poll）、`IAccountApi`、`IMarginAccountApi`、`IExchangeInfoApi`
- DTO: `Ticker`, `Board/OrderBook`, `Execution`, `OrderRequest/Result/Status`, `OpenOrder`, `Balance`, `Position`, `Collateral`, `ExchangeInfo`

## 3. セットアップ（簡易）
1) .NET 10+ 環境でリポジトリを取得・ビルド  
2) `BitflyerClientFactory.Create(apiKey, apiSecret)` で `BitflyerExchangeClient`（Facade）を生成  
   - HTTP/署名/Raw/Adapters/Apis/Facade は Factory が組み立て  
3) Private API 利用時は API キー/シークレットを設定（署名は RestClient/Signer に委譲）

## 4. 典型的な呼び出し（Stage5）
- Ticker: `GetTickerAsync("BTC/JPY")`
- 残高: `GetBalancesAsync()`
- 発注: `SendOrderAsync(new OrderRequest(...))`（MARKET/LIMIT/STOP に対応）
- キャンセル: `CancelOrderAsync`, `CancelAllOrdersAsync`
- ポーリング: `PollOrderStatusAsync`（1s/最大30回がデフォルト）

## 5. エラーと例外
- 未サポートシンボル: `SymbolNotSupportedException`
- HTTP/取引所エラー: `ExchangeApiException`（`StatusCode`, `ExchangeErrorCode`, `ErrorCategory` を参照）
- STOP 系のパラメータ不足や不正値は `ArgumentException`

## 6. 利用上の注意
- Candles は REST 未サポート（NotSupported を返す）。
- product_code は bitFlyer 仕様に合わせる（例: `BTC_JPY`, `FX_BTC_JPY`）。抽象シンボルは `BTC/JPY`。

## 7. 参考ドキュメント
- 抽象 API 対応表: `docs/stage4/A042-STG4-ABSTRACT-MAP.md`（Stage4時点）
- Stage5 構成: `docs/stage5/STRUCTURE-OPTIMAL.md`
- 動作確認メモ: `docs/stage5/TESTS.md`
- Stage 概要: `docs/STAGES-OVERVIEW.md`

## 8. 次ステップ（今後の拡張）
- Stage6: WS（Realtime）対応検討
- Stage7 以降: 信頼性・運用強化、複数取引所対応、ドキュメント整備
