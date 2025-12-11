# Stage6 Summary（REST-only 信頼性・運用強化）

## 信頼性パターン
- 実装: Timeout/Retry/RateLimit（トークンバケット＋バースト）/CircuitBreaker をポリシー層で実装し、Factory でデフォルト適用。Retry/RateLimit/CB に Observer フックを追加し、リトライ・待機・遮断イベントを観測可能に。
- 目的: REST-onlyでもネットワーク揺らぎや429/5xxを吸収し、フェイルファストを一貫したルールで行う。
- 効果: 最小設定で安全デフォルトが効き、劣化環境下でも呼び出しが安定。Retry/CB/RLがカテゴリベースで動くため運用判断がシンプル。

## エラー分類
- 実装: `IExchangeErrorClassifier` + bitFlyerマッピングで `ExchangeErrorCategory` を付与し、Retry/CB がカテゴリベース判定。エラーペイロードパーサ追加で error_code/message/code 以外の非JSONテキストもメッセージとして扱う。
- 目的: 再試行可否やCB判定を明確にし、取引所固有エラーをドメイン例外に揃える。
- 効果: 認証/レートリミット/一時障害/業務エラーの扱いが統一され、運用判断と再試行ポリシーがぶれない。

## 観測性
- 実装: `IRestCallObserver`、OTelブリッジ（`exchangeapi_requests_total`, `exchangeapi_request_duration_seconds` with endpoint/method/status/product_code/error）、構造化JSONロガーを追加。`WithObservability(...)` で適用例を用意。
- 目的: メトリクス/ログ/トレースの命名を標準化し、OTel/外部監視との接続を容易にする。
- 効果: 監視導入が即時に可能になり、レイテンシ/エラー率/CB状態を一貫したタグで可視化できる。

## DX / テストシーム
- 実装: `BitflyerClientOptions` に設定を束ね、TestFactory/ApiBundle/InternalsVisibleTo でモック注入を分離。本番APIを汚さずテスト経路を確保。
- 目的: 設定と依存注入ポイントを単純化し、劣化環境E2Eや将来の拡張を容易にする。
- 効果: 公開APIのシンプルさを維持したまま、テスト専用シームでモック差し替えや劣化シナリオ検証が可能。

## デフォルト値（2024-計測叩き台）
- Timeout: 8s
- Retry: GET 最大3回（base 200ms / max 2s）、POST は一時障害のみ1回
- RateLimit: 5 req/s, burst 2
- CircuitBreaker: 20s窓で失敗率>50%でOpen、閾値3、Open後5sでHalf-Open
- ※ 本番計測で見直し予定（遅延/429/500 モックでp50/p95/p99取得）

## 観測性（推奨セット）
- ActivitySource=`ExchangeApi.RestClient`、Meter=`exchangeapi`
- メトリクス: `exchangeapi_requests_total{endpoint,method,status,product_code,error}`、`exchangeapi_request_duration_seconds{endpoint,method,status,product_code,error}`
- ログ: 構造化JSONで `timestamp,event_type,method,uri/status_code,duration_ms?,product_code?,error?`（機密除外）
- 適用例: `new BitflyerClientOptions().WithObservability(new RestCallOpenTelemetryObserver(), new StructuredRestClientLogger(Console.WriteLine))`

## テスト状況
- 単体: ポリシー（Retry/Timeout/RL/CB）、観測性通知、エラーマッピング境界
- Fault Injection: 429/一時断/タイムアウト/CB開放を検証
- 劣化環境E2E: TestFactory+モックTransportで正式フロー（残高→注文→約定確認→履歴→決済※反対売買でポジション解消→キャンセル→ポジション→証拠金）を通過
- 計測: 本番想定の実測調整は未実施（方針のみ記載）

## 残タスク/リスク
- 本番計測によるデフォルト値の最終確定（計測結果をSPEC/TODOに反映）
- 劣化環境E2Eを正式なStage5フロー（決済/履歴詳細含む）まで拡充するか検討
- TestFactory/ApiBundleの誤用防止: 公開APIとの使い分けを開発者ガイドで徹底、必要ならCIで参照チェック
- ExchangeInfo を JSON で読み込み切り替える運用（設定フラグ/DIサンプル）を決定・実装する
