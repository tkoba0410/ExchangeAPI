# Entry Guide

利用者向けの導入ガイドです。Raw-first レイアウト（Common.Core + Exchange.* + Exchange.Factory）に移行済み。Unified 的な束ねは Factory 側のヘルパで扱う想定です。REST-only 方針を維持しつつ、信頼性パターン（Timeout/Retry/RateLimit/CircuitBreaker）と観測性フックを提供しています（Realtime/WS は廃止、エラー分類はカテゴリ単位）。

## 1. 対応範囲（現行）
- 取引所: bitFlyer
- Market: Ticker / Board / MarketExecutions（歩み値, Candles は未サポート, Public）
- Trading: MARKET / LIMIT（STOP は取引所依存）、キャンセル（単体）
- Account/Margin: 残高・建玉・証拠金・AccountExecutions（自口座の約定履歴）
- ExchangeInfo: BTC/JPY の最小数量・価格刻みなど
- WebSocket: 非対応（REST only、正式廃止）
- 信頼性/運用: Timeout/Retry/RateLimit/CircuitBreaker デフォルト、観測性フック（OTelブリッジ/構造化ログ）

## 2. 抽象インターフェース（主要）
- `IMarketDataApi`（Ticker/Board/MarketExecutions）、`IAccountApi`（Balances/AccountExecutions）、`ITradingApi`（Place/Cancel/OpenOrders/GetOrder）、`IMarginAccountApi`、`IExchangeInfoApi`
- DTO: `Ticker`, `Board/OrderBook`, `MarketExecution`, `AccountExecution`, `OrderRequest/Result/Status`, `OpenOrder`, `Balance`, `Position`, `Collateral`, `ExchangeInfo`

## 3. セットアップ（簡易）
1) .NET 10+ 環境でリポジトリを取得・ビルド  
2) Raw-first の最小例:  
   - `Common.Core` と `Exchange.Bitflyer` を参照し、`BitflyerClientFactory.Create(apiKey, apiSecret)` で `BitflyerExchangeClient`（Facade）を生成  
   - HTTP/署名/Raw/Adapters/Apis/Facade は Factory が組み立て  
3) 複数取引所を束ねる場合（任意）:  
   - `UnifiedClient` に Bitflyer/Bittrade のクライアントを渡し、`PrimaryExchange` でデフォルト取引所を切替可能  
4) Private API 利用時は API キー/シークレットを設定（署名は RestClient/Signer に委譲）

## 4. 典型的な呼び出し（Stage6）
- Ticker: `GetTickerAsync(new Symbol("BTC/JPY"))`
- 市場約定（歩み値, Public）: `GetMarketExecutionsAsync(new Symbol("BTC/JPY"))`
- 口座約定（Private）: `GetAccountExecutionsAsync(new Symbol("BTC/JPY"))`
- 残高: `GetBalancesAsync()`
- 発注: `PlaceMarketOrderAsync` / `PlaceLimitOrderAsync`（STOP は取引所依存）
- キャンセル: `CancelOrderAsync(Symbol, OrderKey)`（全件キャンセルは Raw API でのみ提供）
- 照会: `GetOrderAsync(Symbol, OrderKey)`
- ポーリング: `OrderPolling.WaitForOrderAsync`（1s/最大30回がデフォルト）

## 5. 注文識別子（OrderKey）
- `OrderKey = (OrderIdKind, Value)` の組です。
- **接続保証**: `OrderResult.Key` / `OpenOrder.Key` は、そのまま `GetOrderAsync` / `CancelOrderAsync` / `OrderPolling` に渡せます。

### bitFlyer の注意点
- `AcceptanceId` と `ExchangeOrderId` は別物です。
- 一覧 API から `AcceptanceId` が取れない場合は `OpenOrder.Key` が `ExchangeOrderId` になります。
- `OrderIdKind.ExchangeOrderId` を受け付けない API では `ExchangeFeatureNotSupportedException` を投げます。

## 6. ポーリングと not found
- not found は `ExchangeOrderNotFoundException` として扱います。
- `NotFoundPolicy.Continue` は再試行継続、`NotFoundPolicy.StopAsNotFound` は例外をそのまま返します（デフォルトは Continue）。

## 7. エラーと例外
- 未サポートシンボル: `SymbolNotSupportedException`
- 未対応機能: `ExchangeFeatureNotSupportedException`
- 注文が見つからない: `ExchangeOrderNotFoundException`
- HTTP/取引所エラー: `ExchangeApiException`（`StatusCode`, `ExchangeErrorCode`, `ErrorCategory` を参照）※カテゴリ粒度の分類
- STOP 系のパラメータ不足や不正値は `ArgumentException`

## 8. 利用上の注意
- Candles は REST 未サポート（`ExchangeFeatureNotSupportedException` を返す）。
- product_code は bitFlyer 仕様に合わせる（例: `BTC_JPY`, `FX_BTC_JPY`）。抽象シンボルは `BTC/JPY`。
- `Symbol` は値オブジェクト。新銘柄は `new Symbol("XYZ/JPY")` のように文字列で表現できる。
- exchangeId は `ExchangeCode` に統一。文字列入力が必要な場合は `ExchangeCodeParser.Parse(string)` を入口で使う。

## 9. 参考ドキュメント
- 抽象 API 対応表: `docs/stage4/A042-STG4-ABSTRACT-MAP.md`（Stage4時点、名称は旧構成）  
- Stage5 構成: `docs/stage5/STRUCTURE-OPTIMAL.md`（旧命名ベースだが概念は踏襲）  
- 動作確認メモ: `docs/stage5/TESTS.md`  
- Contracts 詳細: `docs/Contracts/`（注文 DTO, ExchangeInfo, 認証の補足。現在は Common.Core 配下に移動）  
- Stage 概要: `docs/STAGES-OVERVIEW.md`  
- Stage7 移行ロードマップ: `docs/stage7/A020-STG7-RAW-FIRST-ROADMAP.md`（新レイアウトの詳細）

## 10. 次ステップ（今後の拡張）
- Raw-first での取引所追加、必要に応じた Factory 経由の統合クライアント組み立てヘルパ
- ドキュメントの旧命名から新命名への置き換え継続（Stage1〜6資料は旧名のままの箇所あり）
- WS/Realtime の再検討は別モジュールで扱う予定（現時点では REST-only 維持）
