# REVIEW-DOCS

本レビューは Docs（文書構造 / SSOT）軸に基づく確認を行う。

重大度定義は PROJECT-FATAL-DEFINITION.md を参照する。
Fatal判定時は F番号を明示すること。

---

## 0. 対象

* PR番号:
* 文書範囲:
* 変更概要:

---

## 1. 判定サマリ

| 観点                 | 判定 | 重大度 (F番号明示) | CI化可否 | 備考 |
| ------------------ | -- | ----------- | ----- | -- |
| 正本/参照混線            |    |             |       |    |
| review-framework整合 |    |             |       |    |
| 用語統一               |    |             |       |    |
| 重複文書               |    |             |       |    |

---

## 2. 観点詳細

### 正本/参照混線

* 判定基準: ルールは正本に集約
* NG例: READMEに規約記載
* 該当Fatal: F2（SSOT逸脱）

---

## 3. CI自動化候補

* 正本リンク先の解決性検査（dead link / 相対リンク崩れ）
* docs変更時の CHANGE / exceptions 更新漏れ検査
* docs/reference の Non-Normative 明示検査（将来導入時）

---

## 4. 関連Normative / 判例

* docs/process/process.md（7.2）
* docs/normative/topspec.md
* docs/process/exceptions.md
* docs/process/reviews/templates/PROJECT-FATAL-DEFINITION.md

---

## 5. 最終結論

* OK / 要修正 / NG
