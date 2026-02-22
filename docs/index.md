# Documentation Index

## はじめに

本リポジトリの文書は、対象領域ごとに正本（Normative）を定義する。
本リポジトリでは SSOT（Source of Truth）を「参照すべき正本」という広い意味で用いる。
そのうち **Normative** は「拘束力（MUST / MUST NOT）を持つ正本」を指す。

※ inventory は「事実の正本（Fact SSOT）」であり、設計規範（拘束力）の正本ではない。
※ Process 配下には運用上の SSOT（例: review-framework）も存在するが、設計規範の正本ではない。

以下は、それぞれの領域における正本（Normative）である。

* **技術仕様・設計規範**：`docs/normative/topspec.md`
* **取引所モジュール物理構成（機械可読）**：`docs/normative/layout/exchange-module-shape.json`
* **命名規則**：`docs/normative/naming-rules.md`
* **公開 API 契約（条文）**：`docs/normative/contracts/contracts.md`
* **失敗時契約（429 / Timeout / Partial Failure）**：`docs/normative/contracts/resilience.md`
* **設計判断の裁定**：`docs/normative/governance.md`

以下では、上記正本を起点に、利用導線と補助文書を示す。

技術仕様・設計規範については TopSpec と Layout Shape を、
公開 API 契約については Contracts を、
失敗時契約については Resilience を、
設計判断の裁定については Governance を基準として参照すること。

---

## 最短導線（目的別）

* **初見でまず動かしたい（最初の1コール）**：`README.md`（Quickstart）
* **安定重視（取引所横断）**：`docs/normative/contracts/overview.md` → `docs/normative/contracts/contracts.md` → `docs/inventory/`
* **Bot / 高度利用（取引所別の機能網羅）**：`docs/process/public-surface.md` → `src/Exchanges/*/Normalized/Api` → `docs/inventory/`
* **実装/貢献**：`docs/normative/topspec.md` → `docs/process/process.md` → `docs/process/review-framework.md`

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

以下の文書は、**外部利用者に公開される安定 API（Contract 層）**に関する文書群です。

* `docs/normative/contracts/overview.md`
  利用者向け契約の概要と導線。公開 API 契約条文は `docs/normative/contracts/contracts.md`、失敗時契約は `docs/normative/contracts/resilience.md` を参照。

* `docs/normative/contracts/contracts.md`
  共通 DTO、Capability I/F、契約上の意味論を定義。

* `docs/normative/contracts/resilience.md`
  429 / Timeout / Partial Failure の公開契約を定義。

※ Raw / Normalized / Wire 層は内部実装または高度利用向けであり、公開安定 API ではありません。
※ 取引所別の機能網羅を優先する場合（Bot / 高度利用）、Normalized を主利用面として利用できます（追従前提）。
※ 取引所横断で安定性を優先する場合、Contracts を最小横断面として利用します。
※ 安定保証の対象は Contracts のみです。

### 失敗時の対処（最小）

* 429 / Timeout / Partial Failure: `docs/normative/contracts/resilience.md`
* 認証キー/資格情報（暗号化運用）: `docs/process/templates/README.md`
* 公開面の選択（Contracts / Normalized）: `docs/process/public-surface.md`

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
* `docs/process/review-framework.md`
* `docs/process/codex-review-runbook.md`
* `docs/process/reviews/`（実施済みレビュー監査ログ）
* `docs/process/reviews/README.md`
* `docs/process/reviews/templates/`
* `docs/process/templates/README.md`
* `docs/process/revision-history.md`
* `stage9.md`
* `docs/archive/document-plan.md`
* `docs/reference/utilities.md`

これらの文書は TopSpec / Layout Shape / Contracts / Resilience / Governance に反しない範囲でのみ有効です。

---

## 6. 参考資料（Reference）

* `docs/reference/`（`docs/reference/reviews/` を含む。過去レビューの判例・参考）
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
