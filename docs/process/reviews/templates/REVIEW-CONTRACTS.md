# REVIEW-CONTRACTS

本レビューは Contracts（公開面 / 型境界 / 契約）軸に基づく確認を行う。

重大度定義は PROJECT-FATAL-DEFINITION.md を参照する。
Fatal判定時は F番号を明示すること。

---

## 0. 対象

* PR番号:
* 変更概要:
* public surface変更:
* 対象層（Contracts / Adapter / Composition）:

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

### public surface整合

* 判定基準: 公開面が契約正本と一致し、過不足なく維持される
* OK条件: 公開APIの追加/削除/変更が契約文書と一致している
* NG条件: 実装公開面と契約文書に差分がある
* 不合格例: 実装に公開メソッド追加があるが契約文書未更新
* 該当Fatal: F3（公開契約破壊）
* 修正方針: 契約正本を先に更新し、実装公開面を追従させる

### Try/OrThrow統一

* 判定基準: エラーハンドリング契約が Try/OrThrow 規約で統一される
* OK条件: 同種APIで戻り契約が統一され利用者に揺らぎがない
* NG条件: 同一責務でTry系と例外直投げが混在する
* 不合格例: 同じカテゴリのAPIで一部のみOrThrow相当が欠落
* 該当Fatal: F3（契約一貫性破壊）
* 修正方針: 返却契約を統一し、非準拠APIを段階的に移行する

### string流入禁止

* 判定基準: stringは入口以外で使用禁止
* OK条件: stringは入口でValueObject化され、内部伝播しない
* NG条件: Contracts境界を越えてstringが流入・伝播している
* 不合格例: 内部処理でstring伝播
* 該当Fatal: F3（公開契約破壊）
* 修正方針: 境界で型化し、DTO/VO契約に寄せる

### Exchange語彙混入

* 判定基準: Contracts層に取引所固有語彙がない
* OK条件: 取引所固有語彙はAdapter以下に閉じ込められている
* NG条件: Contracts公開型に取引所固有語彙が露出している
* 不合格例: Contracts DTOに取引所固有識別子が直接含まれる
* 該当Fatal: F3
* 修正方針: 抽象語彙へ置換し、変換責務を下位層へ移動

---

## 3. CI自動化候補

* string境界検査

---

## 4. 関連Normative / 判例

* docs/normative/contracts/contracts.md
* docs/normative/topspec.md
* docs/process/process.md（7.2）
* docs/process/reviews/templates/PROJECT-FATAL-DEFINITION.md

---

## 5. 最終結論

* OK / 要修正 / NG
