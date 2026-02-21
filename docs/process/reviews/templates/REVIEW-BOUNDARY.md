# REVIEW-BOUNDARY

本レビューは Boundary（層 / 依存 / 境界）軸に基づく確認を行う。

重大度定義は **PROJECT-FATAL-DEFINITION.md** を上位基準とする。
重大度は `Severity` と `FatalClass` の 2 軸で記録すること。
`Severity=Fatal` の場合は `FatalClass=F1〜F5` を明示すること。

---

## 0. 対象

* PR番号:
* 対象範囲:
* 対象層（Wire / Raw / Normalized / Adapter / Composition / Contracts）:
* 変更概要:

---

## 1. 判定サマリ

| 観点 | 判定 (OK/要修正/NG) | Severity (Fatal/High/Medium/Low/Nit) | FatalClass (F1-F5/None) | CI化可否 | 備考 |
| --- | --- | --- | --- | --- | --- |
| 層責務の混線 |  |  |  |  |  |
| 依存方向の逆流 |  |  |  |  |  |
| Core→Exchange依存 |  |  |  |  |  |
| 差異の閉じ込め |  |  |  |  |  |
| 情報塊依存の復活 |  |  |  |  |  |

---

## 2. 観点詳細

### 層責務の混線

* 判定基準: 各層が責務を越えていない
* OK条件: 隣接層のみ参照し、責務逸脱がない
* NG条件: 層ジャンプまたは責務越えの参照がある
* 不合格例: ContractsがWire型へ直接依存
* 該当Fatal: F1（境界破壊）
* 修正方針: 適切な層へ再配置

### 依存方向の逆流

* 判定基準: 依存は上位→下位のみ
* OK条件: 依存方向が TopSpec 定義と一致
* NG条件: 下位→上位の逆流依存がある
* 不合格例: NormalizedがComposition参照
* 該当Fatal: F1
* 修正方針: 逆流参照を除去し、中間層に再配置

### Core→Exchange依存

* 判定基準: Coreに取引所固有依存を持ち込まない
* OK条件: Exchange固有型/実装はAdapter以下に限定される
* NG条件: Coreが取引所固有名前空間・DTO・定数へ直接依存する
* 不合格例: Core層がExchange固有DTOを直接参照する
* 該当Fatal: F1（境界破壊）
* 修正方針: 依存点を抽象化し、固有依存を下位層へ押し戻す

### 差異の閉じ込め

* 判定基準: 取引所差異は境界で吸収され上位へ漏れない
* OK条件: 差異吸収がAdapter/Raw境界で完了し上位は共通契約のみ扱う
* NG条件: 取引所別分岐ロジックがNormalized/Contractsへ漏出する
* 不合格例: Contracts層で取引所名switchを行う
* 該当Fatal: F1（差異閉じ込め破壊）
* 修正方針: 差異処理を下位層へ移し、共通契約へ正規化する

### 情報塊依存の復活

* 判定基準: 横断的な巨大情報塊（God DTO / God Context）へ再依存しない
* OK条件: 必要最小限の契約型でデータ受け渡しが分離されている
* NG条件: 多責務の情報塊が複数層で共有され結合度を上げる
* 不合格例: 1つの巨大DTOをWire〜Contractsで使い回す
* 該当Fatal: F1（境界破壊）
* 修正方針: 情報塊を責務単位に分割し、層ごとの専用型へ分離する

---

## 3. CI自動化候補

* namespace依存検査
* Core→Exchanges参照検出

---

## 4. 関連Normative / 判例

* docs/normative/topspec.md
* docs/process/process.md（7.2）
* docs/process/reviews/templates/PROJECT-FATAL-DEFINITION.md

---

## 5. 最終結論

* OK / 要修正 / NG
