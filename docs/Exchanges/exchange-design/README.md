# exchange-design (Template Pack) v2

このパックは、Exchange 層（取引所層）を **使い勝手最優先**で実装するための「設計正本 + 実装テンプレ集」です。

v2 は、会話で提示した内容を **省略せず**にテンプレ化した “フル版” です。
（Factory/Composition、Capability、例外/Operation運用、テスト観点も収録）

## 収録物

- `DesignContract.md`：Exchange 層の Single Source of Truth（正本）
- `templates/`：実装テンプレ（読む順＝実装順）

## 読む順 / 実装順（推奨）

1. `DesignContract.md`
2. `templates/01-directory-structure.md`
3. `templates/02-exchange-client.md`
4. `templates/03-raw-api.md`
5. `templates/04-wire-api.md`
6. `templates/05-adapter-api.md`
7. `templates/06-factory-composition.md`
8. `templates/07-exceptions-operations.md`
9. `templates/08-tests-fast-suite.md`

## 基本方針（超要約）

- 普通の利用者：Common（`IExchangeClient` の `Trading/Account/MarketData/ExchangeInfo`）だけで完結
- 玄人/調査：`client.Raw<T>()`（公式鏡像）と `client.Wire<T>()`（正規化）で最短導線
- 実装は Raw-first：`Raw → Wire(Normalized) → Common(Adapter)` の順で昇格
