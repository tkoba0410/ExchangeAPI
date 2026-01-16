# Docs Index

このディレクトリ（`docs/`）は、本リポジトリにおける**設計判断・契約・運用ルールの正本**を集約した場所である。
迷った場合は、必ず本ページから辿ること。

---

## はじめに

* 運用ルール（文書化・レビュー・例外）：[`process.md`](./process.md)

---

## Plan

* 文書整備計画：[`document-plan.md`](./document-plan.md)

---

## 規範（Normative）

* **TopSpec**（最上位規範）：[`topspec.md`](./topspec.md)
* **Contracts Overview**：[`contracts/overview.md`](./contracts/overview.md)
* **Contracts**（横断契約の規範）：[`contracts/contracts.md`](./contracts/contracts.md)
* 物理構成方針（Shared 廃止後の正本）: TopSpec（`docs/topspec.md`）
* Application / Composition の配置規範も TopSpec に含まれる
* Application は Contracts を参照しない（詳細は TopSpec）

---

## 一覧（Inventory / Fact）

* **Endpoints Inventory**（API エンドポイント一覧・最小形）：[`inventory/endpoints.md`](./inventory/endpoints.md)
* **Inventory: Bitflyer**：[`inventory/inventory-bitflyer.md`](./inventory/inventory-bitflyer.md)
* **Inventory: Bittrade**：[`inventory/inventory-bittrade.md`](./inventory/inventory-bittrade.md)

---

## 例外（Decisions）

* **Exceptions Ledger**（設計逸脱の台帳）：[`exceptions.md`](./exceptions.md)

---

## 運用（Process）

* **Process**（文書化・レビュー・例外運用）：[`process.md`](./process.md)

---

## 参考（Reference）

- [documentation-design-notes.md](./_references/documentation-design-notes.md)

> これらは背景資料であり、正本ではない。

---

## 読み進め方の指針

* 最上位の判断（禁止/許可）を確認したい：**TopSpec → Contracts**
* 実装・変更時の判断に迷った場合：**TopSpec / Contracts / Review Checklist**
* 特定の API 利用有無を確認したい：**Endpoints / Inventory**
* 原則からの逸脱が必要な場合：**Exceptions Ledger**

---

※ 本ページ自体は判断を定義しない。判断の正本は各リンク先文書とする。
