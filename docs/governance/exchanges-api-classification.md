# 注意

本書は、Exchange 配下における API 分類・整理を目的とした **補助文書（Governance）** である。
設計規範・層責務・公開範囲・Call 抽象などの正本は  
**TopSpec（docs/topspec.md）** とする。

本書の内容は、TopSpec に反しない範囲でのみ有効であり、
本書単体で設計判断を行ってはならない。

---

## Exchanges API 分類ルール（Wire / Raw / Normalized / Constants）【Normalized も分類しない】

### 0. 目的と位置付け

本書は `src/Exchanges/<Ex>/` 配下における **API の分類（グルーピング）規約**を定める。
ここでいう分類とは、**公開される API 面（インターフェース／メソッド群の分割単位）**を指す。

目的：

* 取引所追加・改修時の分類揺らぎを防止する
* Wire / Raw / Normalized 各層の責務を壊さずに API 面を固定する
* 署名有無・実装都合などが API 面へ漏れることを防ぐ

本書は Exchanges 配下にのみ適用されるガバナンス文書である。

---

### 1. 正本（Authoritative Sources）

* 取引所固有仕様の正本は各取引所の **公式 API 文書**（MUST）。
* 各層の責務・禁止事項は TopSpec（MUST）。
* 本書は「分類の揺らぎ防止」に限定した補助規約である。

---

### 2. 分類ポリシー（結論）

#### 2.1 Wire：分類しない（MUST）

* Wire 層の endpoint メソッドは **分類しない**（MUST）。
* endpoint メソッドは EndpointId と 1:1 でフラットに列挙する（MUST）。
* Public/Private（署名有無）で Wire API 面を分割してはならない（MUST NOT）。

理由：Wire は HTTP endpoint への入口であり、意味分類を持たない。

---

#### 2.2 Raw：分類しない（MUST）

* Raw 層の公開 API 面（`I<Ex>RawApi` / `<Ex>RawApi`）は **分類しない**（MUST）。
* Raw API は Wire endpoint に対応する呼び出しをフラットに列挙する（MUST）。
* Public/Private（署名有無）で Raw API 面を分割してはならない（MUST NOT）。

理由：Raw は lossless な表現写像であり、意味分類を持たない。

---

#### 2.3 Normalized：分類しない（MUST）

* Normalized 層の公開 API 面（`I<Ex>Normalized...Api` 等）は **分類しない**（MUST）。
* Normalized 公開 API は、EndpointId を基底としてフラットに列挙する（MUST）。
* MarketData/Trading/Account 等の分類を Normalized の **公開 API 面**として持ち込んではならない（MUST NOT）。

補足：

* Normalized では「意味の確定・統一」を行うが、分類（グルーピング）は公開 API 面に出さない。

---

#### 2.4 Constants：共通分類（MUST）

* Constants は **取引所横断で共通な分類語彙**で整理する（MUST）。
* 取引所固有のカテゴリ名（公式文書の章名など）を持ち込んではならない（MUST NOT）。

初期共通分類語彙（固定セット）：

* EndpointIds
* Paths
* QueryKeys
* Headers（必要な場合のみ）
* BodyKeys（必要な場合のみ）
* EndpointTraits（署名要否、レート制限グループ等）

分類語彙の追加は例外扱いとし、理由の明示を必須とする（MUST）。

補足（正本の明確化）：

* Constants の正本は **Wire/Constants を起点**とし、他層は参照に徹する（MUST）。

---

### 3. 署名有無（Public/Private）の扱い

* 署名有無は分類ではなく **endpoint の性質（trait）**として扱う（MUST）。
* Public/Private によって API 面（型・namespace・メソッド群）を分割してはならない（MUST）。

許可される表現：

* Constants の `EndpointTraits` で `RequiresAuth` を明示
* 実装上、署名が必要な場合のみ signer を利用（ただし API 面は分割しない）

禁止：

* `*/Public/*` `*/Private/*` を公開 API 面分割の根拠にすること（MUST NOT）

---

### 4. 実装上の分類（内部）は必要な範囲で許可（MAY）

本書が禁止するのは **公開 API 面の分類**である。
以下のような実装上の分類は、責務を侵さない限り許可する（MAY）。

* DTO を Public/Private で分ける
* 内部 helper を署名有無で分ける
* 内部実装をドメイン単位で分割する（例：MarketData/Trading…）

制約：これらは公開 API 面に現れてはならない（MUST）。

---

### 5. 変更ルール（揺らぎ防止）

* Wire / Raw / Normalized の公開 API 面に分類を追加する場合は例外として扱う（MUST）。
* Constants の共通分類語彙を追加する場合も例外扱いとし、理由を明示する（MUST）。
