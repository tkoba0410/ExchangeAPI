# REVIEW-DOCS

本レビューは Docs（文書構造 / SSOT）軸に基づく確認を行う。

重大度定義は PROJECT-FATAL-DEFINITION.md を参照する。
Fatal判定時は F番号を明示すること。

---

## 0. 対象

* PR番号:
* 文書範囲:
* 対象層（Normative / Process / Inventory / Reference）:
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
* OK条件: 規範ルールが TopSpec / Contracts / Governance / Process の正本に集約されている
* NG条件: 参照文書に実質的な新規ルールが記載され、正本と役割が逆転している
* 不合格例: READMEに規約記載
* 該当Fatal: F2（SSOT逸脱）
* 修正方針: ルール本体を正本へ移管し、参照側はリンクのみ残す

### review-framework整合

* 判定基準: レビュー運用が review-framework の軸/深度/トリガ定義と一致
* OK条件: 7軸・L1/L2/L3・トリガ条件が運用文書と矛盾しない
* NG条件: 運用文書が独自の軸や必須手順を追加し、framework と不整合
* 不合格例: runbook にのみ存在する独自必須軸を運用している
* 該当Fatal: F2（SSOT逸脱）
* 修正方針: review-framework を正本として差分を吸収し、重複定義を削除

### 用語統一

* 判定基準: 同一概念に対して同一語彙を用いる
* OK条件: PR/branch/axis/severity などの主要語彙が全運用文書で統一
* NG条件: 同義語が混在し、判定や運用手順の解釈が分岐する
* 不合格例: 同じ意味で「Must」と「NG」が無説明で混在
* 該当Fatal: F2（意味不整合が正本逸脱に波及する場合）
* 修正方針: 用語を正本語彙へ統一し、移行語彙は明示的な対応表で管理

### 重複文書

* 判定基準: 同一目的の文書が重複せず、参照導線が一意
* OK条件: 主目的ごとに正本が一意で、補助文書は役割が明確
* NG条件: 同じ手順/規約を複数文書が独立管理し、更新乖離が発生
* 不合格例: 同一チェックリストが別文書で別内容のまま併存
* 該当Fatal: F2（正本衝突がある場合）
* 修正方針: 正本を1つに統合し、重複文書は廃止または参照化する

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
