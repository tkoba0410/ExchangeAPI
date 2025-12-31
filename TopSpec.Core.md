# TopSpec.Core.md

> **本書は TopSpec の憲法（Core）である。**  
> **本書は常に全文を前提として解釈される。部分読解を禁止する。**  
> **省略・要約・再解釈・暗黙補完を一切許可しない。**  
> **補助規約（Ops）と矛盾する場合、本書を常に優先する。**

---

## 1. 決めること / 決めないこと
- 本書は **境界・責務・依存方向・不変条件**のみを決定する。
- 物理構成・運用・具体例は決定しない（Opsに委譲）。

## 2. 大原則
- 公開境界は明示される。
- 層を跨ぐ責務混在は禁止する。
- 不変条件は破壊的変更扱いとする。

## 3. FIX 宣言
- 本書に定義される規則は **FIX** である。
- FIX 違反は設計エラーとみなす。

## 4. ゴール / 非ゴール
**ゴール**
- 取引所差異の Domain 隔離
- 公開 API 意味論の安定

**非ゴール**
- 公式API仕様の完全写像

## 5. 論理階層（定義）
- **Wire**: transport 仕様（spec）
- **Raw**: API 鏡像 DTO（spec）
- **Normalized**: 正規化 DTO（spec）
- **Adapter**: spec → domain 翻訳境界
- **Contracts**: 公開契約（domain 入口）
- **Domain**: 横断的ふるまい
- **Composition**: 供給・組立

**規範**
- Raw / Normalized は spec 層である。
- Adapter 以降は domain 側である。
- 層越境は禁止する。

## 6. 公開入口（Factory）
- 公開入口は Factory に限定する。
- Factory は利用意図を一意に表現する。

## 7. 公開契約（Contracts）
- 公開 API は Call → Outcome の関係を持つ。
- Outcome は Success / Failure の排他構造である。

## 8. Contracts / Common 境界
- Contracts は **形** を定義する。
- Common は **語彙** を定義する。

## 11. 共通化対象
- 型 / 値 / ふるまい / 契約

## 12. 責務分離
- spec は domain を知らない。
- domain は spec に依存しない。
- 翻訳責務は Adapter にのみ存在する。

## 13. 横断関心
- 横断関心は Domain に属する。

## 14. Raw / Exchange 明示アクセス
- Raw / Exchange への直接アクセスは **opt-in** とする。

## 15. 依存方向
- spec → domain 依存は禁止
- domain → spec 依存は禁止
- 許可される依存:
  - Composition → Domain / Adapter
  - Adapter → spec
  - Domain → Common

## 16. 不変条件
- 公開契約の意味論は後方互換でなければならない。
- Success / Failure の区別は破壊してはならない。

## 18. 禁止事項
- 層跨ぎ DTO の再利用
- 暗黙変換
- domain 内への transport 情報流入

## 19. 一文要約
**spec と domain を分離し、公開契約を不変に保つ。**

