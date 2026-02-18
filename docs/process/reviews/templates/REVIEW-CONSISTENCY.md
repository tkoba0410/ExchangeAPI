# REVIEW-CONSISTENCY

本レビューは Consistency（命名 / 語彙 / 構造整合）軸に基づく確認を行う。

重大度定義は PROJECT-FATAL-DEFINITION.md を参照する。
Fatal判定時は F番号を明示すること。

---

## 0. 対象

* PR番号:
* 対象範囲:
* 対象層（Core / Application / Wire / Raw / Normalized / Contracts）:
* 変更概要:

---

## 1. 判定サマリ

| 観点                    | 判定 | 重大度 (F番号明示) | CI化可否 | 備考 |
| --------------------- | -- | ----------- | ----- | -- |
| EndpointId整合          |    |             |       |    |
| 概念分裂                  |    |             |       |    |
| 定数/enum統一             |    |             |       |    |
| Cross-exchange parity |    |             |       |    |

---

## 2. 観点詳細

### 概念分裂

* 判定基準: 同義概念の型が重複していない
* OK条件: EndpointId起点の命名と語彙が一意に保たれている
* NG条件: 同義概念の別名化・別DTO化が発生している
* 不合格例: 同意味DTOが別名で存在
* 該当Fatal: F1 または F2（構造破壊 / SSOT逸脱）
* 修正方針: 代表語彙へ統合し、inventory/規範へ名称を一本化

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
