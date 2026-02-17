# REVIEW-RELIABILITY

本レビューは Reliability（信頼性）軸に基づく確認を行う。

重大度定義は PROJECT-FATAL-DEFINITION.md を参照する。

---

## 0. 対象

* PR番号:
* 変更概要:

---

## 1. 判定サマリ

| 観点          | 判定 | 重大度 (F番号明示) | 備考 |
| ----------- | -- | ----------- | -- |
| 429処理       |    |             |    |
| timeout処理   |    |             |    |
| retry安全性    |    |             |    |
| idempotency |    |             |    |

---

## 2. 観点詳細

### retry安全性

* 判定基準: 再試行で重複実行の危険がない
* NG例: POST再送で二重注文
* 該当Fatal: F5（Reliability重大欠陥）

---

## 3. CI自動化候補

* retryロジック単体テスト

---

## 4. 最終結論

* OK / 要修正 / NG
