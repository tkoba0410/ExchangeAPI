# REVIEW-CONTRACTS

本レビューは Contracts（公開面 / 型境界 / 契約）軸に基づく確認を行う。

重大度定義は PROJECT-FATAL-DEFINITION.md を参照する。

---

## 0. 対象

* PR番号:
* 変更概要:
* public surface変更:

---

## 1. 判定サマリ

| 観点               | 判定 | 重大度 (F番号明示) | CI化可否 | 備考 |
| ---------------- | -- | ----------- | ----- | -- |
| public surface整合 |    |             |       |    |
| Try/OrThrow統一    |    |             |       |    |
| string流入禁止       |    |             |       |    |
| Exchange語彙混入     |    |             |       |    |

---

## 2. 観点詳細

### string流入禁止

* 判定基準: stringは入口以外で使用禁止
* NG例: 内部処理でstring伝播
* 該当Fatal: F3（公開契約破壊）

### Exchange語彙混入

* 判定基準: Contracts層に取引所固有語彙がない
* 該当Fatal: F3

---

## 3. CI自動化候補

* string境界検査

---

## 4. 最終結論

* OK / 要修正 / NG
