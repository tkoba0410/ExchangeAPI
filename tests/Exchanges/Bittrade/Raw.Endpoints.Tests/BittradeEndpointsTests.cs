using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal;
using ExchangeApi.Exchanges.Bittrade.Wire.Constants;
using ExchangeApi.Exchanges.Bittrade.Wire.Private.Endpoints;
using ExchangeApi.Exchanges.Bittrade.Wire.Public.Endpoints;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Raw.Endpoints.Tests;

public sealed class BittradeEndpointsTests
{
    [Fact]
    public void GetSymbols_builds_request()
    {
        var req = BittradePublicEndpoints.GetSymbols();

        WireCallSpecAssertions.AssertWireCallSpec(
            req,
            method: "GET",
            path: "/v1/common/symbols",
            endpointId: BittradeEndpointIds.GetSymbols);
    }

    [Fact]
    public void GetKlines_builds_request_with_ordered_query()
    {
        var req = BittradePublicEndpoints.GetHistoryKline("btcjpy", "1min", "200");

        WireCallSpecAssertions.AssertWireCallSpec(
            req,
            method: "GET",
            path: "/market/history/kline",
            endpointId: BittradeEndpointIds.GetHistoryKline,
            query: "period=1min&symbol=btcjpy&size=200");
    }

    [Fact]
    public void GetTicker_builds_request()
    {
        var req = BittradePublicEndpoints.GetDetailMerged("btcjpy");

        WireCallSpecAssertions.AssertWireCallSpec(
            req,
            method: "GET",
            path: "/market/detail/merged",
            endpointId: BittradeEndpointIds.GetDetailMerged,
            query: "symbol=btcjpy");
    }

    [Fact]
    public void PlaceOrder_builds_request_with_body_json()
    {
        var request = new RawPrivateRequests.RawPostOrdersPlaceRequest(
            AccountId: new AccountId("123"),
            Symbol: new Symbol("btcjpy"),
            Type: new FreeText("buy-limit"),
            Amount: new FreeText("0.1"),
            Price: new FreeText("3000000"),
            Source: new FreeText("api"));

        var bodyJson = BittradeRawJson.SerializeOrThrow(request, "Bittrade.PlaceOrder");
        var req = BittradePrivateEndpoints.PostOrdersPlace(bodyJson);

        WireCallSpecAssertions.AssertWireCallSpec(
            req,
            method: "POST",
            path: "/v1/order/orders/place",
            endpointId: BittradeEndpointIds.PostOrdersPlace,
            bodyJson: "{\"account-id\":\"123\",\"symbol\":\"btcjpy\",\"type\":\"buy-limit\",\"amount\":\"0.1\",\"price\":\"3000000\",\"source\":\"api\"}");
    }
}
