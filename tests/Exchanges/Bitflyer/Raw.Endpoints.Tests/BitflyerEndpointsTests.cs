using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Call;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Models;
using ExchangeApi.Exchanges.Bitflyer.Raw.RawApi;
using ExchangeApi.Exchanges.Bitflyer.Raw.Requests;
using ExchangeApi.Exchanges.Bitflyer.Wire.Constants;
using ExchangeApi.Exchanges.Bitflyer.Wire.Endpoints;
using PrivateModels = ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Raw.Endpoints.Tests;

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
            endpointId: BitflyerEndpointIds.GetHealth,
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
            endpointId: BitflyerEndpointIds.GetBoardState,
            query: "product_code=BTC_JPY");
    }

    [Fact]
    public void GetTicker_builds_request()
    {
        var req = BitflyerEndpoints.GetTicker("BTC_JPY");

        WireCallSpecAssertions.AssertWireCallSpec(
            req,
            method: "GET",
            path: "/v1/getticker",
            endpointId: BitflyerEndpointIds.GetTicker,
            query: "product_code=BTC_JPY");
    }

    [Fact]
    public void GetExecutions_builds_request_with_ordered_query()
    {
        var req = BitflyerEndpoints.GetExecutionsPublic(
            "BTC_JPY",
            count: "100",
            before: "123",
            after: "456");

        WireCallSpecAssertions.AssertWireCallSpec(
            req,
            method: "GET",
            path: "/v1/getexecutions",
            endpointId: BitflyerEndpointIds.GetExecutionsPublic,
            query: "product_code=BTC_JPY&count=100&before=123&after=456");
    }

    [Fact]
    public void CreateChildOrder_builds_request_with_body_json()
    {
        var bodyJson = "{\"dummy\":true}";
        var req = BitflyerEndpoints.SendChildOrder(bodyJson);

        WireCallSpecAssertions.AssertWireCallSpec(
            req,
            method: "POST",
            path: "/v1/me/sendchildorder",
            endpointId: BitflyerEndpointIds.SendChildOrder,
            bodyJson: bodyJson);
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
            endpointId: BitflyerEndpointIds.SendParentOrder,
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
            endpointId: BitflyerEndpointIds.CancelParentOrder,
            bodyJson: bodyJson);
    }

    [Fact]
    public void GetParentOrders_builds_request_with_query()
    {
        var req = BitflyerEndpoints.GetParentOrders(
            "BTC_JPY",
            parentOrderState: "ACTIVE",
            count: "10",
            before: "100",
            after: "50");

        WireCallSpecAssertions.AssertWireCallSpec(
            req,
            method: "GET",
            path: "/v1/me/getparentorders",
            endpointId: BitflyerEndpointIds.GetParentOrders,
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
            endpointId: BitflyerEndpointIds.GetParentOrder,
            query: "parent_order_acceptance_id=JRF-1");
    }
}
