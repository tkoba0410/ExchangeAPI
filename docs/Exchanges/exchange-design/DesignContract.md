# DesignContract：Exchange層 正本（再検証反映版）

> 本文書は **Exchange 層の Single Source of Truth** である。  
> 実装・レビュー・将来拡張は、必ず本契約に準拠する。

---

## 0. 設計原則（優先順位）

1. **使い勝手（最優先）**  
   - 普通の利用者は迷わず・短く書けること
2. **論理性**  
   - レイヤ境界が型と責務で守られること
3. **合理性**  
   - Raw-first・拡張容易・保守可能であること

---

## 1. 想定ユーザーと利用導線

### 1.1 普通の利用者（最優先）
- 取引所差分を意識せず、Common API のみで完結したい

```csharp
await client.MarketData.GetTickerAsync(symbol);
await client.Trading.PlaceOrderAsync(req);
```

### 1.2 玄人 / 調査用途
- 公式 API の挙動を把握したい
- Common に未昇格の機能を使いたい

```csharp
var raw  = client.Raw<IBitflyerRawApi>();
var wire = client.Wire<IBitflyerWireApi>();
```

---

## 2. データ変換モデル（Exchange層の正本）

Exchange 層では、データの形を次の **3 段階**で扱う。

### 2.1 Raw（JSON構造化後 / 公式API鏡像）

- JSON を DTO にデシリアライズした **公式 API の鏡像**を扱う。
- Raw は取引所仕様の正本であり、**意味変換・正規化・共通化は行わない**。

#### Raw DTO の型制約（重要）

Raw DTO は、公式レスポンスの型を忠実に保持しなければならない。

- 許可される型：
  - `string`, `number`, `bool`, `array`, `object` に対応する DTO
- 禁止される型：
  - `Price`, `Size` 等のドメイン型
  - 意味変換を伴う enum（Domain enum）

#### Raw 層で **行ってはならないこと**

- status / result に基づく成功・失敗の判定
- 数値・日時の意味変換（string → decimal / DateTime 等）
- フィールドの省略・再構成
- 取引所仕様に存在しない補助情報の付与

---

### 2.2 Normalized（正規化 / 取引所内実用形）

- Raw を基に、取引所内での実用性のために **軽い正規化**を行う。
- この層は **依然として取引所固有**であり、Common ではない。

#### 正規化の例

- status != ok の例外化
- error_code / message の抽出
- 数値文字列の Try-style parsing（文脈付き例外）

#### Normalized 層で **行ってはならないこと**

- 他取引所との意味的整合を取る処理
- 取引戦略・業務ルールの組み込み
- Common DTO に近い型への変換

---

### 2.3 Common（抽象化）

- 複数取引所に共通な DTO / API に抽象化する層。
- 利用者の **第一導線**。
- Raw / Normalized を直接露出してはならない。

---

（参考）Wire（生テキスト）および JSON デシリアライズは Core（Transport / Protocol）の責務であり、本契約の範囲外とする。

---

## 3. Raw-first 実装方針

- すべての取引所は将来的に Raw API を完備する
- bitflyer / bittrade / coincheck は最初から Raw を正本として実装してよい
- 機能追加は原則、次の順で昇格させる

```text
Raw → Normalized → Common
```

---

## 4. Done の定義（設計遵守）

- 普通の利用者は Common API だけで迷わず使える
- 玄人は Raw / Normalized を最短導線で使える
- Raw / Normalized / Common の境界が型と契約で守られている
- 将来、全取引所 Raw 化しても設計を壊さず拡張できる
