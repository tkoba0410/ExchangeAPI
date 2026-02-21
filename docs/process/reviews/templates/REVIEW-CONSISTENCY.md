# REVIEW-CONSISTENCY

本レビューは Consistency（命名 / 語彙 / 構造整合）軸に基づく確認を行う。

重大度定義は `PROJECT-FATAL-DEFINITION.md` を参照する。
重大度は `Severity` と `FatalClass` の 2 軸で記録すること。
`Severity=Fatal` の場合は `FatalClass=F1〜F5` を明示すること。

---

## 0. 対象

* PR番号:
* 対象範囲:
* 対象層（Core / Application / Wire / Raw / Normalized / Contracts）:
* 変更概要:

---

## 1. 判定サマリ

| 観点 | 判定 | Severity (Fatal/High/Medium/Low/Nit) | FatalClass (F1-F5/None) | CI化可否 | 備考 |
| --- | --- | --- | --- | --- | --- |
| EndpointId整合 |  |  |  |  |  |
| 概念分裂 |  |  |  |  |  |
| 定数/enum統一 |  |  |  |  |  |
| Cross-exchange parity |  |  |  |  |  |

---

## 2. 観点詳細

### EndpointId整合

* 判定基準: EndpointId起点で命名と参照が一意に対応する
* OK条件: 実装・inventory・レビュー記録で同一EndpointIdが一貫使用される
* NG条件: 同一機能で複数EndpointIdが併存、または別機能が同一EndpointIdを共有
* 不合格例: 同機能のCallに別名EndpointIdが追加される
* 該当Fatal: F2（SSOT逸脱）
* 修正方針: EndpointIdを正本へ統一し、別名をAliasesへ退避する

### 概念分裂

* 判定基準: 同義概念の型が重複していない
* OK条件: EndpointId起点の命名と語彙が一意に保たれている
* NG条件: 同義概念の別名化・別DTO化が発生している
* 不合格例: 同意味DTOが別名で存在
* 該当Fatal: F1 または F2（構造破壊 / SSOT逸脱）
* 修正方針: 代表語彙へ統合し、inventory/規範へ名称を一本化

### 定数/enum統一

* 判定基準: 同じ意味の定数値・enum値が複数定義されない
* OK条件: 単一の定数/enum定義を全層で参照する
* NG条件: 文字列直書きや重複enumで意味が分岐している
* 不合格例: 同一状態値を複数enumで別名表現している
* 該当Fatal: F2（正本語彙と不整合な場合）
* 修正方針: 正準enumへ集約し、重複定義を段階的に削除する

### Cross-exchange parity

* 判定基準: 取引所差異を保ったまま正規化観点が一貫している
* OK条件: 同一概念の入出力構造が取引所間で比較可能に揃う
* NG条件: 取引所ごとに意味や粒度が不一致で比較不能
* 不合格例: A取引所のみ別意味のフィールド名を共通DTOに流入
* 該当Fatal: F1（境界破壊）または F2（SSOT逸脱）
* 修正方針: 差異はAdapter以下へ閉じ込め、共通語彙へ再正規化する

---

## 3. CI自動化候補

* enum直書き検出

---

## 4. 関連Normative / 判例

* docs/normative/naming-rules.md
* docs/normative/topspec.md
* docs/process/process.md（7.2）
* docs/process/reviews/templates/PROJECT-FATAL-DEFINITION.md

---

## 5. 最終結論

* OK / 要修正 / NG
