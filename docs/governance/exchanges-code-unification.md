# 注意

本書は、Exchange 配下のコード実装における統一方針を示す **補助文書（Governance）** である。
層構造・責務分離・公開範囲・API 契約に関する設計規範の正本は  
**TopSpec（docs/topspec.md）** とする。

本書は、TopSpec によって定義された設計方針を前提としており、
TopSpec と矛盾する解釈や運用を許容しない。

---

# 取引所間コード統一方針（Exchanges 配下限定）

## 0. 目的

本書は、取引所間の実装差異を減らし、**`src/Exchanges/<ExchangeName>/` 配下のコードに統一感（横方向の一貫性）**を与えるための最小規約である。

* 本書の対象は **`src/Exchanges/<Ex>/` 配下のみ**とする。
* 取引所横断実装（Core / Contracts / Primitives / Transport 等）は **本書の対象外**とし、ここでは決めない／触れない。
* 取引所固有仕様の正本は **各取引所の公式 API 文書**とする。

本書は「実装をどう書くか」を説明しない。
**どこに差異を閉じ込め、どう同形に収束させるか**だけを固定する。

## 1. 正本（Source of Truth）

* 取引所固有仕様：各取引所の公式 API 文書
* Exchanges 配下の実装統一：コード（共通テンプレ／基底／helper）
* 本書：Exchanges 配下における差異の配置ルールのみを定める

## 2. スコープの厳格化

* 本書で扱う差異は、**取引所間差異のみ**とする。
* 1 つしか存在しない実装（取引所横断実装）は、差異の対象にならないため本書のスコープ外とする。
* 取引所間差異は **必ず `src/Exchanges/<Ex>/` 配下に閉じ込める**。

## 3. Exchanges 配下の物理配置（Physical Layout）

`src/Exchanges/<Ex>/` は、次のサブ構成を必ず持つ。

* `Wire/` : 外部 API との I/O 境界
* `Raw/` : 外部 JSON 表現を lossless に型へ落とす層
* `Normalized/` : 取引所差を吸収し、意味を確定する層
* `Adapter/` : Normalized を公開契約へ写像する境界

（Adapter の意味論的定義は TopSpec を正本とし、本書では再定義しない。）

ここで固定するのは **構成の箱**と、差異が増殖しやすい箇所（Raw の配置・粒度）の **最小限の形**のみである。

### 3.1 Raw 配下の最小サブ構成

Raw は、取引所間差異の増殖を抑えるため、次のサブ構成を **必須**とする。

* `Raw/Public/` : 未認証 endpoint 群
* `Raw/Private/` : 認証が必要な endpoint 群
* `Raw/Internal/` : Raw の lossless 実装に必要な内部要素（encoding / query 整形 / JSON helper 等）

補足:

* `RawApi/` のような別名フォルダは使用しない。
* `Internal` は「Raw の責務（lossless / semantic-free）を維持するために必要な最小実装」に限定し、意味解釈は持ち込まない。

Raw の詳細な正規形（Canon）は TopSpec を正本とする（本書では再定義しない）。

## 4. 名前空間（Namespace）規約

* 物理配置と namespace は一致させる。
* 取引所固有 namespace は次の形式に統一する。

```
Exchanges.<Ex>.Wire.*
Exchanges.<Ex>.Raw.*
Exchanges.<Ex>.Normalized.*
Exchanges.<Ex>.Adapter.*
```

## 5. 差異の閉じ込めルール

取引所間差異は、必ず次のいずれかに閉じ込める。

1. `Exchanges.<Ex>.Wire`
2. `Exchanges.<Ex>.Raw`
3. `Exchanges.<Ex>.Normalized`
4. `Exchanges.<Ex>.Adapter`

* 差異を理由に、Exchanges 配下以外の構造や API を分岐させてはならない。
* 差異を一般化できた場合のみ、コード側で共通テンプレ／helper へ昇格させる。

## 6. Wire メソッド命名

* Wire の endpoint 生成メソッド名は **EndpointId と同一**にする。
* `Get` などの接頭辞は付けない（例外は設けない）。

## 7. エイリアス Endpoint の表現

エイリアス endpoint は「I/O 経路（Wire）」と「DTO 形状（Raw）」で意味が異なるため、層ごとに扱いを固定する。

### 7.1 Wire

* 取引所がエイリアス endpoint を持つ場合は、**別 EndpointId / 別メソッド**として定義する。
* EndpointId / Path を切り替えるための **分岐フラグ（例: useAliasPath）** を Wire に持ち込まない。

### 7.2 Raw

* Raw は endpoint の I/O 経路差を吸収しない（Wire の責務）。
* Raw の公開 API にエイリアス由来の **同義メソッドを増殖させない**。
  * 互換維持が必要な場合のみ、`[Obsolete]` を付けた **forwarding（委譲）** として残してよい。
  * forwarding は EndpointId の分岐・意味変更を含んではならない（lossless のまま Wire を呼ぶだけ）。

## 8. 論理責務について

Wire / Raw / Normalized の**責務・禁止事項の定義は TopSpec を正本**とする。
Adapter の意味定義も TopSpec に委譲し、本書では再定義しない。
本書では再定義しない。

本書で追記した Raw 配下サブ構成・alias 取り扱いは、
差異が増殖しやすい箇所に限って「配置ルール」を固定するための例外的最小規約である。

## 9. 例外の扱い

* Exchanges 配下に閉じ込められない差異が発生した場合、
  本書に例外として追記しない。
* その差異は「差異として扱うべきか」「一般化すべきか」を別系統の設計判断として扱う。

## 10. 変更ルール（文書肥大の防止）

* 文書に追記してよいのは、**差異の配置ルールが変わる場合のみ**とする。
* 実装上の揺らぎは、文書ではなくコード（テンプレ／基底／helper）で解消する。
