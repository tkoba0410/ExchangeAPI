# REVIEW-BOUNDARY

本レビューは Boundary（層 / 依存 / 境界）軸に基づく確認を行う。

重大度定義は **PROJECT-FATAL-DEFINITION.md** を上位基準とする。
Fatal 判定時は F1〜F5 の該当番号を明示すること。

---

## 0. 対象

* PR番号:
* 対象範囲:
* 対象層（Wire / Raw / Normalized / Adapter / Composition / Contracts）:
* 変更概要:

---

## 1. 判定サマリ

| 観点              | 判定 (OK/要修正/NG) | 重大度 (F番号明示) | CI化可否 | 備考 |
| --------------- | -------------- | ----------- | ----- | -- |
| 層責務の混線          |                |             |       |    |
| 依存方向の逆流         |                |             |       |    |
| Core→Exchange依存 |                |             |       |    |
| 差異の閉じ込め         |                |             |       |    |
| 情報塊依存の復活        |                |             |       |    |

---

## 2. 観点詳細

### 層責務の混線

* 判定基準: 各層が責務を越えていない
* NG例: ContractsがWire型へ直接依存
* 該当Fatal: F1（境界破壊）
* 修正方針: 適切な層へ再配置

### 依存方向の逆流

* 判定基準: 依存は上位→下位のみ
* NG例: NormalizedがComposition参照
* 該当Fatal: F1

---

## 3. CI自動化候補

* namespace依存検査
* Core→Exchanges参照検出

---

## 4. 最終結論

* OK / 要修正 / NG
