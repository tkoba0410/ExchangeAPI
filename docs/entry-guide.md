# Entry Guide

利用者向けの導入ガイドです。Stage3 までで実装済みの機能（bitFlyer向け Ticker / Balance / MARKET 発注）を対象とし、Stage4 で抽象インターフェースを拡張中です。positions / executions / collateral / cancel 系は Stage4 でインターフェースのみ追加され、実装は Stage5 以降に続きます。

## 1. 対応範囲（Stage3）
- 取引所: bitFlyer
- Public: Ticker
- Private: 残高取得、MARKET 発注
- WebSocket: 未実装（将来 Stage6 で検討）
- Stage4 追加予定（インターフェースのみ）: ポジション/約定/証拠金/オープン注文取得、キャンセル、LIMIT/STOP 系の注文属性

## 2. 抽象インターフェース（主要）
- `IExchangeClient`: Market + Account + Trading をまとめた入口
- Stage3 で利用できるメソッド: `GetTickerAsync`, `GetBalancesAsync`, `SendOrderAsync`（MARKET）
- Stage4 で追加されるメソッド（実装は Stage5 以降）: positions/executions/collateral/open-orders/cancel、注文オプション（`TimeInForce`, `MinuteToExpire`, `TriggerPrice` など）
- DTO: Stage3 実装分は `Ticker`, `Balance`, `OrderRequest/Result`。Stage4 で `Position`, `Execution`, `Collateral`, `OpenOrder` 等を追加予定。

## 3. セットアップ
1) .NET 10+ 環境でリポジトリを取得・ビルド  
2) DI 登録（Transport → RestClient → bitFlyer API → IExchangeClient）  
3) Private API 利用時は API キー/シークレットを設定  
   - 署名は BitflyerPrivateApi 内で RestClient に委譲（RestClient 側の設定参照）

## 4. 典型的な呼び出し（Stage3 時点）
- Ticker: `GetTickerAsync("BTC/JPY")`
- 残高: `GetBalancesAsync()`
- 発注: `PlaceOrderAsync(new OrderRequest(...))`  
  - Stage3 では MARKET のみ。LIMIT/STOP/キャンセル等は Stage5 以降に追加。

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
- 親注文（sendparentorder）の抽象化
- WebSocket (ticker/board/executions) の追加
- 複数取引所対応時の DTO マッピング拡充
