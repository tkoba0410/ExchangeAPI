using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using Xunit;

namespace ExchangeApi.Exchanges.Bitflyer.Tests;

public sealed class BitflyerRawTradingRequest_Tests
{
    [Fact]
    public void RawSendChildOrderRequest_Serializes_WithExpectedKeys()
    {
        var request = new RawSendChildOrderRequest
        {
            ProductCode = "BTC_JPY",
            ChildOrderType = "MARKET",
            Side = "BUY",
            Size = 0.01m,
            Price = 1.0m,
            MinuteToExpire = 1,
            TimeInForce = "GTC",
            TriggerPrice = 0.5m,
        };

        var json = JsonSerializer.Serialize(request);

        Assert.Contains("\"product_code\"", json);
        Assert.Contains("\"child_order_type\"", json);
        Assert.Contains("\"side\"", json);
        Assert.Contains("\"size\"", json);
        Assert.Contains("\"price\"", json);
        Assert.Contains("\"minute_to_expire\"", json);
        Assert.Contains("\"time_in_force\"", json);
        Assert.Contains("\"trigger_price\"", json);
    }
}
