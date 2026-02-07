using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public sealed class BitflyerExecutionNormalizedTests
{
    [Fact]
    public void TryNormalizeList_keeps_raw_snapshot()
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

        var raw = RawJson.DeserializeOrThrow<IReadOnlyList<RawPublicDtos.ExecutionPublicResponse>>(
            json,
            "Bitflyer.GetExecutions");
        Assert.True(ExecutionNormalizer.TryNormalizeList(raw, json, out var normalized, out var error));
        Assert.Null(error);

        Assert.Single(normalized!);
        Assert.Equal(JsonValueKind.Object, normalized![0].RawSnapshot.ValueKind);
        Assert.True(normalized![0].RawSnapshot.TryGetProperty("product_code", out _));
    }
}
