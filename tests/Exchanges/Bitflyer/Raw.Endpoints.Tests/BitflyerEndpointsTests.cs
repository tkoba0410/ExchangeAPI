using ExchangeApi.Exchanges.Bitflyer.Raw.Types;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Endpoints.Tests;

public sealed class BitflyerEndpointsTests
{
    [Fact]
    public void GetHealth_builds_request()
    {
        var req = BitflyerEndpoints.GetHealth(new RawProductCode("FX_BTC_JPY"));

        WireRequestAssertions.AssertWireRequest(
            req,
            method: "GET",
            path: "/v1/gethealth",
            query: "product_code=FX_BTC_JPY");
    }

    [Fact]
    public void GetBoardState_builds_request()
    {
        var req = BitflyerEndpoints.GetBoardState(new RawProductCode("BTC_JPY"));

        WireRequestAssertions.AssertWireRequest(
            req,
            method: "GET",
            path: "/v1/getboardstate",
            query: "product_code=BTC_JPY");
    }

    [Fact]
    public void GetExecutions_builds_request_with_ordered_query()
    {
        var req = BitflyerEndpoints.GetExecutions(
            new RawProductCode("BTC_JPY"),
            count: 100,
            before: 123,
            after: 456,
            useAliasPath: false);

        WireRequestAssertions.AssertWireRequest(
            req,
            method: "GET",
            path: "/v1/getexecutions",
            query: "product_code=BTC_JPY&count=100&before=123&after=456");
    }

    [Fact]
    public void CreateChildOrder_builds_request_with_body_json()
    {
        var request = new RawSendChildOrderRequest
        {
            ProductCode = new RawProductCode("FX_BTC_JPY"),
            ChildOrderType = "LIMIT",
            Side = "BUY",
            Size = 0.1m,
            Price = 5000000m,
            MinuteToExpire = 1,
            TimeInForce = "GTC",
        };

        var req = BitflyerEndpoints.CreateChildOrder(request);

        WireRequestAssertions.AssertWireRequest(
            req,
            method: "POST",
            path: "/v1/me/sendchildorder",
            bodyJson: "{\"product_code\":\"FX_BTC_JPY\",\"child_order_type\":\"LIMIT\",\"side\":\"BUY\",\"size\":0.1,\"price\":5000000,\"minute_to_expire\":1,\"time_in_force\":\"GTC\"}");
    }
}
