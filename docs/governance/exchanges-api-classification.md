# Exchanges API 分類ルール（Wire / Raw / Normalized / Constants）【再検証版】

## 0. 目的と位置付け

本書は `src/Exchanges/<Ex>/` 配下における **API の分類（グルーピング）規約**を定める。
ここでいう分類とは、**公開される API 面（インターフェース／メソッド群）の分割単位**を指す。

目的は以下である。

* 取引所追加・改修時の分類揺らぎを防止する
* Wire / Raw / Normalized 各層の責務を明確化する
* 実装上の都合（署名有無など）が API 面へ漏れることを防ぐ

本書は **Exchanges 配下にのみ適用されるガバナンス文書**であり、
TopSpec（層責務）および公式 API 文書を前提として補完的に機能する。

---

## 1. 正本（Authoritative Sources）

* 取引所固有仕様の正本は各取引所の **公式 API 文書**とする（MUST）。
* 各層の責務・禁止事項は TopSpec を正本とする（MUST）。
* 本書は「分類の揺らぎ防止」に限定した補助規約である。

---

## 2. 分類ポリシー（結論）

### 2.1 Wire 層：分類しない（MUST）

* Wire 層の endpoint メソッドは **分類しない**（MUST）。
* endpoint メソッドは EndpointId と 1:1 でフラットに列挙する（MUST）。
* Public / Private（署名有無）によって Wire API を分割してはならない（MUST NOT）。

理由：

* Wire は HTTP endpoint への直接的な入口であり、分類は意味を持たない。
* 分類を導入すると、取引所間差異や将来変更が API 面へ漏れやすくなる。

---

### 2.2 Raw 層：分類しない（MUST）

* Raw 層の公開 API（`I<Ex>RawApi` / `<Ex>RawApi`）は **分類しない**（MUST）。
* Raw API は Wire endpoint に対応する呼び出しをフラットに列挙する（MUST）。
* Public / Private（署名有無）によって Raw API 面を分割してはならない（MUST NOT）。

理由：

* Raw の責務は lossless な表現写像であり、意味分類を持たない。
* 署名有無は「意味」ではなく「実装条件」である。

---

### 2.3 Normalized 層：取引所固有分類（MAY / 推奨）

* Normalized 層は、取引所ごとのドメイン理解に基づく分類を行ってよい（MAY）。

  * 例：MarketData / Trading / Account / ExchangeInfo など
* ただし、その分類名・粒度を Wire / Raw / Contracts へ逆流させてはならない（MUST NOT）。

制約：

* Normalized の分類は **Normalized 層内部に閉じる**。
* 他層の API 面や共通 DTO に影響を与えてはならない。

---

### 2.4 Constants 層：共通分類（MUST）

* Constants は **取引所横断で共通な分類語彙**で整理する（MUST）。
* 取引所固有のカテゴリ名（公式文書の章名など）を持ち込んではならない（MUST NOT）。

初期共通分類語彙（固定セット）：

* EndpointIds
* Paths
* QueryKeys
* Headers（必要な場合のみ）
* BodyKeys（必要な場合のみ）
* EndpointTraits（署名要否、レート制限グループ等）

ルール：

* 上記分類語彙の追加は例外扱いとし、理由の明示を必須とする（MUST）。

---

## 3. 署名有無（Public / Private）の扱い

* 署名有無は「分類」ではなく **endpoint の性質（trait）**として扱う（MUST）。
* Public / Private によって API 面（型・namespace・メソッド群）を分割してはならない（MUST）。

許可される表現：

* Constants の `EndpointTraits` 等で `RequiresAuth` を明示する
* 実装上、署名が必要な場合のみ signer を注入する

禁止される表現：

* `Wire/Public/*` / `Wire/Private/*` のような API 面分割（MUST NOT）
* Raw 公開 API を Public / Private に分割すること（MUST NOT）

---

## 4. 実装上の分類の扱い（MAY）

本書が禁止するのは **公開 API 面の分類**である。
以下のような実装上の分類は、責務を侵さない限り許可される（MAY）。

例：

* DTO を Public / Private で分ける
* 内部 helper や HTTP 処理を署名有無で分ける
* 認証・署名ロジックを別コンポーネントとして切り出す

制約：

* これらの分類は公開 API 面に現れてはならない（MUST）。

---

## 5. 変更ルール（揺らぎ防止）

* Wire / Raw の API 面に分類を追加する場合は例外として扱う（MUST）。
* Constants の共通分類語彙を追加する場合も例外扱いとし、理由を明示する（MUST）。
* Normalized の分類追加は取引所固有判断として許容するが、外部へ漏らしてはならない（MUST）。

---

## 6. 本書の位置付けまとめ

* Wire / Raw：**列挙体としての API 面**
* Normalized：**取引所理解を反映した API 面**
* Constants：**取引所横断の語彙・分類の正本**

本書は、分類に関する設計判断を将来に固定するための最小規約である。
