# A040-STG3-ARCB bitFlyer Private API → 抽象層マッピング（send child order）

## 1. 本文書の目的
Stage3 では、Private POST API の最初の対象として **`/v1/me/sendchildorder`** を扱い、
これを抽象インターフェース（`IExchangeTradingClient.SendOrderAsync`）までマッピングする。

本ドキュメントでは、bitFlyer のリクエスト形式・レスポンス形式と、抽象ドメイン（OrderRequest / OrderResult）との
**対応表・値変換仕様・例外扱い** を定義する。

Stage4 以降の注文系 API（LIMIT、STOP、キャンセルなど）のテンプレートとなる基礎仕様でもある。

---

## 2. API 定義（bitFlyer Private POST）

### 2.1 エンドポイント
- **HTTP Method**: POST
- **Path**: `/v1/me/sendchildorder`
- **Auth**: 必須（API Key / API Secret / 署名）
- **Content-Type**: `application/json`
- **Query Parameters**: なし（すべて body で渡す）

### 2.2 リクエスト（bitFlyer 固有の JSON 形式）
例：MARKET BUY 注文
```json
{
  "product_code": "BTC_JPY",
  "child_order_type": "MARKET",
  "side": "BUY",
  "size": 0.01
}
```

### 2.3 レスポンス（bitFlyer 固有形式）
```json
{
  "child_order_acceptance_id": "JRF20201203-133344-058042"
}
```

この ID が、抽象層（OrderResult.OrderId）へ対応する。

---

## 3. DTO（Private 層で扱う型）

### 3.1 BitflyerSendChildOrderRequest
```csharp
public sealed class BitflyerSendChildOrderRequest
{
    public string ProductCode { get; init; } = string.Empty;

    public string ChildOrderType { get; init; } = string.Empty; // "MARKET"

    public string Side { get; init; } = string.Empty; // "BUY" or "SELL"

    public decimal Size { get; init; }
}
```

> Stage3 時点では、`time_in_force` や `minute_to_expire` は取り扱わない。

### 3.2 BitflyerSendChildOrderResponse
```csharp
public sealed class BitflyerSendChildOrderResponse
{
    public string ChildOrderAcceptanceId { get; init; } = string.Empty;
}
```

---

## 4. 抽象層ドメイン（Abstractions）

### 4.1 OrderRequest
```csharp
public sealed record OrderRequest(
    string ProductCode,
    OrderSide Side,
    OrderType OrderType,
    decimal Size
);
```
> Stage3 では OrderType は Market のみ。

### 4.2 OrderResult
```csharp
public sealed record OrderResult(
    string OrderId
);
```

### 4.3 抽象インターフェース
```csharp
Task<OrderResult> SendOrderAsync(OrderRequest request, CancellationToken ct = default);
```

---

## 5. マッピング仕様（Private → Domain, Domain → Private）

### 5.1 Domain → Private（OrderRequest → BitflyerSendChildOrderRequest）

| Domain フィールド | Private フィールド | 変換仕様 | 注意点 |
|-------------------|---------------------|----------|--------|
| `ProductCode` | `product_code` | 文字列をそのまま渡す | bitFlyer では `BTC_JPY` 等の形式 |
| `Side` | `side` | `Buy` → `"BUY"`, `Sell` → `"SELL"` | 大文字固定 |
| `OrderType` | `child_order_type` | Market → `"MARKET"` | Stage3 は MARKET のみ |
| `Size` | `size` | そのままマップ | decimal 精度注意（.NET → JSON 固有問題なし） |

> 将来的に OrderType が増えた場合は変換テーブルを追加する。

### Domain → DTO 変換例
```csharp
var dto = new BitflyerSendChildOrderRequest
{
    ProductCode = request.ProductCode,
    Side = request.Side == OrderSide.Buy ? "BUY" : "SELL",
    ChildOrderType = "MARKET",
    Size = request.Size,
};
```

---

### 5.2 Private → Domain（BitflyerSendChildOrderResponse → OrderResult）

| Private フィールド | Domain フィールド | 変換仕様 |
|--------------------|-------------------|-----------|
| `child_order_acceptance_id` | `OrderId` | 文字列をそのまま渡す |

変換例：
```csharp
return new OrderResult(
    response.ChildOrderAcceptanceId
);
```

---

## 6. IExchangeTradingClient 実装における流れ

```csharp
public async Task<OrderResult> SendOrderAsync(OrderRequest request, CancellationToken ct)
{
    var dto = new BitflyerSendChildOrderRequest
    {
        ProductCode = request.ProductCode,
        Side = request.Side == OrderSide.Buy ? "BUY" : "SELL",
        ChildOrderType = "MARKET",
        Size = request.Size
    };

    var result = await _tradingApi.SendChildOrderAsync(dto, ct);

    return new OrderResult(result.ChildOrderAcceptanceId);
}
```

### フロー図
```
BitflyerExchangeClient
   ↓
IBitflyerPrivateTradingApi.SendChildOrderAsync
   ↓
RestClient.PostAsync("/v1/me/sendchildorder", body)
   ↓
HTTP POST + 署名
   ↓
JSON → BitflyerSendChildOrderResponse
   ↓
OrderResult（ドメインモデル）
```

---

## 7. 署名および body 取り扱いに関する注意点
- POST の署名対象は必ず **body（JSON 文字列）を含む**。
  - `timestamp + method + path + bodyJson`
- JSON のプロパティ順は .NET のシリアライザに依存するが、bitFlyer は順序非依存のため問題なし。
- `Content-Type: application/json` は signer または RestClient が付与する。
- body が null の API は Stage3 では存在しないが、将来の `cancelchildorder` では query + POST の混合形式が出る。

---

## 8. 例外扱い（E1 レベル）
Stage3 では Stage2 と同様に **HTTP ステータスベースの最小限の例外（E1）** とする。

| 種別 | 例外化 | 備考 |
|------|--------|------|
| HTTP 400 / 404 / 500 系 | `ExchangeApiException` | 入力不正 / サーバエラー |
| 認証エラー（403） | `ExchangeApiException` | API key / secret が無効 |
| タイムアウト | `ExchangeApiException` | HttpClient の例外をラップ |

bitFlyer 固有のエラーコードの解釈（例：`INVALID_ORDER`）は **Stage4 以降** に行う。

---

## 9. 今後の拡張に向けた検討
- `OrderType` に LIMIT / STOP / IFDOCO 等を追加する場合、マッピング表を拡張する。
- `time_in_force`（IOC / FOK）および `minute_to_expire` のサポートは Stage4 以降で導入。
- 署名アルゴリズムは GET / POST 共通に保ち、リクエスト生成ロジックを統一。
- `cancelchildorder` / `cancelallchildorders` は query と body の混合（bitFlyer 仕様）なので、本マッピング仕様をテンプレートとして流用できる。

---

## 10. Stage3 完了条件（マッピング観点）
- Private → Domain / Domain → Private の対応が本書の通り実装されている。
- DTO → Domain 変換が正しく行われている。
- `/v1/me/sendchildorder` を通して実データ取得（child_order_acceptance_id）できる。
- Stage3 A010 / A020 / A030 との整合性が取れている。

