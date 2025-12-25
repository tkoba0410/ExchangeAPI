# 非公式文書（参考資料）

> ⚠ 非公式文書（Non-Authoritative）
>
> 本ディレクトリ配下の文書は参考資料であり、公式仕様ではない。
> 本リポジトリにおける唯一の公式仕様（source of truth）は `docs/TopSpec.md` である。
>
> 内容が TopSpec と矛盾する場合、必ず TopSpec を正とする。

# 03 Raw API Template (Official Mirror)

Raw = 公式 API 鏡像。意味変換・正規化・共通化は行わない。

## Raw IF（分割 + Bundle）

```csharp
public interface IBitflyerRawMarketDataApi
{
    Task<RawBoardResponse> GetBoardAsync(string productCode, CancellationToken ct = default);
    Task<RawTickerResponse> GetTickerAsync(string productCode, CancellationToken ct = default);
}

public interface IBitflyerRawApi
{
    IBitflyerRawMarketDataApi MarketData { get; }
}
```

## Raw DTO（鏡像）

> 公式が string なら string で保持。公式が number なら number（decimal等）で保持。  
> Price/Size 等のドメイン型は禁止。

```csharp
public sealed record RawTickerResponse
{
    public decimal BestBid { get; init; }
    public decimal BestAsk { get; init; }
    public decimal Ltp { get; init; }
    public decimal Volume { get; init; }
    public DateTimeOffset Timestamp { get; init; }
}

public sealed record RawBoardResponse
{
    public IReadOnlyList<RawBoardPriceSize> Bids { get; init; } = Array.Empty<RawBoardPriceSize>();
    public IReadOnlyList<RawBoardPriceSize> Asks { get; init; } = Array.Empty<RawBoardPriceSize>();
}

public sealed record RawBoardPriceSize
{
    public decimal Price { get; init; }
    public decimal Size { get; init; }
}
```

## Raw 実装（RestClient を呼ぶだけ）

```csharp
internal sealed class BitflyerRawMarketDataApi : IBitflyerRawMarketDataApi
{
    private readonly IRestClient _rest;
    public BitflyerRawMarketDataApi(IRestClient rest) => _rest = rest;

    public Task<RawBoardResponse> GetBoardAsync(string productCode, CancellationToken ct = default)
        => _rest.GetAsync<RawBoardResponse>($"/v1/board?product_code={productCode}", ct);

    public Task<RawTickerResponse> GetTickerAsync(string productCode, CancellationToken ct = default)
        => _rest.GetAsync<RawTickerResponse>($"/v1/ticker?product_code={productCode}", ct);
}

internal sealed class BitflyerRawApi : IBitflyerRawApi
{
    public IBitflyerRawMarketDataApi MarketData { get; }
    public BitflyerRawApi(IBitflyerRawMarketDataApi marketData) => MarketData = marketData;
}
```
