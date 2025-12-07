# Stage6 仕様メモ（REST-only 信頼性・運用強化）

## 1. 目的とゴール
- Stage5 REST-only 土台に信頼性パターン（Timeout/Retry/RateLimit/CircuitBreaker）と観測性を追加し、運用で扱える安全デフォルトを提供する。
- DX: 最小設定で安全に動作し、必要箇所のみ上書きできるオプション設計を整える。
- テスト: 劣化環境でも代表フローが通ることを Fault Injection と結合テストで確認する。

## 2. 実装フェーズ / 優先度
1) Timeout/Retry デフォルト導入  
2) RateLimiter 実装  
3) CircuitBreaker 実装  
4) 観測性フック（ログ/メトリクス/トレース）  
5) Factory オプション拡張（安全デフォルト + 上級者設定）  
6) Fault Injection / 劣化環境結合テスト  
- 各フェーズでビルド/テストを緑にしてから次へ進む。

## 3. デフォルト値（初期合意の叩き台）
- Timeout: 8s（Public/Private共通の初期値）。
- Retry: GET 最大3回（指数バックオフ、base 200ms / max 2s）、POST はネットワーク一時障害のみ1回。
- RateLimit: 5 req/s、バースト 2（実測で調整可）。
- CircuitBreaker: 20s 窓で失敗率 >50% で Open、5s 後 Half-Open（1リクエスト成功で Close）。初期閾値は 3 回失敗。
- これらは初期値。負荷/レイテンシ計測で見直し、SPECとコードに反映して固定する。

## 4. 失敗分類と再試行可否
- 例外マッピング層で失敗分類タグを付与: Auth/Permission, RateLimit, Transient(NW/Timeout), Business。
- Retry/CircuitBreaker はタグのみを参照して挙動を決定する（例: Business は即失敗、Transient/RateLimit はポリシー許容範囲で再試行）。
- bitFlyer 固有エラー → ドメイン例外 → 分類タグのマッピング表を作成し、テストで検証。

## 5. 観測性
- インターフェース: `IRestCallObserver`（コールバック）に集約し、ログ/メトリクス/トレースを一度で通知。`IRestClientLogger` は RequestId/エンドポイント/所要時間/HTTP ステータス/主要ドメイン属性を記録（秘密情報除外）。
- メトリクス項目: 成功率、p50/p95/p99 レイテンシ、エラー種別カウント、CircuitBreaker 状態（Open/Half-Open/Close）を最低限提供。
- OTel 連携: Meter/Tracer/Logger に流す薄いアダプタをサンプル実装。
- 命名/タグ例: `exchangeapi_request_duration_seconds{endpoint,status,product_code,cb_state}`、`exchangeapi_requests_total{endpoint,status}`、`exchangeapi_cb_state{endpoint}`。ログは JSON で `timestamp, request_id, endpoint, status_code, duration_ms, product_code, cb_state` を最低限に固定し、機密は出力しない。
- 実装状況: `RestCallOpenTelemetryObserver` で Activity/Meter を発行（メトリクス名: `exchangeapi_requests_total`, `exchangeapi_request_duration_seconds`、タグ: endpoint/method/status/product_code/error）。構造化 JSON ログサンプル `StructuredRestClientLogger` を追加。

## 6. 設定と DX
- `BitflyerClientOptions` に Timeouts/Retry/RateLimit/CircuitBreaker/LoggingVerbosity を束ねる。
- 拡張メソッド `WithObservability(...)` で Observer を注入できる導線を提供。
- 最小設定: API キーを入れれば安全デフォルトで動作。詳細設定: 個別ポリシー/メトリクス連携を上書き可能。
- API シグネチャ草案:
  - `BitflyerClientOptions` に `Timeouts`, `RetryPolicy`, `RateLimitPolicy`, `CircuitBreakerPolicy`, `LoggingVerbosity`, `Observer` をプロパティとして持たせる。
  - `WithObservability(IRestCallObserver observer)` を拡張メソッドとして用意し、DI 容器経由でも直接注入でも利用できるようにする。
  - ポリシーは DI で共有インスタンスを注入し、呼び出しごとに参照する形を基本とする（状態を持つ CB はスレッドセーフ実装を前提）。
- 実装状況: `BitflyerClientOptions` と `WithObservability(...)` を追加し、ファクトリでオプション経由の構成に対応。`HttpPolicyOptions` に `RateLimitBurst` を追加し、トークンバケット型 RateLimiter をデフォルトで使用。
- 設計方針アップデート: Options 一本化で注入ポイントを簡素化し、本番は最小構成、詳細はオプションで上書き。Tests アセンブリ限定の TestFactory（InternalsVisibleTo）や API バンドル DTO を用意し、モック注入を本番 API とは分離する計画。
- internal シーム/利用ルール: Tests 専用に `BitflyerTestClientFactory` と `BitflyerApiBundle` を用意し、InternalsVisibleTo でのみ利用。公開 API のシグネチャは最小限を維持する。テストではモック Transport/RestClient/バンドルを差し込む。

## 7. 観測性導入ガイド（推奨セット）
- Tracer/Meter 名: ActivitySource=`ExchangeApi.RestClient`, Meter=`exchangeapi`。
- メトリクス: `exchangeapi_requests_total{endpoint,method,status,product_code,error}`, `exchangeapi_request_duration_seconds{endpoint,method,status,product_code,error}`。
- ログ: 構造化 JSON で `timestamp, event_type, method, uri/status_code, duration_ms(optional), product_code(optional), error(optional)` を記録し、機密（キー、署名）は出力しない。
- サンプル: `RestCallOpenTelemetryObserver`（OTelブリッジ）と `StructuredRestClientLogger` を組み合わせ、Factory で `WithObservability(...)` 経由またはオプション注入で適用する。

## 7. ドキュメント
- 信頼性パターンの推奨デフォルトとシナリオ別設定例を記載（低頻度トレード/高頻度ポーリングなど）。
- 観測性の利用方法（ログ項目例、メトリクス名、OTel サンプルコード）を提示。
- STAGES-OVERVIEW の Stage6 説明を REST-only 信頼性強化として更新。A010 の DoD を完成させる。

## 8. テスト / 検証
- Policy 単体: Retry/Timeout/RateLimiter/CircuitBreaker の成功/失敗/遷移ケース。
- 結合: Stage5 代表フローを劣化環境（遅延・一時失敗・429 模擬）で通す。
- Fault Injection: レートリミット・遅延・NW 障害をモックし、再試行や CB 開閉を検証。Paper/Sandbox 未整備でも Fault Injection で代替する方針。
- 計測: Public/Private のスループット・レイテンシを計測し、デフォルト値見直しの根拠として記録。
- Fault Injection 期待例: 連続 429 を 3 回発生→Retry の打ち切りと CB Open を確認、500ms の追加遅延を挿入→Timeout 発火と再試行挙動を確認、一時的な DNS/接続失敗→Transient 判定で 1 回だけ再試行。
- 計測手順例: `hey` などで Public/Private の代表エンドポイントを一定 QPS で叩き、p50/p95/p99 とエラー率を取得。遅延注入や 429 応答のモックを組み合わせ、初期デフォルト値の妥当性を評価して SPEC/コードに反映する。

## 9. 完了条件（DoD）
- デフォルト設定で Timeout/Retry/RateLimit/CircuitBreaker が有効になり、代表フローが成功する。
- 失敗分類タグとドメイン例外マッピングが整備され、Retry/CB がタグに従って動く。
- 観測性: ログ/メトリクス/トレースがサンプル経由で外部出力できる。
- ドキュメント: 推奨デフォルト・設定例・観測性サンプルが公開され、STAGES-OVERVIEW と A010 DoD が更新済み。
- テスト: Policy 単体と劣化環境結合、Fault Injection が緑。負荷/レイテンシ計測結果を反映済み。
