# Entry Guide

利用者向けの導入ガイドです。Stage6 では REST-only 方針を維持しつつ、信頼性パターン（Timeout/Retry/RateLimit/CircuitBreaker）と観測性フックを提供しています（Realtime/WS は廃止、エラー分類はカテゴリ単位）。

## 1. 対応範囲（Stage6）
- 取引所: bitFlyer
- Market: Ticker / Board / MarketExecutions（歩み値, Candles は未サポート, Public）
- Trading: MARKET / LIMIT / STOP、キャンセル（単体）、ポーリング
- Account/Margin: 残高・建玉・証拠金・AccountExecutions（自口座の約定履歴）
- ExchangeInfo: BTC/JPY の最小数量・価格刻みなど
- WebSocket: 非対応（REST only、正式廃止）
- 信頼性/運用: Timeout/Retry/RateLimit/CircuitBreaker デフォルト、観測性フック（OTelブリッジ/構造化ログ）

## 2. 抽象インターフェース（主要）
- Stage6 で利用できるメソッド: `IMarketDataApi`（Ticker/Board/MarketExecutions）、`IAccountApi`（Balances/AccountExecutions）、`ITradingApi`（Send/Cancel/OpenOrders/Poll）、`IMarginAccountApi`、`IExchangeInfoApi`
- DTO: `Ticker`, `Board/OrderBook`, `MarketExecution`, `AccountExecution`, `OrderRequest/Result/Status`, `OpenOrder`, `Balance`, `Position`, `Collateral`, `ExchangeInfo`

## 3. セットアップ（簡易）
1) .NET 10+ 環境でリポジトリを取得・ビルド  
2) `BitflyerClientFactory.Create(apiKey, apiSecret)` で `BitflyerExchangeClient`（Facade）を生成  
   - HTTP/署名/Raw/Adapters/Apis/Facade は Factory が組み立て  
3) Private API 利用時は API キー/シークレットを設定（署名は RestClient/Signer に委譲）

## 4. 典型的な呼び出し（Stage6）
- Ticker: `GetTickerAsync("BTC/JPY")`
- 市場約定（歩み値, Public）: `GetMarketExecutionsAsync("BTC/JPY")`
- 口座約定（Private）: `GetAccountExecutionsAsync("BTC_JPY")`
- 残高: `GetBalancesAsync()`
- 発注: `SendOrderAsync(new OrderRequest(...))`（MARKET/LIMIT/STOP に対応）
- キャンセル: `CancelOrderAsync`（全件キャンセルは Raw API でのみ提供）
- ポーリング: `PollOrderStatusAsync`（1s/最大30回がデフォルト）

## 5. エラーと例外
- 未サポートシンボル: `SymbolNotSupportedException`
- HTTP/取引所エラー: `ExchangeApiException`（`StatusCode`, `ExchangeErrorCode`, `ErrorCategory` を参照）※カテゴリ粒度の分類
- STOP 系のパラメータ不足や不正値は `ArgumentException`

## 6. 利用上の注意
- Candles は REST 未サポート（NotSupported を返す）。
- product_code は bitFlyer 仕様に合わせる（例: `BTC_JPY`, `FX_BTC_JPY`）。抽象シンボルは `BTC/JPY`。

## 7. 参考ドキュメント
- 抽象 API 対応表: `docs/stage4/A042-STG4-ABSTRACT-MAP.md`（Stage4時点）
- Stage5 構成: `docs/stage5/STRUCTURE-OPTIMAL.md`
- 動作確認メモ: `docs/stage5/TESTS.md`
- Contracts 詳細: `docs/Contracts/`（注文 DTO, ExchangeInfo, 認証の補足）
- Stage 概要: `docs/STAGES-OVERVIEW.md`

## 8. 次ステップ（今後の拡張）
- Stage6: REST-only のまま信頼性・運用周りを継続強化
- Stage7 以降: 複数取引所対応の検証、ドキュメント拡充（WS は別モジュール検討時まで対象外）
