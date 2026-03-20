using RawPrivateRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Private.Requests;

namespace Exchange.Bitflyer.LiveTests;

public sealed class BitflyerWirePrivateGetLiveTests
{
    [BitflyerLiveAuthFact]
    [Trait("Category", "Live")]
    [Trait("Flow", "PrivateGet")]
    [Trait("Layer", "Wire")]
    public async Task GetBalance_Returns200AndJsonArray()
    {
        using var restClient = BitflyerLiveClientFactory.CreatePrivateRestClient();
        var wire = new WireTransport(restClient);

        var call = await wire.SendAsync(BitflyerLiveWireSpecs.GetBalance());
        var response = BitflyerLiveAssert.RequireWireSuccess(call);

        using var json = JsonDocument.Parse(response.Json);
        Assert.Equal(JsonValueKind.Array, json.RootElement.ValueKind);
    }

    [BitflyerLiveAuthFact]
    [Trait("Category", "Live")]
    [Trait("Flow", "PrivateGet")]
    [Trait("Layer", "Wire")]
    public async Task GetChildOrders_Returns200AndJsonArray()
    {
        using var restClient = BitflyerLiveClientFactory.CreatePrivateRestClient();
        var wire = new WireTransport(restClient);

        var call = await wire.SendAsync(BitflyerLiveWireSpecs.GetChildOrders(BitflyerLiveSettings.DefaultProductCode));
        var response = BitflyerLiveAssert.RequireWireSuccess(call);

        using var json = JsonDocument.Parse(response.Json);
        Assert.Equal(JsonValueKind.Array, json.RootElement.ValueKind);
    }

    [BitflyerLiveAuthFact]
    [Trait("Category", "Live")]
    [Trait("Flow", "PrivateGet")]
    [Trait("Layer", "Wire")]
    public async Task GetExecutionsPrivate_Returns200AndJsonArray()
    {
        using var restClient = BitflyerLiveClientFactory.CreatePrivateRestClient();
        var wire = new WireTransport(restClient);

        var call = await wire.SendAsync(BitflyerLiveWireSpecs.GetExecutionsPrivate(BitflyerLiveSettings.DefaultProductCode));
        var response = BitflyerLiveAssert.RequireWireSuccess(call);

        using var json = JsonDocument.Parse(response.Json);
        Assert.Equal(JsonValueKind.Array, json.RootElement.ValueKind);
    }
}

public sealed class BitflyerRawPrivateGetLiveTests
{
    [BitflyerLiveAuthFact]
    [Trait("Category", "Live")]
    [Trait("Flow", "PrivateGet")]
    [Trait("Layer", "Raw")]
    public async Task GetBalance_ReturnsOkResponse()
    {
        using var restClient = BitflyerLiveClientFactory.CreatePrivateRestClient();
        var raw = BitflyerLiveClientFactory.CreateRawApi(restClient);

        var call = await raw.GetBalanceCallAsync(new RawPrivateRequests.GetBalanceRequest());
        var response = BitflyerLiveAssert.RequireOk(call);

        Assert.All(response, item => Assert.False(string.IsNullOrWhiteSpace(item.CurrencyCode)));
    }

    [BitflyerLiveAuthFact]
    [Trait("Category", "Live")]
    [Trait("Flow", "PrivateGet")]
    [Trait("Layer", "Raw")]
    public async Task GetChildOrders_ReturnsOkResponse()
    {
        using var restClient = BitflyerLiveClientFactory.CreatePrivateRestClient();
        var raw = BitflyerLiveClientFactory.CreateRawApi(restClient);

        var call = await raw.GetChildOrdersCallAsync(
            new RawPrivateRequests.GetChildOrdersRequest(
                BitflyerLiveSettings.DefaultProductCode,
                ChildOrderStatusState: new FreeText("ACTIVE")));
        var response = BitflyerLiveAssert.RequireOk(call);

        Assert.All(response, item => Assert.Equal(BitflyerLiveSettings.DefaultProductCode.Value, item.ProductCode));
    }

    [BitflyerLiveAuthFact]
    [Trait("Category", "Live")]
    [Trait("Flow", "PrivateGet")]
    [Trait("Layer", "Raw")]
    public async Task GetExecutionsPrivate_ReturnsOkResponse()
    {
        using var restClient = BitflyerLiveClientFactory.CreatePrivateRestClient();
        var raw = BitflyerLiveClientFactory.CreateRawApi(restClient);

        var call = await raw.GetExecutionsPrivateCallAsync(
            new RawPrivateRequests.GetExecutionsPrivateRequest(
                BitflyerLiveSettings.DefaultProductCode,
                Count: 10));
        var response = BitflyerLiveAssert.RequireOk(call);

        Assert.All(response, item => Assert.Equal(BitflyerLiveSettings.DefaultProductCode.Value, item.ProductCode));
    }
}

public sealed class BitflyerNormalizedPrivateGetLiveTests
{
    [BitflyerLiveAuthFact]
    [Trait("Category", "Live")]
    [Trait("Flow", "PrivateGet")]
    [Trait("Layer", "Normalized")]
    public async Task GetBalance_ReturnsOkResponse()
    {
        var api = BitflyerLiveClientFactory.CreateNormalizedApi();

        var call = await api.GetBalanceCallAsync();
        var response = BitflyerLiveAssert.RequireOk(call);

        Assert.All(response.Items, item => Assert.True(item.Value.Amount >= item.Value.Available));
    }

    [BitflyerLiveAuthFact]
    [Trait("Category", "Live")]
    [Trait("Flow", "PrivateGet")]
    [Trait("Layer", "Normalized")]
    public async Task GetChildOrders_ReturnsOkResponse()
    {
        var api = BitflyerLiveClientFactory.CreateNormalizedApi();

        var call = await api.GetChildOrdersCallAsync(BitflyerLiveSettings.DefaultSymbol);
        var response = BitflyerLiveAssert.RequireOk(call);

        Assert.All(response.Items, item => Assert.Equal(BitflyerLiveSettings.DefaultSymbol, item.Value.Symbol));
    }

    [BitflyerLiveAuthFact]
    [Trait("Category", "Live")]
    [Trait("Flow", "PrivateGet")]
    [Trait("Layer", "Normalized")]
    public async Task GetExecutionsPrivate_ReturnsOkResponse()
    {
        var api = BitflyerLiveClientFactory.CreateNormalizedApi();

        var call = await api.GetExecutionsPrivateCallAsync(BitflyerLiveSettings.DefaultSymbol);
        var response = BitflyerLiveAssert.RequireOk(call);

        Assert.All(response.Items, item => Assert.Equal(BitflyerLiveSettings.DefaultSymbol, item.Value.Symbol));
    }
}
