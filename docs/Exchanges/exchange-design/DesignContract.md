# Exchange Design Contract

本書は、ExchangeAPI における **取引所実装の最上位契約（Single Source of Truth）** である。
本書で定義された原則・責務は、すべての取引所実装・規約文書・テンプレートに優先する。

---

## 1. 設計ゴール

* 取引所ごとの差分を「仕様差」に閉じ込める
* 実装者が **どの層で何をしてよいか／してはいけないか** を迷わない
* 未知の仕様変更が **安全側（fail-fast）** に倒れる設計とする

---

## 2. レイヤモデル

Exchange 実装は、以下の層モデルを採用する。

```
[ Raw ] → [ Normalized / Wire ] → [ Adapter ] → [ Common ]
```

### 2.1 Raw（鏡像）

Raw 層は、**公式 API 仕様の鏡像**である。

* HTTP endpoint / request / response / JSON 形状を忠実に表現する
* 意味変換・正規化・推測を行ってはならない
* 「安全性」を最優先とする

#### Raw 層の原則

* **未知値は fail-fast（例外）**
* 公式仕様が string の値は string として保持する
* open set（例：symbol / product_code / currency / id 群）は enum 化しない

#### Raw enum ポリシー

* closed set（仕様上、追加が重大変更となる値）は strict enum を許可
* open set は enum 禁止（string または strong type）

---

### 2.2 Normalized / Wire

Normalized 層は、**Raw を束ね、利用しやすく配線する層**である。
本プロジェクトでは、この層を **Wire** と呼ぶ。

* Raw API を組み合わせて再利用性を高める
* 再試行、timestamp 取得、補助的な配線は許可される
* **意味変換・ドメイン解釈は行わない**

> Normalized と Wire は **概念的に同義**であり、実装単位としては Wire を採用する。

---

### 2.3 Adapter

Adapter 層は、Raw/Wire を **Common 契約に適合させる唯一の層**である。

* Common DTO への変換
* 値の検証、制約チェック
* 例外の正規化（ExchangeApiException など）

Adapter 以外の層で、ドメイン解釈を行ってはならない。

---

## 3. Symbol / 値解釈の基本方針

* 入力文字列の揺れ吸収（`BTCJPY`, `BTC_JPY`, `BTC/JPY` 等）は **entry-point のみ**
* Adapter 内では **ExchangeInfo に基づく strict な検証**を行う
* 推測による補正は禁止する

---

## 4. 本書の位置づけ

* 本書は **WHY / WHAT** を定義する
* 実装上の MUST / DO / DON’T は `STRUCTURE-RULES.md` に委譲する
* 衝突がある場合は **本書を最優先**とする
