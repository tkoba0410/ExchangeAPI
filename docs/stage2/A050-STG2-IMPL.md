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

- **Bitflyer.Private**
  - bitFlyer 固有のパス・レスポンス構造を扱う層。
  - `/v1/me/getbalance` の呼び出しと、`BitflyerBalanceResponse` の定義に専念する。

- **Bitflyer.Adapter (ExchangeClient)**
  - Private API DTO → ドメインの変換と、`IExchangeAccountClient` 実装に徹する。

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
- エラー判定のロジックは RestClient に集約し、Private API / Adapter 層での HTTP 判定は行わない。

---

## 5. Bitflyer Private API 実装ノート

### 5.1 DTO 定義
```csharp
public sealed class BitflyerBalanceResponse
{
    [JsonPropertyName("currency_code")]
    public string CurrencyCode { get; init; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; init; }

    [JsonPropertyName("available")]
    public decimal Available { get; init; }
}
```
- JSON 属性名は `JsonPropertyName` で指定し、bitFlyer の生レスポンスを忠実に表現する。

### 5.2 IBitflyerPrivateApi / 実装
```csharp
public interface IBitflyerPrivateApi
{
    Task<IReadOnlyList<BitflyerBalanceResponse>> GetBalancesAsync(CancellationToken ct = default);
}

public sealed class BitflyerPrivateApi : IBitflyerPrivateApi
{
    private readonly IRestClient _restClient;

    public BitflyerPrivateApi(IRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public Task<IReadOnlyList<BitflyerBalanceResponse>> GetBalancesAsync(CancellationToken ct = default)
    {
        return _restClient.GetAsync<IReadOnlyList<BitflyerBalanceResponse>>(
            "/v1/me/getbalance",
            query: null,
            ct);
    }
}
```

- Private API 層は **API パスと DTO の管理に専念**し、署名や例外処理は RestClient へ委譲する。

---

## 6. Bitflyer.Adapter 実装ノート

### 6.1 BitflyerExchangeClient
```csharp
public sealed class BitflyerExchangeClient : IExchangeClient
{
    private readonly IBitflyerPublicApi _publicApi;
    private readonly IBitflyerPrivateApi _privateApi;

    public BitflyerExchangeClient(IBitflyerPublicApi publicApi, IBitflyerPrivateApi privateApi)
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
    }

    public async Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken ct = default)
    {
        var rawBalances = await _privateApi.GetBalancesAsync(ct).ConfigureAwait(false);
        return rawBalances
            .Select(dto => new Balance(dto.CurrencyCode, dto.Amount, dto.Available))
            .ToArray();
    }

    // 他のメソッドは Stage3 以降に実装
}
```

- 変換は `BitflyerExchangeClient` 内で完結し、特別な Mapper クラスは不要。
- Stage2 の時点では、他の `IExchangeClient` メソッドは `NotImplementedException` または TODO でよい。
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

        var httpClient = new HttpClient { BaseAddress = BitflyerApiBaseUri };
        IHttpTransport baseTransport = new HttpTransport(httpClient, disposeHttpClient: true);

        IExchangeClock clock = new SystemClock();
        IRequestSigner signer = new BitflyerRequestSigner(apiKey, apiSecret, clock);
        IHttpTransport signingTransport = new BitflyerSigningTransport(baseTransport, signer);

        IRestClient restClient = new RestClient(BitflyerApiBaseUri, signingTransport);
        var publicApi = new BitflyerPublicApi(restClient);
        var privateApi = new BitflyerPrivateApi(restClient);

        return new BitflyerExchangeClient(publicApi, privateApi);
    }
}
```

- Stage2 時点では、1つの `HttpClient` と `HttpTransport` の組み合わせで十分。
- Factory は「すぐに使える IExchangeClient を返す」ことに特化し、鍵の取得処理は持たない。
- `Create(apiKey, apiSecret)` は **鍵を入力として受け取るだけ** に徹し、取得は `IApiCredentialProvider` に委譲する。

### 7.2 IApiCredentialProvider の利用
```csharp
public interface IApiCredentialProvider
{
    ApiCredentials Get(string exchangeId, string accountId);
}

public sealed record ApiCredentials(string ApiKey, string ApiSecret);
```
- Factory には `Create(IApiCredentialProvider provider, string exchangeId, string accountId)` オーバーロードを追加し、`provider` から取得したキーを `Create(apiKey, apiSecret)` に委譲する。
- `provider` が `null` の場合は `ArgumentNullException`、取得結果のキー/シークレットが空の場合は `ArgumentException` として扱う。
- Factory が鍵をキャッシュする必要はない。呼び出しごとに Provider から取得しても差し支えない。

### 7.3 Credential Provider 実装例
- `EnvironmentVariableApiCredentialProvider`
  - 変数名は `<EXCHANGE>_<ACCOUNT>_API_KEY` / `<EXCHANGE>_<ACCOUNT>_API_SECRET` を推奨（例: `BITFLYER_DEFAULT_API_KEY`）。
  - `exchangeId` / `accountId` から変数名を組み立て、未設定時は `InvalidOperationException` などで通知する。
- `WindowsCredentialManagerApiCredentialProvider`
  - 汎用資格情報に `exchangeId/accountId/api_key|api_secret` のような名称で保存し、`CredRead` で取得する。
  - Windows 標準 UI では平文表示できないが、同一ユーザーであれば API 経由で読み出せる点に留意し、最小権限で運用する。
- `CompositeCredentialProvider`
  - 複数の Provider（環境変数 → Windows → CI シークレットなど）を順番に試し、最初に有効な `ApiCredentials` を返す。
  - フォールバック構成により、運用の移行やローテーション時に段階的な切り替えが可能。

### 7.4 資格情報のガイドライン
- API キー/シークレットは Git 管理下のファイルに保存しない。環境変数・資格情報マネージャー・CI シークレットなどの安全なストアを利用する。
- 平文キーをログ / 例外 / UI / クリップボードに出さない。必要最小限のオンメモリ利用に留め、利用後は早期にスコープ外へ追い出す。
- 鍵の取得責務は Orchestration 層に集約し、RestClient / Signer / Private API 層には渡さない。
- 多取引所・多アカウントを考慮し、`provider.Get("bitflyer", "default")` のように `exchangeId` / `accountId` を指定できる API 形態を維持する。

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
