using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelAllChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalance;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateral;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateralHistory;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCollateralAccounts;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetChildOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetExecutions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetPositions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetTradingCommission;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendChildOrder;
using ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests;

public sealed class PrivateProtocolEndpointTests
{
    [Fact]
    public async Task GetBalance_UsesPrivateGetContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetBalanceProtocolEndpoint(transport);

        var call = await endpoint.SendAsync();

        Assert.True(call.IsSuccess);
        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/getbalance", transport.LastRequest!.Path);
        Assert.Null(transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task GetCollateral_UsesPrivateGetContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetCollateralProtocolEndpoint(transport);

        var call = await endpoint.SendAsync();

        Assert.True(call.IsSuccess);
        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/getcollateral", transport.LastRequest!.Path);
        Assert.Null(transport.LastRequest.Query);
        Assert.Null(transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task GetCollateralAccounts_UsesPrivateGetContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetCollateralAccountsProtocolEndpoint(transport);

        var call = await endpoint.SendAsync();

        Assert.True(call.IsSuccess);
        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/getcollateralaccounts", transport.LastRequest!.Path);
        Assert.Null(transport.LastRequest.Query);
        Assert.Null(transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task GetChildOrders_UsesPrivateGetContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetChildOrdersProtocolEndpoint(transport);

        var call = await endpoint.SendAsync("BTC_JPY", 10, 20, 30, "COMPLETED", "JOR1", "JRF1", "JCO1");

        Assert.True(call.IsSuccess);
        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/getchildorders", transport.LastRequest!.Path);
        Assert.NotNull(transport.LastRequest.Query);
        Assert.Equal("BTC_JPY", transport.LastRequest.Query!["product_code"]);
        Assert.Equal("10", transport.LastRequest.Query["count"]);
        Assert.Equal("20", transport.LastRequest.Query["before"]);
        Assert.Equal("30", transport.LastRequest.Query["after"]);
        Assert.Equal("COMPLETED", transport.LastRequest.Query["child_order_state"]);
        Assert.Equal("JOR1", transport.LastRequest.Query["child_order_id"]);
        Assert.Equal("JRF1", transport.LastRequest.Query["child_order_acceptance_id"]);
        Assert.Equal("JCO1", transport.LastRequest.Query["parent_order_id"]);
        Assert.Null(transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task GetExecutions_UsesPrivateGetContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetExecutionsProtocolEndpoint(transport);

        var call = await endpoint.SendAsync("BTC_JPY", 10, 20, 30, "JOR1", "JRF1");

        Assert.True(call.IsSuccess);
        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/getexecutions", transport.LastRequest!.Path);
        Assert.NotNull(transport.LastRequest.Query);
        Assert.Equal("BTC_JPY", transport.LastRequest.Query!["product_code"]);
        Assert.Equal("10", transport.LastRequest.Query["count"]);
        Assert.Equal("20", transport.LastRequest.Query["before"]);
        Assert.Equal("30", transport.LastRequest.Query["after"]);
        Assert.Equal("JOR1", transport.LastRequest.Query["child_order_id"]);
        Assert.Equal("JRF1", transport.LastRequest.Query["child_order_acceptance_id"]);
        Assert.Null(transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task GetCollateralHistory_UsesPrivateGetContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetCollateralHistoryProtocolEndpoint(transport);

        var call = await endpoint.SendAsync(10, 20, 30);

        Assert.True(call.IsSuccess);
        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/getcollateralhistory", transport.LastRequest!.Path);
        Assert.NotNull(transport.LastRequest.Query);
        Assert.Equal("10", transport.LastRequest.Query!["count"]);
        Assert.Equal("20", transport.LastRequest.Query["before"]);
        Assert.Equal("30", transport.LastRequest.Query["after"]);
        Assert.Null(transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task SendChildOrder_UsesPrivatePostContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new SendChildOrderProtocolEndpoint(transport);

        await endpoint.SendAsync("{\"x\":1}");

        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/sendchildorder", transport.LastRequest!.Path);
        Assert.Equal("{\"x\":1}", transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task GetPositions_UsesPrivateGetContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetPositionsProtocolEndpoint(transport);

        var call = await endpoint.SendAsync("FX_BTC_JPY");

        Assert.True(call.IsSuccess);
        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/getpositions", transport.LastRequest!.Path);
        Assert.NotNull(transport.LastRequest.Query);
        Assert.Equal("FX_BTC_JPY", transport.LastRequest.Query!["product_code"]);
        Assert.Null(transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task GetTradingCommission_UsesPrivateGetContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetTradingCommissionProtocolEndpoint(transport);

        var call = await endpoint.SendAsync("BTC_JPY");

        Assert.True(call.IsSuccess);
        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/gettradingcommission", transport.LastRequest!.Path);
        Assert.NotNull(transport.LastRequest.Query);
        Assert.Equal("BTC_JPY", transport.LastRequest.Query!["product_code"]);
        Assert.Null(transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task CancelChildOrder_UsesPrivatePostContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new CancelChildOrderProtocolEndpoint(transport);

        await endpoint.SendAsync("{\"x\":1}");

        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/cancelchildorder", transport.LastRequest!.Path);
        Assert.Equal("{\"x\":1}", transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task CancelAllChildOrders_UsesPrivatePostContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new CancelAllChildOrdersProtocolEndpoint(transport);

        await endpoint.SendAsync("{\"x\":1}");

        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/cancelallchildorders", transport.LastRequest!.Path);
        Assert.Equal("{\"x\":1}", transport.LastRequest.BodyText);
    }
}
