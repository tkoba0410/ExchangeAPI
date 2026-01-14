# Docs Index

このディレクトリ（`docs/`）は、本リポジトリにおける**設計判断・契約・運用ルールの正本**を集約した場所である。
迷った場合は、必ず本ページから辿ること。

---

## はじめに

* 文書化の方針・ルール（最優先）：[`documentation-policy.md`](./documentation-policy.md)

---

## 設計（Why / What）

* **TopSpec**（設計原則・全体俯瞰）：[`topspec.md`](./topspec.md)
* **TopSpec Guide**（読み方・誤用防止）：[`topspec-guide.md`](./topspec-guide.md)

---

## 境界（Rules）

* **Boundaries**（層境界の正本）：[`boundaries.md`](./boundaries.md)

---

## 契約（Shape / Semantics）

* **Contracts**（DTO 形状・意味論）：[`contracts.md`](./contracts.md)

---

## 一覧（Fact）

* **Endpoints Inventory**（API エンドポイント一覧・最小形）：[`endpoints.md`](./endpoints.md)

---

## 運用（Process）

* **Review Checklist**（PR 最終判断装置）：[`review-checklist.md`](./review-checklist.md)

---

## 例外（Decision）

* **Exceptions Ledger**（設計逸脱の台帳）：[`exceptions.md`](./exceptions.md)

---

## 読み進め方の指針

* 設計思想や全体像を把握したい場合：**TopSpec → Boundaries → Contracts**
* 実装・変更時の判断に迷った場合：**Boundaries / Contracts / Review Checklist**
* 特定の API 利用有無を確認したい場合：**Endpoints Inventory**
* 原則からの逸脱が必要な場合：**Exceptions Ledger**

---

※ 本ページ自体は判断を定義しない。判断の正本は各リンク先文書とする。
