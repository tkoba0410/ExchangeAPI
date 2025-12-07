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

## 5. 設定と DX
- `BitflyerClientOptions` に Timeouts/Retry/RateLimit/CircuitBreaker/LoggingVerbosity を束ね、`WithObservability(...)` で Observer 注入を提供。最小設定の安全デフォルトと上級者向け詳細設定を両立する API シグネチャを確定。

## 6. ドキュメント
- 信頼性パターンの推奨デフォルトとシナリオ別設定例、ログ/メトリクス/トレースの取り扱いサンプルを追加。STAGES-OVERVIEW の Stage6 説明を REST-only 信頼性強化に更新し、A010 の DoD を完成させる。

## 7. テスト/検証
- Policy 単体テスト（成功/失敗/サーキット遷移）を優先実装し、次に劣化環境で Stage5 代表フローの結合テスト。
- レートリミット・遅延・ネットワーク障害を模擬する Fault Injection テストを整備。Paper/Sandbox 未整備でも Fault Injection で代替する方針を前提にする。
- Paper Trading/Sandbox/ドライラン利用方針を記載した運用ガイドを添える。
