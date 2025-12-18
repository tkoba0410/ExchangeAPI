# Bittrade Exchange

## 構造

```text
Bittrade/
├─ Raw/
└─ Adapter/
```

---

## Raw（主役）

### できること
- Bittrade の全 API をそのまま利用可能
- 公開 API は資格情報なしで利用可能

### 例

```csharp
var raw = BittradeFactory.CreateRaw();
var markets = await raw.GetMarketsAsync();
```

---

## Adapter（補助・注意あり）

### できること
- Common DTO / Interface を返す
- Raw API を内部で利用

### 必須条件
- `AccountId` の指定が必須
- 認証が必要な操作では資格情報が必須

### 例

```csharp
var api = BittradeFactory.CreateAdapter(
    new BittradeFactoryOptions {
        AccountId = "your-account-id",
        Credentials = new ApiCredentials("accessKey", "secretKey")
    });
```

---

## 使い分け

- Raw：Bittrade API を直接利用
- Adapter：共通処理を行いたい場合（前提条件に注意）

---

## 制限事項

- 複数取引所を束ねる API は提供しない
- 戦略ロジックは含まない

