using System.Text.Json;
using ExchangeApi.Primitives.DomainCommon.Types;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Raw.Tests;

public sealed class BittradeCreateOrderRequestTests
{
    [Fact]
    public void CreateOrderRequest_SerializesWithExpectedKeys()
    {
        var request = new RawPrivateRequests.RawCreateOrderRequest(
            AccountId: new AccountId("account-1"),
            Symbol: new Symbol("btcjpy"),
            Type: new FreeText("buy-limit"),
            Amount: new FreeText("0.1"),
            Price: new FreeText("100"),
            Source: null);

        var json = JsonSerializer.Serialize(request);

        Assert.Contains("\"account-id\"", json);
        Assert.Contains("\"symbol\"", json);
        Assert.Contains("\"type\"", json);
        Assert.Contains("\"buy-limit\"", json);
    }
}
