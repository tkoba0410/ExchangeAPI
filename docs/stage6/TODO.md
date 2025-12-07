# Stage6 やることリスト（REST-only 信頼性・運用強化）

## 1. 実装フェーズと優先度（デメリット克服の軸）
- ① Timeout/Retry デフォルト導入 → ② RateLimiter → ③ CircuitBreaker → ④ 観測性フック → ⑤ Factory オプション拡張 → ⑥ Fault Injection/結合テスト。各ステップでビルド/テストを緑にしてから進む。
- デフォルト値を早期合意: Timeout=Public 5s / Private 8–10s、Retry=GET 最大3回（指数2x, max 4s）・POSTはネットワーク一時障害のみ1回、RateLimit=Public 5req/s・Private 3req/s（実測で調整）、CircuitBreaker=20s 窓で失敗率>50%で Open、5s 後 Half-Open。

### 進行順序（チェックポイント付き）
1. Timeout/Retry: デフォルト適用で代表フローが通ることを確認（ポリシー単体テスト緑）。
2. RateLimiter: RL 適用後もフロー通過、429 モック時の挙動確認。
3. CircuitBreaker: 失敗連続で Open/Half-Open/Close 遷移がテストで確認できる。
4. 観測性: Observer/Logger がメトリクス・ログを出力するサンプルが動作。
5. Factory オプション: `BitflyerClientOptions`/`WithObservability(...)` 経由で設定が反映される E2E テスト緑。
6. Fault Injection/結合: 遅延/429/一時断を注入した劣化環境で Stage5 代表フロー成功。

## 2. 信頼性パターン（デフォルト込み）
- Policy 層で RateLimiter（固定間隔/トークンバケット相当）、Retry、Timeout、CircuitBreaker を構成可能にし、安全寄りデフォルトを Factory から提供。サーキット状態など可視化ポイントを定義。
- CircuitBreaker/Retry は失敗分類に紐づけ、再試行可否を一元管理。

## 3. エラー分類 E2/E3
- `ExchangeApiException` を認証/権限・レートリミット・一時的ネットワーク・業務エラーに整理し、bitFlyer エラーコード→ドメイン例外のマッピング表と再試行可否基準を整備。

## 4. 観測性フック
- `IRestCallObserver`（コールバック）でログ/メトリクス/トレースを集約し、`IRestClientLogger` 拡張で RequestId/エンドポイント/所要時間/HTTP ステータス/主要ドメイン属性を記録（秘密情報除外）。OpenTelemetry に流す薄いアダプタをサンプル実装。
- 実装済み: `RestCallOpenTelemetryObserver` で Activity/Meter を発行（メトリクス名: `exchangeapi_requests_total`, `exchangeapi_request_duration_seconds`、タグ: endpoint/method/status/product_code/error）。構造化 JSON ログサンプル `StructuredRestClientLogger` を追加。

## 5. 設定と DX
- `BitflyerClientOptions` に Timeouts/Retry/RateLimit/CircuitBreaker/LoggingVerbosity を束ね、`WithObservability(...)` で Observer 注入を提供。最小設定の安全デフォルトと上級者向け詳細設定を両立する API シグネチャを確定。
- 実装済み: `BitflyerClientOptions` と拡張メソッド `WithObservability(...)` を追加し、Factory でオプション経由の構成に対応。`HttpPolicyOptions` に `RateLimitBurst` を追加し、トークンバケット型 RateLimiter をデフォルト使用。

## 6. ドキュメント
- 信頼性パターンの推奨デフォルトとシナリオ別設定例、ログ/メトリクス/トレースの取り扱いサンプルを追加。STAGES-OVERVIEW の Stage6 説明を REST-only 信頼性強化に更新し、A010 の DoD を完成させる。

## 7. テスト/検証
- Policy 単体テスト（成功/失敗/サーキット遷移）を優先実装し、次に劣化環境で Stage5 代表フローの結合テスト。
- レートリミット・遅延・ネットワーク障害を模擬する Fault Injection テストを整備。Paper/Sandbox 未整備でも Fault Injection で代替する方針を前提にする。
- Paper Trading/Sandbox/ドライラン利用方針を記載した運用ガイドを添える。
- 実装済み: Transport レベルで 429/一時断/タイムアウト/CB 開放を検証する Fault Injection テストを追加。TestFactory/モック Transport を用いた劣化環境 E2E（残高→注文→約定確認→履歴）を追加済み。
- 方針: 劣化環境では Stage5 代表フロー（残高→注文→約定確認→決済→履歴）をモックTransportで再現し、E2E テストを追加して DoD を満たす。簡略フローではなく正式フローを対象とする。

## 設計方針アップデート（シンプル化と将来性優先）
- オプション一本化: `BitflyerClientOptions` に HttpClient/Transport/RestClient/Policy/Logger/Observer/ErrorClassifier を束ね、Factory は基本このオプション 1 つを受ける形に簡素化する。
- テスト用シームの分離: 公開 API を汚さず、Tests アセンブリ限定の TestFactory（InternalsVisibleTo）を用意し、`IHttpTransport`/`IRestClient`/モック API バンドルを直接注入できるようにする。
- API バンドル化: Public/Private/Raw のセットをまとめたバンドル DTO を用意し、Facade の internal コンストラクタで受け取れるようにする。本番は Factory が正規バンドルを組み立て、テストはモックバンドルを注入。
- 可視性の整理: Facade に Public/Private/Raw への読み取り専用プロパティを internal で持たせ、Tests からのみ利用可とする。公開 API のシンプルさを維持。
- ドキュメント反映: 上記構成（Options一本化、TestFactory、APIバンドル、InternalsVisibleTo）を SPEC に明記し、テスト/本番の顔を分離する方針を定義する。
- 観測性ガイド: Tracer/Meter 名（ActivitySource=`ExchangeApi.RestClient`, Meter=`exchangeapi`）、メトリクス/タグ（requests_total, request_duration_seconds with endpoint/method/status/product_code/error）、構造化ログ項目（機密除外）を推奨セットとしてドキュメント化し、`WithObservability(...)` での適用例を示す。
- デフォルト値調整: Public/Private の代表エンドポイントで簡易計測（遅延/429/500 モック）を行い、Timeout/Retry/RL/CB の値を調整して SPEC/コードに固定する手順を記録。結果を DoD に反映。
- internal シーム利用ガイド: TestFactory/ApiBundle の使い方と公開 API の使い分けを開発者向けに明記し、公開 API は最小、内部は Tests 限定とするルールを整理。
