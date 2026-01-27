using System.Text.Json;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Internal.Mappers;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public sealed class BittradeTickerNormalizedTests
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

        var raw = BittradeRawJson.DeserializeOrThrow<RawPublicModels.RawMergedResponse>(json, "Bittrade.GetTicker");
        var normalized = BittradeNormalizer.NormalizeTicker(raw, json);

        Assert.Equal(JsonValueKind.Object, normalized.RawSnapshot.ValueKind);
        Assert.True(normalized.RawSnapshot.TryGetProperty("tick", out _));
    }
}
