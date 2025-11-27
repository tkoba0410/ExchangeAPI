# A050-STG3-IMPL Stage3 実装ノート（send child order）

## 1. 本文書の目的
Stage3 では、bitFlyer Private POST API **`/v1/me/sendchildorder`** を抽象層まで通し、
最小限の MARKET 注文を end-to-end で発注できる縦スライスを実装する。

本ドキュメントは、Stage3 の「実装時に考慮すべきポイント」「レイヤごとの実装指針」「注意点」
などを整理した技術ノートであり、Stage4 以降の Trading API 拡張のテンプレートとなる。

---

## 2. 実装方針の全体像
Stage2（GET 縦スライス）と同様、**レイヤ構造を厳密に守り、責務混在を避けることが最重要**。

Stage3 は以下の 4 層で構成される：

1. **Abstractions**（OrderRequest / OrderResult / IExchangeTradingClient）
2. **Infrastructure**（RestClient POST、Signer、Exception）
3. **Bitflyer Private API**（DTO / HTTP 呼び出し）
4. **Bitflyer Adapter**（Domain ⇄ DTO 変換 + インターフェース実装）

これらの層ごとに実装ポイントを解説していく。

---

## 3. Abstractions 実装ノート（Domain / Interfaces）

### 3.1 OrderRequest
```csharp
public sealed record OrderRequest(
    string ProductCode,
    OrderSide Side,
    OrderType OrderType,
    decimal Size
);
```
- Stage3 では MARKET 注文のみを扱う。
- `ProductCode` は柔軟に他取引所でも使える文字列のままで良い（列挙にしない）。
- `Size` は decimal 固定でよい（将来 FIX / WebSocket と組み合わせる場合も問題ない）。

### 3.2 OrderResult
```csharp
public sealed record OrderResult(string OrderId);
```
- 将来 LIMIT 注文などで追加情報が必要になれば拡張する。
- 既存フィールドとの後方互換性を優先する設計にする。

### 3.3 IExchangeTradingClient
```csharp
public interface IExchangeTradingClient
{
    Task<OrderResult> SendOrderAsync(OrderRequest request, CancellationToken ct = default);
}
```
- bitFlyer 固有の命名を避け、他取引所でも通用する抽象 API として設計する。
- Stage3 では `SendOrderAsync` 1 本だけ実装すればよい。

---

## 4. Infrastructure 実装ノート（POST 対応）

### 4.1 RestClient（POST 対応）
POST 呼び出し用のシグネチャ：
```csharp
Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest body, CancellationToken ct);
```
実装ポイント：
- `JsonSerializer.Serialize(body)` を行い、UTF-8 JSON として送信する。
- `HttpMethod.Post` を設定し、`Content-Type: application/json` を付与する。
- `IRequestSigner` に POST リクエスト全体を渡し、署名ヘッダを付与する。
- HTTP ステータス 200–299 以外は `ExchangeApiException` に変換する。
- JSON 変換中の例外はそのままドメイン外エラーとして `ExchangeApiException` にラップする。

#### 4.1.1 署名対象文字列の生成
```
var json = JsonSerializer.Serialize(body);
var prehash = timestamp + "POST" + path + json;
var signature = HMAC_SHA256(secret, prehash);
```
- `json` は **送信する実体と完全一致** させる（改行・空白・プロパティ順序も含む）。
- `timestamp` は clock から一度だけ取得し、署名・ヘッダに同じ値を使う。

#### 4.1.2 POST 時に付与するヘッダ
| Header | 値 |
|--------|------|
| `ACCESS-KEY` | API key |
| `ACCESS-TIMESTAMP` | clock から取得した timestamp |
| `ACCESS-SIGN` | prehash を HMAC-SHA256 した値 |
| `Content-Type` | `application/json` |
| `Accept` | `application/json` |

#### 4.1.3 エラー処理とシリアライズ
- 200–299 以外は `ExchangeApiException`（ステータスを保持）に統一。
- ネットワーク例外（Timeout/DNS など）も `ExchangeApiException` にラップする。
- decimal は `JsonSerializer` に任せ、手動の文字列化はしない（桁ぶれ防止）。

### 4.2 IRequestSigner（POST 対応）
署名対象：
```
string prehash = timestamp + method + path + bodyJson;
```
注意点：
- body は **JSON 文字列そのもの** を署名に使う。
- JSON のプロパティ順は .NET が固定で生成するので問題なし（bitFlyer 側も順序非依存）。
- GET の場合 `bodyJson` は空文字 `""` とする。
- `ACCESS-KEY` / `ACCESS-TIMESTAMP` / `ACCESS-SIGN` を必ず付ける。

### 4.3 ExchangeApiException
```csharp
public sealed class ExchangeApiException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public ExchangeApiException(string message, HttpStatusCode statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }
}
```
- Stage3 では E1（HTTP ステータスベース）のみ。
- bitFlyer 固有エラーの解釈は Stage4 以降で追加する。

---

## 5. Bitflyer Private API 実装ノート

### 5.1 インターフェース
```csharp
public interface IBitflyerPrivateTradingApi
{
    Task<BitflyerSendChildOrderResponse> SendChildOrderAsync(BitflyerSendChildOrderRequest req, CancellationToken ct);
}
```
- Stage3 では Trading API はこれ 1 本でよい。
- 今後 `CancelChildOrderAsync` などを追加可能。

### 5.2 DTO リクエスト
```csharp
public sealed class BitflyerSendChildOrderRequest
{
    [JsonPropertyName("product_code")] public string ProductCode { get; init; } = string.Empty;
    [JsonPropertyName("child_order_type")] public string ChildOrderType { get; init; } = string.Empty;
    [JsonPropertyName("side")] public string Side { get; init; } = string.Empty;
    [JsonPropertyName("size")] public decimal Size { get; init; }
}
```

### 5.3 DTO レスポンス
```csharp
public sealed class BitflyerSendChildOrderResponse
{
    [JsonPropertyName("child_order_acceptance_id")] public string ChildOrderAcceptanceId { get; init; } = string.Empty;
}
```

### 5.4 BitflyerPrivateApi 実装
```csharp
public Task<BitflyerSendChildOrderResponse> SendChildOrderAsync(
    BitflyerSendChildOrderRequest req,
    CancellationToken ct)
{
    return _restClient.PostAsync<BitflyerSendChildOrderRequest, BitflyerSendChildOrderResponse>(
        "/v1/me/sendchildorder",
        req,
        ct);
}
```
- 署名や例外処理は RestClient に完全委譲。
- Private API 層は「bitFlyer API をそのまま呼ぶ」以外の責務を持たない。

---

## 6. Bitflyer Adapter 実装ノート（ExchangeClient）

### 6.1 SendOrderAsync の主要ロジック
```csharp
public async Task<OrderResult> SendOrderAsync(OrderRequest request, CancellationToken ct)
{
    var dto = new BitflyerSendChildOrderRequest
    {
        ProductCode = request.ProductCode,
        Side = request.Side == OrderSide.Buy ? "BUY" : "SELL",
        ChildOrderType = "MARKET", // Stage3 固定
        Size = request.Size
    };

    var response = await _tradingApi.SendChildOrderAsync(dto, ct).ConfigureAwait(false);

    return new OrderResult(response.ChildOrderAcceptanceId);
}
```
注意点：
- Adapter は **DTO ⇄ Domain の変換のみ** を担当し、POST の構造や署名には関与しない。
- `ConfigureAwait(false)` はライブラリとして推奨（UI スレッドを意識しないため）。
- 値の検証（size <= 0 とか）は Stage3 では行わない（将来拡張）。

---

## 7. Factory 実装ノート

### 7.1 Factory 全体像
```csharp
public static class BitflyerClientFactory
{
    public static IExchangeClient Create(string apiKey, string apiSecret)
    {
        var httpClient = new HttpClient { BaseAddress = BitflyerApiBaseUri };

        IExchangeClock clock = new SystemClock();
        IRequestSigner signer = new BitflyerRequestSigner(apiKey, apiSecret, clock);

        IHttpTransport baseTransport = new HttpTransport(httpClient, disposeHttpClient: true);
        IHttpTransport signingTransport = new BitflyerSigningTransport(baseTransport, signer);
        IRestClient rest = new RestClient(BitflyerApiBaseUri, signingTransport);

        var privateApi = new BitflyerPrivateApi(rest);
        var publicApi = new BitflyerPublicApi(rest);

        return new BitflyerExchangeClient(publicApi, privateApi, privateApi);
    }
}
```
ポイント：
- Stage2 の構成をほぼそのまま流用。
- HttpTransport → SigningTransport → RestClient の経路で署名を適用し、RestClient は送受信 + JSON に専念させる。
- public/private の両 API に同じ RestClient を使う（署名は signer が判断）。

---

## 8. ログ / デバッグ方針（Stage3）

### 8.1 基本方針
- ログは **RestClient** に集約する（Adapter や PrivateApi に残さない）。
- 機密情報（API キー・署名・body）を直接ログに出さない。

### 8.2 最低限ログ出すと便利な情報（実装者向け）
- 呼び出しパス（`/v1/me/sendchildorder`）
- HTTP ステータスコード
- 成功時：`child_order_acceptance_id`
- 失敗時：レスポンス本文（鍵・署名を除く）

---

## 9. 今後の拡張への注意点

### 9.1 LIMIT 注文の追加
- `OrderType.Limit` を追加し、DTO に `price` を追加する必要がある。
- POST body のフィールドが増えるため、DTO の拡張または派生モデルが必要。

### 9.2 キャンセル API（`/v1/me/cancelchild
