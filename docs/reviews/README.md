# docs/reviews README

> このディレクトリは **Reference（非規範）** です。ここでの記述は判断材料であり、最終的な採否や規約の裁定は Normative 文書（Governance / TopSpec）に従います。

## Index

- `REVIEW-01-naming.md`
- `REVIEW-02-parameters.md`
- `REVIEW-03-implementation.md`
- `REVIEW-04-layering.md`
- `REVIEW-05-cross-exchange.md`
- `REVIEW-06-constants.md`
- `REVIEW-07-boilerplate.md`

## 読む順（推奨）

1. `REVIEW-01`〜`REVIEW-04` で基本方針（命名・引数・実装・責務分離）を確認
2. `REVIEW-05` で取引所間の差分設計を確認
3. `REVIEW-06` で定数整理の方針を確認
4. `REVIEW-07` で定型化（ボイラープレート）の扱いを確認

## 運用ルール

- レビューで有効と判断した採用ルールは、この README や各 REVIEW に閉じず、**Governance / TopSpec に移植して規範化する**。
- `docs/reviews` は、規範化前の比較・検討メモを置く場所として使う。
- 規範化後は、対応する REVIEW から Normative 文書への参照を追記し、重複記述は最小化する。

## 変更時の参照ガイド

- **新規取引所の追加**: まず `REVIEW-05-cross-exchange.md` と `REVIEW-07-boilerplate.md` を参照する。
- **新規 endpoint の追加**: まず `REVIEW-05-cross-exchange.md` と `REVIEW-07-boilerplate.md` を参照し、必要に応じて `REVIEW-02-parameters.md` / `REVIEW-03-implementation.md` を確認する。


## Normative 反映先（採用ルール）

- REVIEW-01〜06 で採用された運用ルールは `docs/governance.md` の「8. REVIEW 採用ルール（Normative）」を正本とする。
- 各 REVIEW は引き続き検討過程・比較観点を残す **Reference（非規範）** として扱う。
