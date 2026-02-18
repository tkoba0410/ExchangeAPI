# REVIEW-CHANGE

本レビューは Change（変更統治）軸に基づく確認を行う。

重大度定義は PROJECT-FATAL-DEFINITION.md を参照する。
Fatal判定時は F番号を明示すること。

---

## 0. 対象

* PR番号:
* 変更概要:
* Public Surface影響:
* 対象層（Contracts / Application / Composition / Docs）:

---

## 1. 判定サマリ

| 観点                | 判定 | 重大度 (F番号明示) | CI化可否 | 備考 |
| ----------------- | -- | ----------- | ----- | -- |
| Breaking Change明示 |    |             |       |    |
| 影響範囲明確化           |    |             |       |    |
| 移行方法提示            |    |             |       |    |
| Bot影響記載           |    |             |       |    |

---

## 2. 観点詳細

### Breaking Change未記録

* 判定基準: public surface変更時はCHANGE記録必須
* OK条件: Public Surface変更がCHANGEに明示され、影響と移行が記録済み
* NG条件: 破壊的変更または挙動変更が記録されていない
* 不合格例: DTO変更だが記録なし
* 該当Fatal: F2（SSOT逸脱）
* 修正方針: CHANGE文書に影響範囲・移行方法・Bot影響を追記

---

## 3. CI自動化候補

* public surface diff検出

---

## 4. 関連Normative / 判例

* docs/process/CHANGE-*.md
* docs/process/process.md（7.2）
* docs/normative/contracts/contracts.md
* docs/process/reviews/templates/PROJECT-FATAL-DEFINITION.md

---

## 5. 最終結論

* OK / 要修正 / NG
