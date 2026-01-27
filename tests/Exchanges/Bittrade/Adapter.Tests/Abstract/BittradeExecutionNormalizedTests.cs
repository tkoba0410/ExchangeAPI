using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Internal.Mappers;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

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

        var raw = BittradeRawJson.DeserializeOrThrow<RawPublicModels.RawTradeResponse>(json, "Bittrade.GetTrades");
        var entries = raw.Tick?.Data ?? new List<RawPublicModels.RawTradeEntry>();
        var normalized = BittradeNormalizer.NormalizeExecutions(entries, json);

        Assert.Single(normalized);
        Assert.Equal(JsonValueKind.Object, normalized[0].RawSnapshot.ValueKind);
        Assert.True(normalized[0].RawSnapshot.TryGetProperty("price", out _));
    }
}
