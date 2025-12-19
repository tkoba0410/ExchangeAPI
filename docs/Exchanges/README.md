# Adapter レイヤーについて

このドキュメントは、`Exchanges/*/Adapter` レイヤーの役割と、
**Raw API との違い**、および **どのような場面で Adapter を使うべきか**を整理したものです。

本ライブラリの基本方針は **「Raw 主役」** です。Adapter は補助的な翻訳層として提供されます。

---

## Adapter とは何か

Adapter は、各取引所が提供する **Raw API（取引所固有仕様）** を、
`Common` レイヤーで定義された **共通語彙（DTO / Enum / Interface）** に変換する層です。

主な責務は以下です：

- Raw API のレスポンスを Common DTO に変換する
- 取引所固有のエラー形式を共通のエラー分類にマッピングする
- Raw API を `ITradingApi` / `IMarketDataApi` などの共通 Interface として提供する

Adapter は **Raw API を隠蔽しません**。Raw は常に直接利用可能です。

---

## Raw API との違い

| 観点 | Raw API | Adapter |
|---|---|---|
| 抽象度 | 低い（取引所固有） | 中（取引所共通語彙） |
| DTO | 取引所固有モデル | `Common.Dtos` |
| エラー | 取引所固有 | 共通 ErrorCategory |
| 拡張性 | 最大 | 一部制約あり |
| 主用途 | 完全制御・最適化 | 共通処理・横断ロジック |

Raw API は **自由度・完全性を最優先**します。
Adapter は **可搬性・共通処理のしやすさ**を優先します。

---

## Adapter を使うべき場面

以下のようなケースでは Adapter の利用が適しています。

- 複数取引所で **同じ処理ロジック** を書きたい
- 資産一覧、板情報、注文といった **基本的な操作** を共通化したい
- 取引所ごとの差分を **最小限に抑えたい**
- エラー分類やリトライ判断を **共通ロジックで扱いたい**

例：
```csharp
ITradingApi trading = bitflyerAdapter;
await trading.PlaceOrderAsync(orderRequest);
```

---

## Adapter を使わないほうがよい場面

以下の場合は Raw API の直接利用を推奨します。

- 取引所固有の機能・特殊注文を使う場合
- レイテンシ・通信回数・細かい挙動を厳密に制御したい場合
- API 仕様変更に即応したい場合

例：
```csharp
var raw = BitflyerFactory.CreateRaw();
await raw.PrivateApi.SendChildOrderAsync(...);
```

---

## 設計上の方針（重要）

- Adapter は **最小限の抽象化** に留める
- Adapter に「万能 API」や「統合クライアント」は作らない
- Raw API を使う自由を奪わない
- Adapter は **あくまで翻訳層（ラッパー）**

この方針により、ライブラリ全体の設計は

> Raw を主役とし、共通化したい部分だけを Adapter に委ねる

という明確な構造を保っています。

---

## まとめ

- **迷ったら Raw を使う**
- 共通化・横断処理が必要になったら Adapter を使う
- Adapter は便利だが、主役ではない

この思想が、本ライブラリ全体の一貫した設計指針です。

