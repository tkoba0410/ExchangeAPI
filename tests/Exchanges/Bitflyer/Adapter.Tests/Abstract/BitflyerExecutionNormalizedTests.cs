using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using Xunit;

namespace ExchangeApi.Exchanges.Bitflyer.Tests;

public sealed class BitflyerExecutionNormalizedTests
{
    [Fact]
    public void NormalizeList_keeps_raw_snapshot()
    {
        var json = """
        [
          {
            "id": 1,
            "product_code": "BTC_JPY",
            "side": "BUY",
            "price": 100,
            "size": 0.1,
            "exec_date": "2024-01-01T00:00:00Z",
            "child_order_acceptance_id": "JRF-1"
          }
        ]
        """;

        var raw = BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<ExecutionPublicResponse>>(
            json,
            "Bitflyer.GetExecutions");
        var normalized = BitflyerExecutionNormalizer.NormalizeList(raw, json);

        Assert.Single(normalized);
        Assert.Equal(JsonValueKind.Object, normalized[0].RawSnapshot.ValueKind);
        Assert.True(normalized[0].RawSnapshot.TryGetProperty("product_code", out _));
    }
}
