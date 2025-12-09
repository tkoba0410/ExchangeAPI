# Changelog
ExchangeApi プロジェクトの変更履歴を管理します。  
このファイルは [Semantic Versioning](https://semver.org/) に従います。

---

## [Unreleased] — Stage6 (in progress)
REST-only 方針で bitFlyer 縦スライスを Private まで実装し、信頼性/運用を強化しています。WebSocket/Realtime は廃止済み。

### Added
- bitFlyer Private REST: 残高/証拠金/ポジション/口座約定/オープン注文、`sendchildorder`、`cancelchildorder`、`cancelallchildorders`
- Stop/StopLimit（Stop+Price）を含む発注系のマッピングとポーリング実装
- Timeout/Retry/RateLimit/CircuitBreaker のデフォルトポリシーと観測性フック（構造化ログ/OTelサンプル Observer）
- `BitflyerClientFactory` とテスト用 Factory で Http/Signer/Policy 配線を簡略化

### Changed
- REST-only に統一（WS/Realtime は提供しない）
- ExchangeErrorCategory によるカテゴリ粒度のエラー分類フックを導入

### Planned (Next)
- 複数取引所対応の検証とドキュメント整備
- 信頼性テスト（劣化環境・Fault Injection）の拡充

---

## [v0.1.0] — Stage1 Final — 2025-xx-xx
Stage1 を完了し、ExchangeApi の **初期実装バージョン** を確定しました。  
本バージョンは「Public REST による Ticker 取得」を中心とした **最小実装 (MVP)** です。  
設計文書とコードの整合が取れた安定点であり、ここを基点として Stage2 に進みます。

### Added
- A000-STG1-GOAL-Vision を追加し、Stage1 の最終ゴールを文書化
- Abstractions 層に `Ticker` モデルと `IExchangeClient` を定義
- Infrastructure 層に以下を実装  
  - `IRestClient`  
  - `RestClient`（path + query形式へ整理）  
  - `IHttpTransport` / `HttpTransport`
- Bitflyer Adapter  
  - Public REST `/v1/getticker` の実装  
  - Raw JSON → Ticker 変換のマッピング整理
- DTO  
  - Timestamp を `DateTimeOffset` に統一  
  - bitFlyer ISO8601 と完全整合
- テスト  
  - Ticker DTO Test  
  - BitflyerExchangeClient Test  
  - RestClient URI 組み立て Test

### Changed
- A010〜A060 の Stage1 文書を全体的に改訂し、コードと完全整合
- `getticker` 参照を削除し、公式の `/v1/getticker` に統一
- Ticker Timestamp に関する仕様を確定（UTC + Offset、例外不要）
- README を Stage1 完了形に整備  
  - DI セットアップ  
  - Ticker 使用例  
  - プロジェクト構成

### Fixed
- REST クエリ文字列生成の揺らぎを排除  
  - UriBuilder に統一  
  - パラメータ順の揺れを解消
- Timestamp パース例外試験を削除（仕様と責務に合わないため）
- 古い API 名の表記揺れを完全排除

### Notes
- Stage1 は本バージョンをもって **凍結 (Freeze)**  
- Stage2 は Private REST / WS / Orchestration を中心に展開予定  
- 番号体系の再構築（Series + 4文字カテゴリ）は Stage2 で導入

---

## [v0.0.0] — Project Start — 2025-xx-xx
ExchangeApi プロジェクトの初期コミット。

### Added
- ソリューション構成（Abstractions / Infrastructure / Adapter / Tests）
- 基本的なディレクトリ構成  
- 最初の README（簡易）

---
