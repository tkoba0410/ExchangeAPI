using ExchangeApi.Exchanges.Bitflyer.Api.Raw;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Api.Raw.Internal.Encoding;
namespace ExchangeApi.Tests.Exchanges.Bitflyer.Raw.Endpoints.Tests;

public sealed class BitflyerParentOrderRawTests
{
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

        var result = BitflyerRawJson.DeserializeOrThrow<IReadOnlyList<RawPrivateDtos.RawGetParentOrdersResponse>>(
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

        var result = BitflyerRawJson.DeserializeOrThrow<RawPrivateDtos.RawGetParentOrderResponse>(
            json,
            "Bitflyer.GetParentOrder");

        Assert.Equal("JCP20150825-046876-036161", result.ParentOrderId);
        Assert.Equal("IFDOCO", result.OrderMethod);
        Assert.Single(result.Parameters);
    }
}
