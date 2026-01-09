using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Endpoints.Tests;

public sealed class BitflyerParentOrderRawTests
{
    [Fact]
    public void SendParentOrder_serializes_request_body()
    {
        var request = new RawSendParentOrderRequest
        {
            OrderMethod = "IFDOCO",
            MinuteToExpire = 10000,
            TimeInForce = "GTC",
            Parameters = new[]
            {
                new RawSendParentOrderParameter
                {
                    ProductCode = "BTC_JPY",
                    ConditionType = "LIMIT",
                    Side = "BUY",
                    Size = 0.1m,
                    Price = 30000m
                },
                new RawSendParentOrderParameter
                {
                    ProductCode = "BTC_JPY",
                    ConditionType = "LIMIT",
                    Side = "SELL",
                    Size = 0.1m,
                    Price = 32000m
                },
                new RawSendParentOrderParameter
                {
                    ProductCode = "BTC_JPY",
                    ConditionType = "STOP_LIMIT",
                    Side = "SELL",
                    Size = 0.1m,
                    Price = 28800m,
                    TriggerPrice = 29000m
                }
            }
        };

        var json = BitflyerRawJson.SerializeOrThrow(request, "Bitflyer.SendParentOrder");

        Assert.Equal(
            "{\"order_method\":\"IFDOCO\",\"minute_to_expire\":10000,\"time_in_force\":\"GTC\",\"parameters\":[{\"product_code\":\"BTC_JPY\",\"condition_type\":\"LIMIT\",\"side\":\"BUY\",\"size\":0.1,\"price\":30000},{\"product_code\":\"BTC_JPY\",\"condition_type\":\"LIMIT\",\"side\":\"SELL\",\"size\":0.1,\"price\":32000},{\"product_code\":\"BTC_JPY\",\"condition_type\":\"STOP_LIMIT\",\"side\":\"SELL\",\"size\":0.1,\"price\":28800,\"trigger_price\":29000}]}",
            json);
    }

    [Fact]
    public void GetParentOrders_deserializes_response()
    {
        var json = """
        [
          {
            "id": 138398,
            "parent_order_id": "JCO20150707-084555-022523",
            "product_code": "BTC_JPY",
            "side": "BUY",
            "parent_order_type": "STOP",
            "price": 30000,
            "average_price": 30000,
            "size": 0.1,
            "parent_order_state": "COMPLETED",
            "expire_date": "2015-07-14T07:25:52",
            "parent_order_date": "2015-07-07T08:45:53",
            "parent_order_acceptance_id": "JRF20150707-084552-031927",
            "outstanding_size": 0,
            "cancel_size": 0,
            "executed_size": 0.1,
            "total_commission": 0
          }
        ]
        """;

        var result = BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<RawGetParentOrdersResponse>>(
            json,
            "Bitflyer.GetParentOrders");

        Assert.Single(result);
        Assert.Equal("JCO20150707-084555-022523", result[0].ParentOrderId);
        Assert.Equal("COMPLETED", result[0].ParentOrderState);
    }

    [Fact]
    public void GetParentOrder_deserializes_response()
    {
        var json = """
        {
          "id": 4242,
          "parent_order_id": "JCP20150825-046876-036161",
          "order_method": "IFDOCO",
          "expire_date": "2015-09-24T04:35:59.277",
          "time_in_force": "GTC",
          "parameters": [{
            "product_code": "BTC_JPY",
            "condition_type": "LIMIT",
            "side": "BUY",
            "price": 30000,
            "size": 0.1,
            "trigger_price": 0,
            "offset": 0
          }],
          "parent_order_acceptance_id": "JRF20150925-060559-396699"
        }
        """;

        var result = BitflyerRawJson.DeserializeOrThrow<RawGetParentOrderResponse>(
            json,
            "Bitflyer.GetParentOrder");

        Assert.Equal("JCP20150825-046876-036161", result.ParentOrderId);
        Assert.Equal("IFDOCO", result.OrderMethod);
        Assert.Single(result.Parameters);
    }
}
