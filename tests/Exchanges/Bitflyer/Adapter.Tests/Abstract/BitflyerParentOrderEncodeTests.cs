using ExchangeApi.Exchanges.Bitflyer.Raw.Internal;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public sealed class BitflyerParentOrderEncodeTests
{
    [Fact]
    public void BuildBodyJson_contains_expected_keys_for_send_child_order()
    {
        var request = new RawPrivateRequests.CreateChildOrderRequest
        {
            ProductCode = "FX_BTC_JPY",
            ChildOrderType = "LIMIT",
            Side = "BUY",
            Size = 0.1m,
            Price = 5000000m,
            MinuteToExpire = 1,
            TimeInForce = "GTC",
        };

        var bodyJson = BitflyerRawJson.SerializeOrThrow(request, "Bitflyer.SendChildOrder");

        Assert.Contains("\"product_code\"", bodyJson);
        Assert.Contains("\"child_order_type\"", bodyJson);
        Assert.Contains("\"side\"", bodyJson);
    }

    [Fact]
    public void BuildBodyJson_contains_expected_keys_for_send_parent_order()
    {
        var request = new RawPrivateRequests.CreateParentOrderRequest
        {
            OrderMethod = "IFDOCO",
            MinuteToExpire = 10000,
            TimeInForce = "GTC",
            Parameters = new[]
            {
                new RawPrivateRequests.CreateParentOrderParameter
                {
                    ProductCode = "BTC_JPY",
                    ConditionType = "LIMIT",
                    Side = "BUY",
                    Size = 0.1m,
                    Price = 30000m
                }
            }
        };

        var bodyJson = BitflyerRawJson.SerializeOrThrow(request, "Bitflyer.SendParentOrder");

        Assert.Contains("\"order_method\"", bodyJson);
        Assert.Contains("\"parameters\"", bodyJson);
        Assert.Contains("\"condition_type\"", bodyJson);
    }
}
