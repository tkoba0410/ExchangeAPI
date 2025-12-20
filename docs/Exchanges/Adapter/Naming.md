# Exchanges / Adapter API 命名規約

本ドキュメントは **ExchangeAPI における Adapter レイヤ専用の命名規約**を定義します。

対象は以下です。

- Adapter クライアント（またはサービス）クラス名
- Adapter のメソッド名
- Adapter DTO（共通 DTO / Interface への境界）

Adapter は Raw API を利用して **意味を共通語彙へ翻訳する層**です。
Raw の忠実性を壊さず、利用者が「複数取引所で同じ書き方ができる」ことを優先します。

> Adapter は translation。名前は Common の語彙に寄せる。

---

## 適用範囲

- 本規約は **Adapter レイヤにのみ適用**される
- Raw レイヤには適用しない（Raw は `Raw/Naming.md` を参照）
- Common の DTO / Interface の命名は `Common` 側の規約に従う

---

## 命名方針（最重要）

### 1. Adapter の公開 API は Common の語彙に揃える

- メソッド名・戻り値・引数の概念は **Common の DTO / Interface 名**を基準にする
- Raw の用語（取引所固有のリソース名・フィールド名）は、Adapter の公開 API へ持ち込まない

### 2. Adapter は「意味」を表し、「HTTP」や「エンドポイント」を表さない

- `GetV1...` / `Post...` / `CallEndpoint...` のような名前は禁止
- 取引所 API の階層（v1/v2/private/public）をメソッド名に含めない

### 3. 最小の統一、最小の驚き

- 複数取引所で同じ意味の操作は **同じ名前**にする
- 取引所差が大きい場合は、無理に揃えない（必要なら機能自体を提供しない）

---

## クラス命名規約

### Adapter クライアント

- 取引所ごとに 1 クラス（または用途別に少数）
- 命名形式：

```
<ExchangeName>Adapter
```

例：

- `BitflyerAdapter`
- `BittradeAdapter`

> Adapter が複数領域（Trading / MarketData など）に分割される場合は、
> `BitflyerTradingAdapter` のように **領域名を後置**する。

---

## メソッド命名規約

### 基本形

```
<Verb><CommonNoun>[ByCondition]Async
```

- `Async` サフィックスは必須
- Noun は **Common の概念名**

---

### Verb 一覧（推奨）

| Verb | 意味 | 例 |
|---|---|---|
| Get | 取得 | `GetBalancesAsync` |
| Place | 発注（共通語彙） | `PlaceOrderAsync` |
| Cancel | 取消 | `CancelOrderAsync` |
| GetOpen | 状態付き取得 | `GetOpenOrdersAsync` |

注意：

- Raw の `CreateOrderAsync` は Adapter では `PlaceOrderAsync` を推奨
  - 理由：Adapter は「注文という業務操作」を表すため

---

### Noun のルール（Common 準拠）

- `Balance(s)`, `Order(s)`, `OrderBook`, `Ticker`, `Execution(s)` など
- **Common の DTO / Interface の命名**を優先する

---

### ByCondition のルール

- Adapter は条件名も共通語彙に寄せる
- Raw の `ProductCode` / `Symbol` などは、Common の概念に統一する

例：

- `GetOrdersBySymbolAsync`（Symbol が共通概念の場合）
- `GetExecutionsByOrderIdAsync`

---

## 返り値・DTO の命名

### 原則

- Adapter の公開 API が返す型は **Common の DTO / Interface**を使用する
- Adapter 専用 DTO を新規作成するのは最小限

### Adapter 専用 DTO が必要な場合

- 命名形式：

```
<CommonNoun>AdapterOptions
<CommonNoun>AdapterResult
```

例：

- `OrderAdapterOptions`
- `OrderAdapterResult`

> 取引所固有フィールドを持つ DTO を Adapter に置くことは原則禁止。
> それが必要なら Raw API を使う。

---

## 禁止事項

- ❌ Raw のエンドポイント名をそのまま公開メソッド名にする
- ❌ 取引所固有語彙（例：product_code など）を公開 API に露出
- ❌ 取引所差の大きい機能を「同じ名前」に無理やり統一
- ❌ Adapter で便利関数を増殖させる（Application 側で行う）

---

## 命名の対応関係（例）

| 意味 | Raw API（例） | Adapter API（例） |
|---|---|---|
| 注文作成 | `CreateOrderAsync` | `PlaceOrderAsync` |
| 注文取消 | `CancelOrderAsync` | `CancelOrderAsync` |
| 残高取得 | `GetBalancesAsync` | `GetBalancesAsync` |

---

## まとめ

- Adapter の名前は **Common の語彙に揃える**
- Raw の名称・階層・癖を公開 API に持ち込まない
- 揃えられないものは無理に揃えない

> Minimal unification. Predictable names. Translation only.

