using ExchangeApi.Exchanges.Bitflyer.Normalized.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public sealed class BitflyerParentOrderEncodeTests
{
    [Fact]
    public void BuildBodyJson_contains_expected_keys_for_send_child_order()
    {
        var request = new CreateChildOrderRequest
        {
            ProductCode = "FX_BTC_JPY",
            ChildOrderType = "LIMIT",
            Side = "BUY",
            Size = 0.1m,
            Price = 5000000m,
            MinuteToExpire = 1,
            TimeInForce = "GTC",
        };

        var bodyJson = BitflyerOrderEncoder.BuildChildOrderBodyJson(request);

        Assert.Contains("\"product_code\"", bodyJson);
        Assert.Contains("\"child_order_type\"", bodyJson);
        Assert.Contains("\"side\"", bodyJson);
    }

    [Fact]
    public void BuildBodyJson_contains_expected_keys_for_send_parent_order()
    {
        var request = new CreateParentOrderRequest
        {
            OrderMethod = "IFDOCO",
            MinuteToExpire = 10000,
            TimeInForce = "GTC",
            Parameters = new[]
            {
                new CreateParentOrderParameter
                {
                    ProductCode = "BTC_JPY",
                    ConditionType = "LIMIT",
                    Side = "BUY",
                    Size = 0.1m,
                    Price = 30000m
                }
            }
        };

        var bodyJson = BitflyerOrderEncoder.BuildParentOrderBodyJson(request);

        Assert.Contains("\"order_method\"", bodyJson);
        Assert.Contains("\"parameters\"", bodyJson);
        Assert.Contains("\"condition_type\"", bodyJson);
    }
}
