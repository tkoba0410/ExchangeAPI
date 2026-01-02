using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Wire.Endpoints;
using ExchangeApi.Exchanges.Bittrade.Wire.Types;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Endpoints.Tests;

public sealed class BittradeEndpointsTests
{
    [Fact]
    public void GetSymbols_builds_request()
    {
        var req = BittradeEndpoints.GetSymbols();

        WireRequestAssertions.AssertWireRequest(
            req,
            method: "GET",
            path: "v1/common/symbols");
    }

    [Fact]
    public void GetKlines_builds_request_with_ordered_query()
    {
        var req = BittradeEndpoints.GetKlines("btcjpy", "1min", 200);

        WireRequestAssertions.AssertWireRequest(
            req,
            method: "GET",
            path: "market/history/kline",
            query: "period=1min&symbol=btcjpy&size=200");
    }

    [Fact]
    public void GetTicker_builds_request()
    {
        var req = BittradeEndpoints.GetTicker("btcjpy");

        WireRequestAssertions.AssertWireRequest(
            req,
            method: "GET",
            path: "market/detail/merged",
            query: "symbol=btcjpy");
    }

    [Fact]
    public void PlaceOrder_builds_request_with_body_json()
    {
        var request = new RawCreateOrderRequest(
            AccountId: "123",
            RawSymbol: new RawSymbol("btcjpy"),
            Type: OrderType.BuyLimit,
            Amount: "0.1",
            Price: "3000000",
            Source: "api");

        var bodyJson = BittradeRawJson.SerializeOrThrow(request, "Bittrade.PlaceOrder");
        var req = BittradeEndpoints.PlaceOrder(bodyJson);

        WireRequestAssertions.AssertWireRequest(
            req,
            method: "POST",
            path: "v1/order/orders/place",
            bodyJson: "{\"account-id\":\"123\",\"symbol\":\"btcjpy\",\"type\":\"buy-limit\",\"amount\":\"0.1\",\"price\":\"3000000\",\"source\":\"api\"}");
    }
}
