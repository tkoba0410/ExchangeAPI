using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Wire.Endpoints;
using PrivateModels = ExchangeApi.Exchanges.Bitflyer.Raw.Private;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Endpoints.Tests;

public sealed class BitflyerEndpointsTests
{
    [Fact]
    public void GetHealth_builds_request()
    {
        var req = BitflyerEndpoints.GetHealth("FX_BTC_JPY");

        WireCallSpecAssertions.AssertWireCallSpec(
            req,
            method: "GET",
            path: "/v1/gethealth",
            query: "product_code=FX_BTC_JPY");
    }

    [Fact]
    public void GetBoardState_builds_request()
    {
        var req = BitflyerEndpoints.GetBoardState("BTC_JPY");

        WireCallSpecAssertions.AssertWireCallSpec(
            req,
            method: "GET",
            path: "/v1/getboardstate",
            query: "product_code=BTC_JPY");
    }

    [Fact]
    public void GetTicker_builds_request()
    {
        var req = BitflyerEndpoints.GetTicker("BTC_JPY", useAliasPath: false);

        WireCallSpecAssertions.AssertWireCallSpec(
            req,
            method: "GET",
            path: "/v1/getticker",
            query: "product_code=BTC_JPY");
    }

    [Fact]
    public void GetExecutions_builds_request_with_ordered_query()
    {
        var req = BitflyerEndpoints.GetExecutions(
            "BTC_JPY",
            count: 100,
            before: 123,
            after: 456,
            useAliasPath: false);

        WireCallSpecAssertions.AssertWireCallSpec(
            req,
            method: "GET",
            path: "/v1/getexecutions",
            query: "product_code=BTC_JPY&count=100&before=123&after=456");
    }

    [Fact]
    public void CreateChildOrder_builds_request_with_body_json()
    {
        var request = new PrivateModels.CreateChildOrderRequest
        {
            ProductCode = "FX_BTC_JPY",
            ChildOrderType = "LIMIT",
            Side = "BUY",
            Size = 0.1m,
            Price = 5000000m,
            MinuteToExpire = 1,
            TimeInForce = "GTC",
        };

        var shape = BitflyerRawMappers.MapSendChildOrderRequest(request);
        var bodyJson = BitflyerRawJson.SerializeOrThrow(shape, "Bitflyer.CreateChildOrder");
        var req = BitflyerEndpoints.SendChildOrder(bodyJson);

        WireCallSpecAssertions.AssertWireCallSpec(
            req,
            method: "POST",
            path: "/v1/me/sendchildorder",
            bodyJson: "{\"product_code\":\"FX_BTC_JPY\",\"child_order_type\":\"LIMIT\",\"side\":\"BUY\",\"size\":0.1,\"price\":5000000,\"minute_to_expire\":1,\"time_in_force\":\"GTC\"}");
    }

    [Fact]
    public void SendParentOrder_builds_request_with_body_json()
    {
        var bodyJson = "{\"dummy\":\"payload\"}";
        var req = BitflyerEndpoints.SendParentOrder(bodyJson);

        WireCallSpecAssertions.AssertWireCallSpec(
            req,
            method: "POST",
            path: "/v1/me/sendparentorder",
            bodyJson: bodyJson);
    }

    [Fact]
    public void CancelParentOrder_builds_request_with_body_json()
    {
        var request = new PrivateModels.CancelParentOrderRequest
        {
            ProductCode = "BTC_JPY",
            ParentOrderAcceptanceId = "JRF-1",
        };

        var bodyJson = BitflyerRawJson.SerializeOrThrow(request, "Bitflyer.CancelParentOrder");
        var req = BitflyerEndpoints.CancelParentOrder(bodyJson);

        WireCallSpecAssertions.AssertWireCallSpec(
            req,
            method: "POST",
            path: "/v1/me/cancelparentorder",
            bodyJson: bodyJson);
    }

    [Fact]
    public void GetParentOrders_builds_request_with_query()
    {
        var req = BitflyerEndpoints.GetParentOrders(
            "BTC_JPY",
            parentOrderState: "ACTIVE",
            count: 10,
            before: 100,
            after: 50);

        WireCallSpecAssertions.AssertWireCallSpec(
            req,
            method: "GET",
            path: "/v1/me/getparentorders",
            query: "product_code=BTC_JPY&parent_order_state=ACTIVE&count=10&before=100&after=50");
    }

    [Fact]
    public void GetParentOrder_builds_request_with_acceptance_id()
    {
        var req = BitflyerEndpoints.GetParentOrder(parentOrderAcceptanceId: "JRF-1");

        WireCallSpecAssertions.AssertWireCallSpec(
            req,
            method: "GET",
            path: "/v1/me/getparentorder",
            query: "parent_order_acceptance_id=JRF-1");
    }
}
