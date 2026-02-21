# REVIEW-RELIABILITY

本レビューは Reliability（信頼性）軸に基づく確認を行う。

重大度定義は `PROJECT-FATAL-DEFINITION.md` を参照する。
重大度は `Severity` と `FatalClass` の 2 軸で記録すること。
`Severity=Fatal` の場合は `FatalClass=F1〜F5` を明示すること。

---

## 0. 対象

* PR番号:
* 変更概要:
* 対象層（Wire / Raw / Normalized / Contracts）:

---

## 1. 判定サマリ

| 観点 | 判定 | Severity (Fatal/High/Medium/Low/Nit) | FatalClass (F1-F5/None) | CI化可否 | 備考 |
| --- | --- | --- | --- | --- | --- |
| 429処理 |  |  |  |  |  |
| timeout処理 |  |  |  |  |  |
| retry安全性 |  |  |  |  |  |
| idempotency |  |  |  |  |  |

---

## 2. 観点詳細

### 429処理

* 判定基準: レート制限応答に対して安全な制御がある
* OK条件: 429時の待機・再試行条件・上限が明示されている
* NG条件: 429を一般失敗として扱い、無制御に再送する
* 不合格例: 429受信後に即時無限再試行する
* 該当Fatal: F5（再試行暴走で障害を拡大する場合）
* 修正方針: backoff戦略と停止条件を導入し、429専用処理へ分離する

### timeout処理

* 判定基準: timeoutが分類され、再試行可否が制御されている
* OK条件: timeoutの計測・分類・再試行条件が仕様化されている
* NG条件: timeoutを不明エラーとして一括処理する
* 不合格例: timeout時に原因区別なく同一ハンドラへフォールスルー
* 該当Fatal: F5（復旧不能な失敗分類崩壊）
* 修正方針: timeoutを独立分類し、再試行可否を明示ルール化する

### retry安全性

* 判定基準: 再試行で重複実行の危険がない
* OK条件: 再試行条件が限定され、副作用操作で重複実行を防止できる
* NG条件: 副作用操作で無条件再送または重複防止がない
* 不合格例: POST再送で二重注文
* 該当Fatal: F5（Reliability重大欠陥）
* 修正方針: idempotencyキー導入または再試行条件の厳格化

### idempotency

* 判定基準: 副作用操作で同一要求の重複実行を防止できる
* OK条件: idempotencyキーまたは等価機構で重複抑止が実装される
* NG条件: 同一要求の再送で二重実行が起こり得る
* 不合格例: ネットワーク再送で同一注文が複数回成立する
* 該当Fatal: F5（idempotency不保証）
* 修正方針: 要求識別子を導入し、重複実行時の整合応答を定義する

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
