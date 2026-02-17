# Documentation Index

## はじめに

本リポジトリの文書は、対象領域ごとに正本（Normative）を定義する。
本リポジトリでは「正本（Normative）」「Source of Truth（SSOT）」を同義として扱う。
ただし inventory は「事実の正本（Fact SSOT）」であり、設計規範の正本ではない。

以下は、それぞれの領域における正本である。

* **技術仕様・設計規範**：`docs/normative/topspec.md`
* **命名規則**：`docs/normative/naming-rules.md`
* **公開 API 契約**：`docs/normative/contracts/contracts.md`
* **設計判断の裁定**：`docs/normative/governance.md`

以降に配置されている文書は、上記正本を基準とした
補助文書（Informative / Reference）である。

技術仕様・設計規範については TopSpec を、
公開 API 契約については Contracts を、
設計判断の裁定については Governance を基準として参照すること。

---

## 1. 設計仕様（Normative）

### TopSpec（技術正本）

> 補足: レジリエンス契約（429 / Timeout / Partial Failure）は
> `docs/normative/contracts/resilience.md` を正本とする。

* **`docs/normative/topspec.md`**
  本ライブラリの層構造、責務分離、API サーフェス規則、Call 抽象、公開範囲を定義する唯一の技術規範。

### Naming Rules（命名規則）

* **`docs/normative/naming-rules.md`**
  命名・語彙・DTO 接尾辞などの規則を定義する補助規範。

### Governance（裁定ルール）

* **`docs/normative/governance.md`**
  技術仕様を再定義せず、設計判断の衝突時における優先順位・禁止事項・差異の閉じ込め先を定める裁定文書。

---

## 2. 利用者向け契約（Contracts）

以下の文書は、**外部利用者に公開される安定 API（Contract 層）**の語彙と契約を定義します。

* `docs/normative/contracts/overview.md`
  利用者向け契約の概要。公開安定面は Contract 層のみであることを説明。

* `docs/normative/contracts/contracts.md`
  共通 DTO、Capability I/F、契約上の意味論を定義。

* `docs/normative/contracts/resilience.md`
  429 / Timeout / Partial Failure の公開契約を定義。

※ Raw / Normalized / Wire 層は内部実装または高度利用向けであり、公開安定 API ではありません。

---

## 3. Endpoint 一覧（Inventory / Fact）

取引所公式 API 文書を最上位の正本とし、本リポジトリでは **Endpoint の対応関係一覧（inventory）** のみを管理します。

* `docs/inventory/`

  * `endpoints-contracts.md` — Contracts API 採用/対応関係の SSOT
  * `endpoints-bitflyer.md`
  * `endpoints-bittrade.md`

※ inventory には設計規範・命名規則・判断文は記載しません。
※ 並び順は **公式 API 文書の記載順**を正とします。

---

## 4. Exceptions（逸脱台帳 / 決定記録）

* `docs/process/exceptions.md`
  原則からの逸脱理由・影響範囲・将来対応を記録する台帳です。
  本章の「Exceptions」は設計上の逸脱記録を指し、runtime エラー分類は扱いません。
  runtime のエラー契約は `docs/normative/contracts/resilience.md` を参照してください。

---

## 5. 運用・プロセス（参考）

以下は設計正本ではなく、**運用・作業・計画のための補助資料**です。

* `docs/process/process.md`
* `docs/process/public-surface.md`
* `stage9.md`
* `docs/archive/document-plan.md`
* `docs/reference/utilities.md`

これらの文書は TopSpec / Contracts / Governance に反しない範囲でのみ有効です。

---

## 6. 参考資料（Reference）

* `docs/reference/`
* `docs/archive/references/`

過去の設計検討、実装ガイド、対応関係メモなどを格納します。
本ディレクトリ配下の文書は **いかなる場合も規範ではありません**。

---

## 読み進め方（推奨）

1. **TopSpec を読む**（必須）
2. Governance で裁定ルールを把握
3. Contracts 文書で公開 API を確認
4. inventory で公式 API との対応関係を確認
5. 必要に応じて Exceptions（逸脱台帳）と参考文書を参照
