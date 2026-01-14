# Docs Index

このディレクトリ（`docs/`）は、本リポジトリにおける**設計判断・契約・運用ルールの正本**を集約した場所である。
迷った場合は、必ず本ページから辿ること。

---

## はじめに

* 運用ルール（文書化・レビュー・例外）：[`process.md`](./process.md)

---

## 規範（Normative）

* **TopSpec**（最上位規範）：[`topspec.md`](./topspec.md)
* **Contracts**（横断契約の規範）：[`contracts.md`](./contracts.md)

---

## 一覧（Inventory / Fact）

* **Endpoints Inventory**（API エンドポイント一覧・最小形）：[`endpoints.md`](./endpoints.md)
* **Inventory: Bitflyer**：[`inventory-bitflyer.md`](./inventory-bitflyer.md)
* **Inventory: Bittrade**：[`inventory-bittrade.md`](./inventory-bittrade.md)

---

## 例外（Decisions）

* **Exceptions Ledger**（設計逸脱の台帳）：[`exceptions.md`](./exceptions.md)

---

## 運用（Process）

* **Review Checklist**（PR 最終判断装置）：[`review-checklist.md`](./review-checklist.md)

---

## 参考（Reference / Legacy）

* 旧文書は互換のため `docs/_references/` に保管する（Normative ではない）。
* `docs/_references/documentation-policy-legacy.md`
* `docs/_references/review-checklist-legacy.md`
* `docs/_references/boundaries-legacy.md`
* `docs/_references/topspec-guide-legacy.md`
* `docs/_references/contracts-legacy.md`

---

## 読み進め方の指針

* 最上位の判断（禁止/許可）を確認したい：**TopSpec → Contracts**
* 実装・変更時の判断に迷った場合：**TopSpec / Contracts / Review Checklist**
* 特定の API 利用有無を確認したい：**Endpoints / Inventory**
* 原則からの逸脱が必要な場合：**Exceptions Ledger**

---

※ 本ページ自体は判断を定義しない。判断の正本は各リンク先文書とする。
