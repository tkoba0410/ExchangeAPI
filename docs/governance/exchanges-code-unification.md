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

ここで固定するのは **構成の箱のみ**であり、具体的なクラス構成や実装詳細はコードに委ねる。

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

## 6. エイリアス Endpoint の表現

* 取引所がエイリアス endpoint を持つ場合は、**別 EndpointId / 別メソッド**として定義する。
* EndpointId / Path を切り替えるための **分岐フラグ（例: useAliasPath）** を Wire に持ち込まない。
* エイリアスの有無は仕様差として許容し、**実装差は「別メソッド化」で吸収**する。

## 7. 論理責務について

Wire / Raw / Normalized の**責務・禁止事項の定義は TopSpec を正本**とする。
Adapter の意味定義も TopSpec に委譲し、本書では再定義しない。
本書では再定義しない。

## 8. 例外の扱い

* Exchanges 配下に閉じ込められない差異が発生した場合、
  本書に例外として追記しない。
* その差異は「差異として扱うべきか」「一般化すべきか」を別系統の設計判断として扱う。

## 9. 変更ルール（文書肥大の防止）

* 文書に追記してよいのは、**差異の配置ルールが変わる場合のみ**とする。
* 実装上の揺らぎは、文書ではなくコード（テンプレ／基底／helper）で解消する。
