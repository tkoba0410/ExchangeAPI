# REVIEW-RELIABILITY

本レビューは Reliability（信頼性）軸に基づく確認を行う。

重大度定義は PROJECT-FATAL-DEFINITION.md を参照する。
Fatal判定時は F番号を明示すること。

---

## 0. 対象

* PR番号:
* 変更概要:
* 対象層（Wire / Raw / Normalized / Contracts）:

---

## 1. 判定サマリ

| 観点          | 判定 | 重大度 (F番号明示) | CI化可否 | 備考 |
| ----------- | -- | ----------- | ----- | -- |
| 429処理       |    |             |       |    |
| timeout処理   |    |             |       |    |
| retry安全性    |    |             |       |    |
| idempotency |    |             |       |    |

---

## 2. 観点詳細

### retry安全性

* 判定基準: 再試行で重複実行の危険がない
* OK条件: 再試行条件が限定され、副作用操作で重複実行を防止できる
* NG条件: 副作用操作で無条件再送または重複防止がない
* 不合格例: POST再送で二重注文
* 該当Fatal: F5（Reliability重大欠陥）
* 修正方針: idempotencyキー導入または再試行条件の厳格化

---

## 3. CI自動化候補

* retryロジック単体テスト

---

## 4. 関連Normative / 判例

* docs/normative/contracts/resilience.md
* docs/normative/topspec.md
* docs/process/process.md（7.2）
* docs/process/reviews/templates/PROJECT-FATAL-DEFINITION.md

---

## 5. 最終結論

* OK / 要修正 / NG
