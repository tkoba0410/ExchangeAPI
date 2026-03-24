using ExchangeApi.Exchanges.Bitflyer.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.CancelParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetAddresses;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBalanceHistory;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetBankAccounts;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCoinIns;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetCoinOuts;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetDeposits;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetParentOrders;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetPermissions;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.GetWithdrawals;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.SendParentOrder;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Private.Endpoints.Withdraw;
using ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests.Fakes;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests;

public sealed class AdditionalPrivateProtocolEndpointTests
{
    [Fact]
    public async Task GetPermissions_UsesPrivateGetContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetPermissionsProtocolEndpoint(transport);

        var call = await endpoint.SendAsync();

        Assert.True(call.IsSuccess);
        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/getpermissions", transport.LastRequest!.Path);
        Assert.Null(transport.LastRequest.Query);
        Assert.Null(transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task GetAddresses_UsesPrivateGetContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetAddressesProtocolEndpoint(transport);

        var call = await endpoint.SendAsync();

        Assert.True(call.IsSuccess);
        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/getaddresses", transport.LastRequest!.Path);
        Assert.Null(transport.LastRequest.Query);
        Assert.Null(transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task GetCoinIns_UsesPrivateGetContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetCoinInsProtocolEndpoint(transport);

        var call = await endpoint.SendAsync(10, 20, 30);

        Assert.True(call.IsSuccess);
        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/getcoinins", transport.LastRequest!.Path);
        Assert.NotNull(transport.LastRequest.Query);
        Assert.Equal("10", transport.LastRequest.Query!["count"]);
        Assert.Equal("20", transport.LastRequest.Query["before"]);
        Assert.Equal("30", transport.LastRequest.Query["after"]);
        Assert.Null(transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task GetCoinOuts_UsesPrivateGetContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetCoinOutsProtocolEndpoint(transport);

        var call = await endpoint.SendAsync(10, 20, 30);

        Assert.True(call.IsSuccess);
        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/getcoinouts", transport.LastRequest!.Path);
        Assert.NotNull(transport.LastRequest.Query);
        Assert.Equal("10", transport.LastRequest.Query!["count"]);
        Assert.Equal("20", transport.LastRequest.Query["before"]);
        Assert.Equal("30", transport.LastRequest.Query["after"]);
        Assert.Null(transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task GetBankAccounts_UsesPrivateGetContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetBankAccountsProtocolEndpoint(transport);

        var call = await endpoint.SendAsync();

        Assert.True(call.IsSuccess);
        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/getbankaccounts", transport.LastRequest!.Path);
        Assert.Null(transport.LastRequest.Query);
        Assert.Null(transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task GetDeposits_UsesPrivateGetContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetDepositsProtocolEndpoint(transport);

        var call = await endpoint.SendAsync(10, 20, 30);

        Assert.True(call.IsSuccess);
        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/getdeposits", transport.LastRequest!.Path);
        Assert.NotNull(transport.LastRequest.Query);
        Assert.Equal("10", transport.LastRequest.Query!["count"]);
        Assert.Equal("20", transport.LastRequest.Query["before"]);
        Assert.Equal("30", transport.LastRequest.Query["after"]);
        Assert.Null(transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task Withdraw_UsesPrivatePostContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new WithdrawProtocolEndpoint(transport);

        await endpoint.SendAsync("{\"x\":1}");

        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/withdraw", transport.LastRequest!.Path);
        Assert.Equal("{\"x\":1}", transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task GetWithdrawals_UsesPrivateGetContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetWithdrawalsProtocolEndpoint(transport);

        var call = await endpoint.SendAsync(10, 20, 30, "MSG1");

        Assert.True(call.IsSuccess);
        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/getwithdrawals", transport.LastRequest!.Path);
        Assert.NotNull(transport.LastRequest.Query);
        Assert.Equal("10", transport.LastRequest.Query!["count"]);
        Assert.Equal("20", transport.LastRequest.Query["before"]);
        Assert.Equal("30", transport.LastRequest.Query["after"]);
        Assert.Equal("MSG1", transport.LastRequest.Query["message_id"]);
        Assert.Null(transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task GetParentOrders_UsesPrivateGetContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetParentOrdersProtocolEndpoint(transport);

        var call = await endpoint.SendAsync("BTC_JPY", 10, 20, 30, "ACTIVE");

        Assert.True(call.IsSuccess);
        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/getparentorders", transport.LastRequest!.Path);
        Assert.NotNull(transport.LastRequest.Query);
        Assert.Equal("BTC_JPY", transport.LastRequest.Query!["product_code"]);
        Assert.Equal("10", transport.LastRequest.Query["count"]);
        Assert.Equal("20", transport.LastRequest.Query["before"]);
        Assert.Equal("30", transport.LastRequest.Query["after"]);
        Assert.Equal("ACTIVE", transport.LastRequest.Query["parent_order_state"]);
        Assert.Null(transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task GetParentOrder_UsesPrivateGetContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetParentOrderProtocolEndpoint(transport);

        var call = await endpoint.SendAsync("JPO1", "JPA1");

        Assert.True(call.IsSuccess);
        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/getparentorder", transport.LastRequest!.Path);
        Assert.NotNull(transport.LastRequest.Query);
        Assert.Equal("JPO1", transport.LastRequest.Query!["parent_order_id"]);
        Assert.Equal("JPA1", transport.LastRequest.Query["parent_order_acceptance_id"]);
        Assert.Null(transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task GetBalanceHistory_UsesPrivateGetContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new GetBalanceHistoryProtocolEndpoint(transport);

        var call = await endpoint.SendAsync("JPY", 10, 20, 30);

        Assert.True(call.IsSuccess);
        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/getbalancehistory", transport.LastRequest!.Path);
        Assert.NotNull(transport.LastRequest.Query);
        Assert.Equal("JPY", transport.LastRequest.Query!["currency_code"]);
        Assert.Equal("10", transport.LastRequest.Query["count"]);
        Assert.Equal("20", transport.LastRequest.Query["before"]);
        Assert.Equal("30", transport.LastRequest.Query["after"]);
        Assert.Null(transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task SendParentOrder_UsesPrivatePostContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new SendParentOrderProtocolEndpoint(transport);

        await endpoint.SendAsync("{\"x\":1}");

        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/sendparentorder", transport.LastRequest!.Path);
        Assert.Equal("{\"x\":1}", transport.LastRequest.BodyText);
    }

    [Fact]
    public async Task CancelParentOrder_UsesPrivatePostContract()
    {
        var transport = new FakeProtocolTransport();
        var endpoint = new CancelParentOrderProtocolEndpoint(transport);

        await endpoint.SendAsync("{\"x\":1}");

        Assert.Equal(ProtocolTransportAuthMode.KeySecret, transport.LastAuthMode);
        Assert.Equal("/v1/me/cancelparentorder", transport.LastRequest!.Path);
        Assert.Equal("{\"x\":1}", transport.LastRequest.BodyText);
    }
}
