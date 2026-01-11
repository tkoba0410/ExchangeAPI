using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Exchanges.Bittrade.Normalize.Mappers;
using ExchangeApi.Exchanges.Bittrade.Raw;
using Xunit;

namespace ExchangeApi.Exchanges.Bittrade.Tests;

public sealed class BittradeExecutionNormalizedTests
{
    [Fact]
    public void NormalizeList_keeps_raw_snapshot()
    {
        var json = """
        {
          "status": "ok",
          "tick": {
            "data": [
              {
                "id": "1",
                "price": 100,
                "amount": 0.1,
                "direction": "buy",
                "ts": 1704067200000
              }
            ]
          },
          "ts": 1704067200000
        }
        """;

        var raw = BittradeRawJson.DeserializeOrThrow<RawTradeResponse>(json, "Bittrade.GetTrades");
        var entries = raw.Tick?.Data ?? new List<RawTradeEntry>();
        var normalized = BittradeNormalizer.NormalizeExecutions(entries, json);

        Assert.Single(normalized);
        Assert.Equal(JsonValueKind.Object, normalized[0].RawSnapshot.ValueKind);
        Assert.True(normalized[0].RawSnapshot.TryGetProperty("price", out _));
    }
}
