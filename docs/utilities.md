# Utilities（便利機能レイヤ）

本書は、Contracts から分離された「便利機能」を置く独立レイヤの方針を定義する。
正本は `docs/topspec.md` と `docs/contracts/contracts.md` であり、本書は補助文書である。

---

## 1. 目的

- Contracts を **純粋なデータ定義（Shape / Semantics）** に限定する
- データの整形・変換・補助的操作を **独立レイヤに隔離** する
- 利用者がどの入口（Contracts / Adapter / Composition）から使っても到達できるようにする

---

## 2. 位置づけ

Utilities は **4層構造（Contract/Normalized/Raw/Wire）には含めない**。
あくまで「便利機能」を置くための **独立レイヤ** とする。

---

## 3. 依存関係

- Utilities → Contracts / Primitives への依存は許可
- Contracts / Primitives → Utilities への依存は **禁止**
- Application / Composition から Utilities を利用することは許可

---

## 4. 物理構成（基準）

```
src/Utilities/
  ExchangeApi.Utilities.csproj
  OrderBook/
    OrderBookNormalizer.cs
  Candlestick/
    CandlestickColumnar.cs
    CandlestickColumnarConverter.cs
  Trading/
    OrderRequestFactory.cs
```

---

## 5. 対象機能（例）

- OrderBook の並べ替え・同価格集約
- Candlestick の列指向変換（From / To）
- OrderRequest のファクトリ（Market / Limit）

---

## 6. 非責務

- API 呼び出し
- 取引所固有仕様の吸収（Normalized/Adapter の責務）
- Contracts の Shape / Semantics の拡張

