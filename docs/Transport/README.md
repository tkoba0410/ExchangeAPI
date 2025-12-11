# Transport レイヤー概要

HTTP 基盤レイヤーの構成要素と役割をまとめます。RestClient にポリシー/署名/観測/エラー分類を差し込む設計。

## 主な構成
- Protocol: `IRestClient`/`RestClient`（JSON HTTP 呼び出し）, `IRequestSigner`, `IExchangeErrorClassifier`, `IErrorPayloadParser`（デフォルトは error_code/message 抽出）
- Transport: `IHttpTransport`/`HttpTransport`（HttpClient ベース、ハンドラ注入も可）
- Policy: `IHttpPolicy`/`HttpPolicyPipeline`、`Timeout`/`Retry`/`RateLimit`/`CircuitBreaker`、`HttpPolicyFactory`（デフォルト構成）、`IPolicyObserver`（リトライ/レートリミット/遮断イベント）
- Logging/Observability: `IRestClientLogger`（構造化/NoOp）、`IRestCallObserver`（OTel/メトリクス/NoOp）
- Time: `IExchangeClock`/`SystemClock`
- Mapping: `InMemoryOrderIdMapper`（簡易 ID マッピング）

## エラーとエラーパース
- `ExchangeApiException` に統一し、HTTP ステータス異常/JSON パース失敗/HttpRequestException/タイムアウトをラップ。`ExchangeErrorCategory` で分類。
- `IErrorPayloadParser` でエラーボディをパース（デフォルトは JSON の error_code/message/code を抽出し、非JSONは本文をそのままメッセージに）。

## ポリシー
- `HttpPolicyPipeline` で複数ポリシーを順適用。
- `RetryHttpPolicy`: GET/その他で試行回数を分け、RateLimit/5xx/ネットワーク/タイムアウトを指数バックオフでリトライ。`IPolicyObserver.OnRetry` で観測。
- `RateLimitHttpPolicy`: トークンバケットで最小間隔を保証し、遅延を `OnRateLimitDelay` で通知。
- `CircuitBreakerHttpPolicy`: 連続失敗で Open/HalfOpen を制御し、遮断/開放を `OnCircuitOpened`/`OnCircuitRejected` で通知。
- `TimeoutHttpPolicy`: リクエスト全体のタイムアウト。
- `HttpPolicyFactory`: デフォルト構成を組み立て、Observer を渡せる。

## Transport
- `HttpTransport` は HttpClient または HttpMessageHandler を注入可能。Dispose の責務をコンストラクタで選択。

## ペンディングタスク
- OrderIdMapper の永続化/上限制御サンプル追加、ドキュメント化。
- JSON 以外の POST/PUT（フォーム/バイナリ）対応のための RestClient 拡張オーバーロード検討。
- ポリシー/HttpClient の詳細なチューニングガイド（SocketsHttpHandler 設定例、プロキシ/KeepAlive）。
