# Stage6 → Stage7 バトン（REST-only 信頼性強化から複数取引所・DX仕上げへ）

## Stage6 振り返り（実施済みの要点）
- REST-only 方針を維持したまま信頼性パターンを実装（Timeout/Retry/RateLimit トークンバケット＋バースト/CircuitBreaker）。デフォルト値を `HttpPolicyOptions` で管理し Factory で適用。
- エラー分類を E2/E3 相当へ拡張し、`IExchangeErrorClassifier` + bitFlyer マッピングで `ExchangeErrorCategory` を付与。Retry/CB はカテゴリベースで判定。
- 観測性を標準化：`IRestCallObserver` + OTel ブリッジ（`exchangeapi_requests_total`, `exchangeapi_request_duration_seconds` などのタグ付きメトリクス）と構造化JSONログを追加。`WithObservability(...)` で適用例を提示。
- DX/テストシームを整理：`BitflyerClientOptions` 一本化、TestFactory/ApiBundle/InternalsVisibleTo でモック注入を分離し、本番APIのシンプルさを維持。
- テスト拡充：ポリシー単体、Fault Injection（429/一時断/タイムアウト/CB開放）、観測性メトリクス発行、劣化環境E2E（代表フロー拡張版：残高→注文→約定確認→履歴→キャンセル→ポジション→証拠金）を整備。ドキュメントも同期。

## 未完・持ち越し（Stage6 時点）
- デフォルト値の本番実測確定：遅延/429/500 モックによる簡易計測は方針のみ。Timeout/Retry/RL/CB の最終値を計測結果で確定する必要あり。
- 劣化環境E2Eの実環境妥当性確認：モックで正式フロー（残高→注文→約定確認→決済→履歴→キャンセル→ポジション→証拠金）は完了。実環境に寄せた負荷/遅延条件での再検証は次ステージで判断。
- TestFactory/ApiBundle誤用防止の運用徹底：必要ならCIで本番コードからの参照がないことをチェック。

## Stage7 に引き継ぐべきポイント
- 信頼性/観測性のパターンと命名を共通仕様として流用可能：他取引所対応やDX仕上げで再利用する。
- `HttpPolicyOptions`/`BitflyerClientOptions` を多取引所対応の設計に発展させる（抽象オプション化）。
- 観測性の推奨セット（ActivitySource/Meter名、メトリクス/タグ、構造化ログ項目）を標準ドキュメントとして他取引所にも適用。
- テストシーム（TestFactory/ApiBundle）を共通化し、各取引所の劣化環境E2Eを同じパターンで書けるようにする。

## Stage7 での優先案（たたき台）
1. デフォルト値の計測確定：Public/Private で簡易負荷・遅延/429/500 モック計測を行い、Timeout/Retry/RL/CB を確定し、共通オプションに反映。
2. 劣化環境E2Eの正式フロー化：決済/履歴詳細まで含めた代表フローを劣化条件下で通すテストを完成させ、DoDに組み込む（現在は履歴/キャンセル/ポジション/証拠金まで完了）。
3. 抽象オプション/観測性の共通化：多取引所対応を見越し、ClientOptions/PolicyOptions/Observabilityの抽象レイヤを設計し直す（名称とタグ体系を固定）。
4. 他取引所パイロット（最小縦スライス）：Public/Privateの一部機能を新取引所で実装し、Stage6で作った信頼性・観測性・テストシームがそのまま使えるか検証。
5. 運用ガイドの整備：OTel/構造化ログの設定例、TestFactory誤用防止のルール、計測/劣化E2Eの手順をまとめてDXを仕上げる。

## 気を付けること
- 公開APIのシンプルさを保つ：Test用シームはInternalsVisibleToやTestFactoryに隔離し、多取引所化やDX強化でも外部APIを肥大化させない。
- 命名/タグの一貫性：メトリクス/ログ/トレースのキーは他取引所でも共通化し、ダッシュボード横展開を容易にする。
- 計測の反映をサボらない：デフォルト値は必ず計測に基づいて反映し、SPEC/TODOに記録して固定する。
