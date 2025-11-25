# A050-STG2-IMPL Stage2 実装ノート（get balance）

## 1. 本文書の目的
Stage2（get balance）は、bitFlyer Private API の最初の実装ステージとして、
`/v1/me/getbalance` を Abstractions 経由で利用可能にすることを目的とする。

本ドキュメントでは、実装時に考慮すべきポイント・パターン・注意事項を簡潔にまとめ、
後続の Private API 実装（collateral / positions / executions / orders）のテンプレートとする。

---

## 2. 実装方針の全体像

### 2.1 レイヤごとの役割を崩さない
- **Abstractions**
  - ドメイン型 (`Balance`) とインターフェース (`IExchangeAccountClient`) のみ定義し、実装は持たない。

- **Infrastructure**
  - HttpClient / 署名 / JSON / 例外処理を管理する技術レイヤ。
  - bitFlyer 固有の API パスや DTO を知らない。

- **Bitflyer.Raw**
  - bitFlyer 固有のパス・レスポンス構造を扱う層。
  - `/v1/me/getbalance` の呼び出しと、`BalanceResponse` の定義に専念する。

- **Bitflyer.Adapter (ExchangeClient)**
  - Raw DTO → ドメインの変換と、`IExchangeAccountClient` 実装に徹する。

### 2.2 「Private GET テンプレート」として実装する
- get balance の実装は、今後の以下 API のテンプレートとなることを意識する：
  - `/v1/me/getcollateral`
  - `/v1/me/getpositions`
  - `/v1/me/getexecutions`
- そのため、命名・構造・エラー処理を「汎用的に再利用しやすい形」に揃える。

---

## 3. Abstractions 実装ノート

### 3.1 Balance record
```csharp
public sealed record Balance(
    string Currency,
    decimal Amount,
    decimal Available
);
```

- 取引所固有のフィールド（例: 口座種別、ロック残高の内訳）は Stage2 では持たせない。
- 必要になった場合はフィールド追加で対応し、既存プロパティは変更しない方針とする（後方互換性の確保）。

### 3.2 IExchangeAccountClient / IExchangeClient
- `IExchangeAccountClient` に `GetBalancesAsync` を追加する。
- `IExchangeClient` は `IExchangeAccountClient` を継承する。
- メソッド名・引数は「他取引所でも同じ意味で使える」ことを基準に決める。

---

## 4. Infrastructure 実装ノート

### 4.1 IExchangeClock / SystemClock
```csharp
public interface IExchangeClock
{
    DateTimeOffset UtcNow { get; }
}

public sealed class SystemClock : IExchangeClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
```
- 署名の timestamp 生成・テスト時の差し替えを想定して抽象化する。

### 4.2 IRequestSigner（bitFlyer 用）
- 署名アルゴリズム（例）:
  - `timestamp + method + path + body` を連結した文字列を HMAC-SHA256 で署名。
- 責務:
  - `ACCESS-KEY` / `ACCESS-TIMESTAMP` / `ACCESS-SIGN` / `Content-Type` ヘッダを設定すること。
- リクエストの `RequestUri`, `Method`, `Content` から必要情報を取得する。

### 4.3 IRestClient / RestClient
- 最低限のメソッド:
```csharp
Task<T> GetAsync<T>(string path, object? query = null, CancellationToken ct = default);
```
- 実装のポイント:
  - `HttpClient` の BaseAddress に `https://api.bitflyer.com` を設定。
  - `query` オブジェクトは匿名型などで受け取り、`?key=value` 形式に組み立てる。
  - リクエスト送信前に `IRequestSigner` を呼び出して署名ヘッダを付与する。
  - レスポンスのステータスコードが 2xx 以外の場合は `ExchangeApiException` を送出する（メッセージ・ステータスを保持）。
  - レスポンスボディは `System.Text.Json` で `T` にデシリアライズする。

### 4.4 ExchangeApiException
- コンストラクタ例:
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
- エラー判定のロジックは RestClient に集約し、Raw / Adapter 層での HTTP 判定は行わない。

---

## 5. Bitflyer.Raw 実装ノート

### 5.1 DTO 定義
```csharp
public sealed class BalanceResponse
{
    public string CurrencyCode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Available { get; set; }
}
```
- プロパティ名は C# の命名規約に合わせてパスカルケースで定義し、
  JSON 属性名（`currency_code`）とのマッピングは `JsonPropertyName` 属性などで行う（必要に応じて）。

### 5.2 IBitflyerRawApiClient / 実装
```csharp
public interface IBitflyerRawApiClient
{
    Task<IReadOnlyList<BalanceResponse>> GetBalanceAsync(CancellationToken ct = default);
}

public sealed class BitflyerRawApiClient : IBitflyerRawApiClient
{
    private readonly IRestClient _rest;

    public BitflyerRawApiClient(IRestClient rest)
    {
        _rest = rest;
    }

    public Task<IReadOnlyList<BalanceResponse>> GetBalanceAsync(CancellationToken ct = default)
        => _rest.GetAsync<IReadOnlyList<BalanceResponse>>("/v1/me/getbalance", null, ct);
}
```

- Raw 層は **パスと DTO の管理に特化**し、例外処理は RestClient に委譲する。

---

## 6. Bitflyer.Adapter 実装ノート

### 6.1 Mapper
```csharp
public static class BitflyerDtoMapper
{
    public static Balance ToBalance(BalanceResponse dto)
        => new(
            dto.CurrencyCode,
            dto.Amount,
            dto.Available
        );
}
```
- 変換処理は単純に値を移すだけに留める。
- 将来、通貨コードの正規化やフィルタリングが必要になった場合でも、
  まずは別メソッドとして追加し、既存の `ToBalance` は後方互換性を保つ。

### 6.2 BitflyerExchangeClient
```csharp
public sealed class BitflyerExchangeClient : IExchangeClient
{
    private readonly IBitflyerRawApiClient _raw;

    public BitflyerExchangeClient(IBitflyerRawApiClient raw)
    {
        _raw = raw;
    }

    public async Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken ct = default)
    {
        var dtoList = await _raw.GetBalanceAsync(ct).ConfigureAwait(false);
        return dtoList.Select(BitflyerDtoMapper.ToBalance).ToList();
    }

    // 他のメソッドは Stage3 以降に実装
}
```

- Stage2 の時点では、他の `IExchangeClient` メソッドは `NotImplementedException` または TODO として差し支えない。
- `ConfigureAwait(false)` の使用は、ライブラリとしての利用を想定して推奨する。

---

## 7. Factory 実装ノート

### 7.1 BitflyerClientFactory
```csharp
public static class BitflyerClientFactory
{
    public static IExchangeClient Create(string apiKey, string apiSecret)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("API key is required.", nameof(apiKey));
        if (string.IsNullOrWhiteSpace(apiSecret))
            throw new ArgumentException("API secret is required.", nameof(apiSecret));

        var httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.bitflyer.com")
        };

        IExchangeClock clock = new SystemClock();
        IRequestSigner signer = new BitflyerRequestSigner(apiKey, apiSecret, clock);
        IRestClient rest = new RestClient(httpClient, signer);

        IBitflyerRawApiClient raw = new BitflyerRawApiClient(rest);
        return new BitflyerExchangeClient(raw);
    }
}
```

- Stage2 時点では、HttpClient のライフサイクルは単純な new でよいが、
  将来的には `IHttpClientFactory` の利用や DI への移行を検討する。
- Factory は「すぐに使える IExchangeClient を返す」ことに特化し、
  ロジックは持たない。

---

## 8. ログ・デバッグの方針（Stage2 時点）
- 最低限、以下の情報をログに出せるようにしておくとデバッグが容易になる。
  - 呼び出した API パス（`/v1/me/getbalance`）
  - ステータスコード
  - 失敗時のレスポンスボディ（APIキー等の機密情報は出さない）
- ログの仕組み自体（ILogger 等）は Stage2 の必須範囲外とし、
  実装済みであれば活用する、程度の扱いとする。

---

## 9. 今後の拡張を見据えた注意点
- `GetBalanceAsync` のパターンをそのまま `GetCollateralAsync` / `GetPositionsAsync` に転用できるよう、
  命名・構造・責務分担を意識して実装する。
- 現時点のエラー処理（E1）は暫定であり、Stage3 以降で E2 以上に拡張する可能性があることを念頭に置く。
- Abstractions / Infrastructure / Bitflyer の境界を崩さないようにし、
  取引所追加時にも同じ構造を踏襲できるようにする。

