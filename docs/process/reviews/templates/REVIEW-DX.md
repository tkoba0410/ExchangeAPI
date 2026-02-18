# REVIEW-DX

本レビューは DX（開発者体験 / 誤用耐性 / 診断可能性）軸に基づく確認を行う。

重大度定義は PROJECT-FATAL-DEFINITION.md を参照する。
Fatal判定時は F番号を明示すること。

---

## 0. 対象

* PR番号:
* 利用形態変化:
* エラー挙動変化:

---

## 1. 判定サマリ

| 観点        | 判定 | 重大度 (F番号明示) | CI化可否 | 備考 |
| --------- | -- | ----------- | ----- | -- |
| 自然な利用形態   |    |             |       |    |
| 誤用耐性      |    |             |       |    |
| 診断可能性     |    |             |       |    |
| secret非露出 |    |             |       |    |

---

## 2. 観点詳細

### secret非露出

* 判定基準: secretがログ/例外に出ない
* 該当Fatal: F4（Security重大違反）

---

## 3. CI自動化候補

* secret露出パターン検査（ログ/例外）
* 公開APIシグネチャ変更時の利用例コンパイル検査
* エラーコード/例外メッセージ整合の静的検査（導入可能範囲）

---

## 4. 関連Normative / 判例

* docs/normative/topspec.md
* docs/normative/contracts/contracts.md
* docs/process/process.md（7.2）
* docs/process/reviews/templates/PROJECT-FATAL-DEFINITION.md

---

## 5. 最終結論

* OK / 要修正 / NG
