using System.Text.Json;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Mappers;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public sealed class TickerNormalizedTests
{
    [Fact]
    public void Normalize_keeps_raw_snapshot()
    {
        var json = """
        {
          "status": "ok",
          "tick": {
            "close": 100,
            "open": 99,
            "low": 98,
            "high": 101,
            "amount": 12.3,
            "vol": 456.7,
            "ts": 1704067200000,
            "bid": [99.5, 1.2],
            "ask": [100.5, 1.3]
          },
          "ts": 1704067200000
        }
        """;

        var raw = RawJson.DeserializeOrThrow<RawPublicDtos.GetDetailMergedResponse>(json, "Bittrade.GetTicker");
        var ok = Normalizer.TryNormalizeTicker(raw, json, out var normalized, out var error);
        Assert.True(ok);
        Assert.NotNull(normalized);
        Assert.Null(error);

        Assert.Equal(JsonValueKind.Object, normalized!.RawSnapshot.ValueKind);
        Assert.True(normalized.RawSnapshot.TryGetProperty("tick", out _));
    }
}
