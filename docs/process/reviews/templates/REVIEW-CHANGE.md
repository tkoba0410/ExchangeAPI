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

### Breaking Change明示

* 判定基準: public surface変更時はCHANGE記録必須
* OK条件: Public Surface変更がCHANGEに明示され、影響と移行が記録済み
* NG条件: 破壊的変更または挙動変更が記録されていない
* 不合格例: DTO変更だが記録なし
* 該当Fatal: F2（SSOT逸脱）
* 修正方針: CHANGE文書に影響範囲・移行方法・Bot影響を追記

### 影響範囲明確化

* 判定基準: 変更の影響対象（利用者 / モジュール / 実行時挙動）が特定されている
* OK条件: 影響対象が列挙され、非影響範囲も明示されている
* NG条件: 何が壊れる可能性があるかが記載されていない
* 不合格例: 「内部修正のみ」と記載しつつ公開DTOが変更されている
* 該当Fatal: F2（影響未記載で契約誤認を招く場合）
* 修正方針: API・データ・運用の3観点で影響範囲を明文化する

### 移行方法提示

* 判定基準: 既存利用者が安全に移行できる手順が定義されている
* OK条件: 新旧差分、移行順、期限（必要時）が具体化されている
* NG条件: 変更は示すが移行手順が不在
* 不合格例: 旧フィールド廃止だけ記載し置換先の説明がない
* 該当Fatal: F2（Breaking Changeで移行不能な場合）
* 修正方針: 最小移行手順と互換期間をCHANGEに追記する

### Bot影響記載

* 判定基準: Bot/自動実行系への互換影響が評価されている
* OK条件: Bot影響の有無と必要対応（設定変更・再学習・再検証）が明記される
* NG条件: Bot影響が未評価または無根拠に「なし」とする
* 不合格例: 注文系レスポンス変更にもかかわらずBot影響欄が空欄
* 該当Fatal: F2（影響隠れで運用障害を誘発する場合）
* 修正方針: Bot依存点を一覧化し、影響有無をCHANGEへ固定記録する

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
