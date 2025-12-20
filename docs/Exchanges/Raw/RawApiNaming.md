# Exchanges / Raw API 命名規則

本ドキュメントは **ExchangeAPI における Raw レイヤ専用の命名規則（確定版）** を定義します。

Raw API は、各取引所が提供する **公式 REST API の鏡像**として設計されます。
本規則は「公式 API との対応関係が一目で分かり、迷わず呼べる Raw API」を維持することを目的とします。

> Raw API は公式 API の鏡像である。意味を足さない。揃えない。

---

## 適用範囲

* 本規則は **Raw レイヤにのみ適用**される
* Adapter / Common / Application レイヤには適用しない
* Adapter 側の命名は `doc/Exchanges/Adapter/Naming.md` を参照する

---

## REST API を構成する要素と命名方針

Raw API の命名は、公式 REST API を構成する以下の要素を基準に決定する。

| REST 要素             | Raw での反映先        | 方針                   |
| ------------------- | ---------------- | -------------------- |
| Endpoint / Resource | Noun             | 公式 API の名詞を尊重する      |
| HTTP METHOD         | Verb             | METHOD ではなく「操作意味」を表す |
| Path                | ByCondition      | 識別子のみ名前に反映する         |
| Query               | 引数 / Request DTO | 原則名前に含めない            |
| Body                | Request DTO      | 必ず DTO として表現する       |

---

## クラス命名規則

### Raw API クライアント

* 取引所ごとに **1 クラス**を基本とする
* 命名形式：

```
<ExchangeName>RawApi
```

例：

* `BitflyerRawApi`
* `BittradeRawApi`

責務：

* 公式 API エンドポイントの呼び出し
* 認証・署名（Composition から注入された情報を使用）
* レスポンスの最小限な型付け（意味加工は行わない）

---

## メソッド命名規則

### 基本形

```
<Verb><Noun>[ByCondition]Async
```

* Raw API は **すべて非同期**
* `Async` サフィックスは必須

---

### Verb 一覧（固定）

> ※ HTTP METHOD は **参考情報**。Verb は常に **公式 API が表す操作意味**で決定する。

| Verb   | 主な HTTP METHOD（参考） | 意味         | 使用例            |
| ------ | ------------------ | ---------- | -------------- |
| Get    | GET                | 取得         | 残高、板、注文一覧      |
| Create | POST               | 作成         | 注文作成、入金アドレス生成  |
| Update | PUT / PATCH        | 更新         | 注文修正（存在する場合のみ） |
| Cancel | POST / DELETE      | 取消（状態遷移）   | 注文キャンセル        |
| Delete | DELETE             | 削除（リソース消滅） | APIキー削除 等      |

#### 補足ルール（重要）

* Raw の Verb は **HTTP METHOD そのもの**ではなく、**公式 API が表す操作意味**を表現する
* 多くの場合、HTTP METHOD と Verb は一致するが、**Cancel / Delete は HTTP METHOD から機械的に決めてはならない**

#### `POST` / `DELETE` 系の扱い

* `POST` であっても、公式 API の意味が「取消」の場合は **Cancel** を用いる

  * 例：`POST /cancelchildorder` → `CancelChildOrderAsync`
* `DELETE` であっても、公式 API の意味が「状態遷移」の場合は **Cancel** を用いる
* リソースが概念的に消滅する場合のみ **Delete** を用いる

> 判断基準：**状態が変わるだけ → Cancel / 以後取得不能 → Delete**

---

## Noun の命名規則（Endpoint / Resource）

* 公式 API ドキュメントに登場する **名詞をそのまま使用**する
* 単数 / 複数は **レスポンス形**に合わせる
* C# の慣習に従い PascalCase とする

例：

* `Balance` / `Balances`
* `Order` / `Orders`
* `OrderBook`
* `Ticker`
* `Executions`

---

## ByCondition の命名規則（Path 由来）

* `ByCondition` は **Path に現れる識別子**にのみ使用する
* 識別子は公式 API が想定するキーを尊重する

例：

* `GET /orders/{orderId}` → `GetOrderByOrderIdAsync`
* `GET /orders/{orderId}/executions` → `GetExecutionsByOrderIdAsync`

---

## Query パラメータの扱い

### 原則

* Query パラメータは **メソッド名に含めない**
* すべて引数または Request DTO として受け取る

### 例外（最小）

* 公式 API において **必須であり、識別子として機能する Query** は、Path 識別子と同様に `ByXxx` としてメソッド名に含めることを許可する

例：

* `GET /ticker?product_code=BTC_JPY`（必須） → `GetTickerByProductCodeAsync`

---

## Request DTO の導入ルール（Query 集約）

Query パラメータは以下の条件を満たす場合、Request DTO にまとめる。

### 導入閾値（確定）

* **Query が 3 個以上**ある場合
* **Query のカテゴリが 2 種以上**にまたがる場合

#### Query カテゴリ例

* Filter（状態・種別・フラグ）
* Range（期間・ID 範囲）
* Paging（page / limit / cursor）
* Sort（並び順）

### 命名

```
Get<Noun>Request
```

例：

* `GetOrdersRequest`
* `GetExecutionsRequest`

---

## Dictionary（escape hatch）の位置づけ

* 公式 API が **未定義または拡張用途**として任意 Query を許容している場合、escape hatch として `Dictionary<string, string>` 等を受け取る API を併設してもよい
* Dictionary は **原則ではなく例外手段**とする

---

## Request / Response DTO 命名規則

### Request DTO（Body / Query）

```
<Verb><Noun>Request
```

例：

* `CreateOrderRequest`
* `UpdateOrderRequest`
* `GetOrdersRequest`

### Response DTO

```
<Noun>Response
```

* 配列レスポンスであっても、Response は **1 エンティティ単位**の命名とする

例：

* `OrderResponse`
* `BalanceResponse`

---

## やってはいけないこと

* ❌ 意味の統合（全取引所共通操作の追加）
* ❌ Adapter 由来の語彙を Raw に持ち込む
* ❌ 公式 API に存在しない convenience API の追加
* ❌ Query 条件の組み合わせをメソッド名で表現する

---

## まとめ

* Raw API の命名は **公式 API への忠実性が最優先**
* REST の構造（Endpoint / Path / Query / Body）を役割ごとに分離して表現する
* 揃える・翻訳する責務は Adapter に委ねる

> Raw first. Faithful mapping. No abstraction.
