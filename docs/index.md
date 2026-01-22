# Documentation Index

## はじめに

本リポジトリの設計・仕様に関する**唯一の正本（Normative）**は、以下の文書です。

* **`docs/topspec.md`**

以降に配置されている文書は、すべて **TopSpec を参照する補助文書（Informative / Reference）** です。
本リポジトリの設計判断・層責務・API 契約については、必ず TopSpec を基準にしてください。

---

## 1. 設計仕様（Normative）

### TopSpec（正本）

* **`docs/topspec.md`**
  本ライブラリの層構造、責務分離、API サーフェス規則、Call 抽象、公開範囲を定義する唯一の規範文書。

---

## 2. 利用者向け契約（Contracts）

以下の文書は、**外部利用者に公開される安定 API（Contract 層）**の語彙・契約を定義します。

* `docs/contracts/overview.md`
  利用者向け契約の概要。公開安定面は Contract 層のみであることを説明。

* `docs/contracts/contracts.md`
  共通 DTO、Capability I/F、契約上の意味論を定義。

※ Raw / Normalized / Wire 層は内部実装または高度利用向けであり、公開安定 API ではありません。

---

## 3. Endpoint 一覧（Inventory）

取引所公式 API 文書を正本とし、本リポジトリでは **Endpoint の対応関係一覧（inventory）** のみを管理します。

* `docs/inventory/`

  * `endpoints-bitflyer.md`
  * `endpoints-bittrade.md`

※ inventory には規範や設計判断は記載しません。  
※ 層構造・公開範囲・API 契約に関する判断は **TopSpec（docs/topspec.md）** を参照してください。

---

## 4. EndpointId（対応・一覧）

以下の文書は、EndpointId の命名および既存エンドポイントとの対応関係を示す一覧・補助資料です。

* `docs/endpoint-id/endpointid-common.md`
* `docs/endpoint-id/endpointid-bitflyer.md`
* `docs/endpoint-id/endpointid-bittrade.md`
* `docs/endpoint-id/endpointid-code-mapping.md`

※ EndpointId に関する設計規範は TopSpec に従います。

---

## 5. エラー・例外

* `docs/exceptions.md`
  Call 抽象を前提としたエラー／失敗の分類と扱い方を定義。

---

## 6. 運用・ガバナンス（参考）

以下は設計正本ではなく、**運用・補助・参考資料**です。

* `docs/governance/exchanges-code-unification.md`
* `docs/process.md`
* `docs/document-plan.md`

これらの文書は TopSpec に反しない範囲でのみ有効です。

---

## 7. 参考資料

* `docs/_references/`

過去の設計検討や参考用文書を格納します。正本ではありません。

---

## 読み進め方（推奨）

1. **TopSpec を読む**（必須）
2. Contracts 文書で公開 API を確認
3. inventory / endpoint-id で対応エンドポイントを把握
4. 必要に応じて例外・運用文書を参照
