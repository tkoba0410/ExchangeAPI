# Stage6 Summary（REST-only 信頼性・運用強化）

## 概要
- REST-only 方針を維持しつつ、Timeout/Retry/RateLimit（トークンバケット＋バースト）/CircuitBreaker をポリシー層で実装し、Factory でデフォルト適用。
- エラー分類を E2/E3 レベルに拡張し、`IExchangeErrorClassifier` + bitFlyer マッピングで Retry/CB がカテゴリベース判定。
- 観測性フックを追加（`IRestCallObserver`、OTel ブリッジ、構造化ログ）、`WithObservability(...)` で適用可能。
- DX/テストシーム: `BitflyerClientOptions` 一本化 + TestFactory/ApiBundle/InternalsVisibleTo でモック注入を分離。

## デフォルト値（2024-計測叩き台）
- Timeout: 8s
- Retry: GET 最大3回（base 200ms / max 2s）、POST は一時障害のみ1回
- RateLimit: 5 req/s, burst 2
- CircuitBreaker: 20s窓で失敗率>50%でOpen、閾値3、Open後5sでHalf-Open
※ 本番計測で見直し予定（遅延/429/500 モックでp50/p95/p99取得）

## 観測性（推奨セット）
- ActivitySource=`ExchangeApi.RestClient`、Meter=`exchangeapi`
- メトリクス: `exchangeapi_requests_total{endpoint,method,status,product_code,error}`、`exchangeapi_request_duration_seconds{endpoint,method,status,product_code,error}`
- ログ: 構造化JSONで `timestamp,event_type,method,uri/status_code,duration_ms?,product_code?,error?`（機密除外）
- 適用例: `new BitflyerClientOptions().WithObservability(new RestCallOpenTelemetryObserver(), new StructuredRestClientLogger(Console.WriteLine))`

## テスト状況
- 単体: ポリシー（Retry/Timeout/RL/CB）、観測性通知
- Fault Injection: 429/一時断/タイムアウト/CB開放を検証
- 劣化環境E2E: TestFactory+モックTransportで代表フロー簡略版（残高→注文→約定確認→履歴）を通過
- 計測: 本番想定の実測調整は未実施（方針のみ記載）

## 残タスク/リスク
- 本番計測によるデフォルト値の最終確定（計測結果をSPEC/TODOに反映）
- 劣化環境E2Eを正式なStage5フロー（決済/履歴詳細含む）まで拡充するか検討
- TestFactory/ApiBundleの誤用防止: 公開APIとの使い分けを開発者ガイドで徹底、必要ならCIで参照チェック
