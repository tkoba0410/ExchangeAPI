using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public sealed class BitflyerTickerNormalizedTests
{
    [Fact]
    public void Normalize_keeps_raw_snapshot()
    {
        var json = """
        {
          "product_code": "BTC_JPY",
          "timestamp": "2024-01-01T00:00:00Z",
          "tick_id": 1,
          "best_bid": 100,
          "best_ask": 101,
          "best_bid_size": 0.1,
          "best_ask_size": 0.2,
          "total_bid_depth": 1.1,
          "total_ask_depth": 1.2,
          "ltp": 100.5,
          "volume": 10,
          "volume_by_product": 9.9
        }
        """;

        var raw = BitflyerRawJson.DeserializeOrThrow<RawPublicDtos.Ticker>(json, "Bitflyer.GetTicker");
        var normalized = BitflyerTickerNormalizer.Normalize(raw, json);

        Assert.Equal(JsonValueKind.Object, normalized.RawSnapshot.ValueKind);
        Assert.True(normalized.RawSnapshot.TryGetProperty("best_bid", out _));
    }
}
