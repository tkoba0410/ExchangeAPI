# Changelog
ExchangeApi プロジェクトの変更履歴を管理します。  
このファイルは [Semantic Versioning](https://semver.org/) に従います。

---

## [Unreleased]
次期リリース（Stage2）に向けた作業領域です。  
Private REST / 認証 / WebSocket / Orchestration の設計を含みます。

### Planned (Stage2)
- Private REST API（Orders / Positions / Executions / Balance）
- 認証（HMAC, API Key 管理）
- WebSocket（Public / Private）
- Error Model の詳細化
- Orchestration（Multi-Exchange）基盤の準備
- 番号体系（Series / 4文字カテゴリ）導入
- Gシリーズ（Goal体系）の正式採用

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
  - Public REST `/v1/ticker` の実装  
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
- `getticker` 参照を削除し、公式の `/v1/ticker` に統一
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
