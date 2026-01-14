using System.Text.Json;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Raw.Tests;

public sealed class BittradeCreateOrderRequestTests
{
    [Fact]
    public void CreateOrderRequest_SerializesWithExpectedKeys()
    {
        var request = new RawCreateOrderRequest(
            AccountId: "account-1",
            Symbol: "btcjpy",
            Type: "buy-limit",
            Amount: "0.1",
            Price: "100",
            Source: null);

        var json = JsonSerializer.Serialize(request);

        Assert.Contains("\"account-id\"", json);
        Assert.Contains("\"symbol\"", json);
        Assert.Contains("\"type\"", json);
        Assert.Contains("\"buy-limit\"", json);
    }
}
