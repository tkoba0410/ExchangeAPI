# Entry Guide

利用者向けの導入ガイドです。Stage3 までで実装済みの機能（bitFlyer向け Ticker / Balance / MARKET 発注）を対象とし、Stage4 では「REST+WS 抽象 API」を 6 区分（Market/Trading/Account/Margin/Realtime/ExchangeInfo）で定義しています。Stage4 で追加される要素は抽象のみで、実装は Stage5 以降に続きます。

## 1. 対応範囲（Stage3）
- 取引所: bitFlyer
- Public: Ticker
- Private: 残高取得、MARKET 発注
- WebSocket: 未実装（将来 Stage6 で検討）
- Stage4 追加（抽象のみ）: Market (Ticker/Board/Executions), Trading (Send/Cancel/OpenOrders), Account (Balances), Margin (Positions/Collateral), Realtime (WS サブスク), ExchangeInfo 入口

## 2. 抽象インターフェース（主要）
- Stage3 で利用できるメソッド: `GetTickerAsync`, `GetBalancesAsync`, `SendOrderAsync`（MARKET）
- Stage4 で追加される抽象（実装は Stage5 以降）: `IMarketDataApi` / `ITradingApi` / `IAccountApi` / `IMarginAccountApi` / `IRealtimeMarketDataApi` / `IExchangeInfoApi`
- DTO: Stage3 実装分は `Ticker`, `Balance`, `OrderRequest/Result`。Stage4 で `Position`, `Execution`, `Collateral`, `OpenOrder` 等の抽象型を追加予定。

## 3. セットアップ
1) .NET 10+ 環境でリポジトリを取得・ビルド  
2) DI 登録（Transport → RestClient → bitFlyer API → BitflyerExchangeClient → 各抽象IF にマップ）  
3) Private API 利用時は API キー/シークレットを設定  
   - 署名は BitflyerPrivateApi 内で RestClient に委譲（RestClient 側の設定参照）

## 4. 典型的な呼び出し（Stage3 時点）
- Ticker: `GetTickerAsync("BTC/JPY")`
- 残高: `GetBalancesAsync()`
- 発注: `PlaceOrderAsync(new OrderRequest(...))`  
  - Stage3 では MARKET のみ。LIMIT/STOP/キャンセル等は Stage5 以降に実装予定。

## 5. エラーと例外
- 未サポートシンボル: `SymbolNotSupportedException`
- HTTP/取引所エラー: `ExchangeApiException`（`StatusCode`, `ExchangeErrorCode`, `ErrorCategory` を参照）
- STOP 系のパラメータ不足や不正値は `ArgumentException`（Stage5 以降で適用予定）

## 6. 利用上の注意
- Ticker の bid/ask は価格のみ（サイズが必要なら Board を使用予定）。LTP は直近約定価格であり、その時刻を表すものではない。
- STOP/STOP_LIMIT のパラメータ要件（`Price` と `TriggerPrice` 等）は Stage5 以降で適用される。
- product_code は bitFlyer 仕様に合わせる（例: `BTC_JPY`, `FX_BTC_JPY`）。抽象シンボルは `BTC/JPY`。

## 7. 参考ドキュメント
- 抽象 API 対応表: `docs/stage4/A042-STG4-ABSTRACT-MAP.md`
- DTO マッピング（Ticker 例）: `docs/stage4/DTO-Ticker-MAP.md`
- 動作確認メモ: `docs/stage4/A070-STG4-OPS.md`
- Stage 概要: `docs/STAGES-OVERVIEW.md`

## 8. 次ステップ（今後の拡張）
- Stage4: REST+WS 抽象 API を 6 区分で確定（本書と A0xx 参照）
- Stage5: bitFlyer 実装（REST/WS）を追加し、LIMIT/STOP/キャンセル/ポジション取得などを動作させる
- Stage6 以降: 信頼性・運用強化、複数取引所対応、ドキュメント整備
