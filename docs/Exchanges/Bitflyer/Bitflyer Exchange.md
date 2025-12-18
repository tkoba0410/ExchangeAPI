# Bitflyer Exchange

## 構造

```text
Bitflyer/
├─ Raw/
└─ Adapter/
```

---

## Raw（主役）

### できること
- Bitflyer の全 API をそのまま利用可能
- 取引所固有モデル・Enum を使用
- 公開 API は資格情報なしで利用可能

### 例

```csharp
var raw = BitflyerFactory.CreateRaw();
var ticker = await raw.GetTickerAsync("BTC_JPY");
```

---

## Adapter（補助）

### できること
- Common DTO / Interface を返す
- Raw API を内部で利用

### 注意
- Adapter は最小共通化のみ
- Raw の機能を隠さない

### 例

```csharp
var api = BitflyerFactory.CreateAdapter();
var balances = await api.GetBalancesAsync();
```

---

## 使い分け

- Raw：Bitflyer 固有機能・完全制御
- Adapter：共通処理を簡潔に書きたい場合

---

## 制限事項

- 他取引所との統合 API は提供しない
- 戦略ロジックは含まない

