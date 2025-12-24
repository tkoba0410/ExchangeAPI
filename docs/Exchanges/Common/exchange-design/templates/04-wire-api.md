# 04 Wire (Normalized) API Template

Wire = Raw を基にした「取引所内の実用形」。
status 判定・エラー抽出・Try-parse をここで行う（ただし Common 化はしない）。

## Wire IF（分割 + Bundle）

```csharp
public interface IBitflyerWireMarketDataApi
{
    Task<WireBoard> GetBoardAsync(string productCode, CancellationToken ct = default);
    Task<WireTicker> GetTickerAsync(string productCode, CancellationToken ct = default);
}

public interface IBitflyerWireApi
{
    IBitflyerWireMarketDataApi MarketData { get; }
}
```

## Wire Models

```csharp
public sealed record WireTicker(
    decimal BestBid,
    decimal BestAsk,
    decimal Ltp,
    decimal Volume,
    DateTimeOffset Timestamp
);

public sealed record WireBoard(
    IReadOnlyList<WirePriceSize> Bids,
    IReadOnlyList<WirePriceSize> Asks
);

public sealed record WirePriceSize(decimal Price, decimal Size);
```

## Wire Mapper（Raw → Wire）

```csharp
internal static class BitflyerWireMapper
{
    public static WireTicker ToWire(RawTickerResponse raw)
        => new(raw.BestBid, raw.BestAsk, raw.Ltp, raw.Volume, raw.Timestamp);

    public static WireBoard ToWire(RawBoardResponse raw)
        => new(
            raw.Bids.Select(x => new WirePriceSize(x.Price, x.Size)).ToArray(),
            raw.Asks.Select(x => new WirePriceSize(x.Price, x.Size)).ToArray()
        );
}
```

## Wire 実装（Raw を呼び、必要な正規化を適用）

```csharp
internal sealed class BitflyerWireMarketDataApi : IBitflyerWireMarketDataApi
{
    private readonly IBitflyerRawMarketDataApi _raw;
    public BitflyerWireMarketDataApi(IBitflyerRawMarketDataApi raw) => _raw = raw;

    public async Task<WireBoard> GetBoardAsync(string productCode, CancellationToken ct = default)
    {
        var raw = await _raw.GetBoardAsync(productCode, ct);
        return BitflyerWireMapper.ToWire(raw);
    }

    public async Task<WireTicker> GetTickerAsync(string productCode, CancellationToken ct = default)
    {
        var raw = await _raw.GetTickerAsync(productCode, ct);
        return BitflyerWireMapper.ToWire(raw);
    }
}

internal sealed class BitflyerWireApi : IBitflyerWireApi
{
    public IBitflyerWireMarketDataApi MarketData { get; }
    public BitflyerWireApi(IBitflyerWireMarketDataApi marketData) => MarketData = marketData;
}
```

## status wrapper がある取引所向け（例：Envelope 正規化）

```csharp
public sealed record RawEnvelope<T>
{
    public string? Status { get; init; }
    public string? ErrorCode { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }
}

internal static class EnvelopeNormalizer
{
    public static T RequireOk<T>(RawEnvelope<T> env, string operation)
    {
        if (env is null) throw new InvalidOperationException($"{operation}: envelope is null");
        if (env.Status == "ok" && env.Data is not null) return env.Data;

        throw new ExchangeApiException(
            message: $"{operation}: status={env.Status}, code={env.ErrorCode}, msg={env.Message}",
            category: ExchangeErrorCategory.ExchangeError
        );
    }
}
```
