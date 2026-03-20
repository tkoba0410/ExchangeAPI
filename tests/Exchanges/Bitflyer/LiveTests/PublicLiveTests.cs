using RawPublicRequests = ExchangeApi.Exchanges.Bitflyer.Raw.Public.Requests;

namespace Exchange.Bitflyer.LiveTests;

public sealed class BitflyerWirePublicLiveTests
{
    [BitflyerLivePublicFact]
    [Trait("Category", "Live")]
    [Trait("Flow", "PublicGet")]
    [Trait("Layer", "Wire")]
    public async Task GetTicker_Returns200AndTickerFields()
    {
        using var restClient = BitflyerLiveClientFactory.CreatePublicRestClient();
        var wire = new WireTransport(restClient);

        var call = await wire.SendAsync(BitflyerLiveWireSpecs.GetTicker(BitflyerLiveSettings.DefaultProductCode));
        var response = BitflyerLiveAssert.RequireWireSuccess(call);

        using var json = JsonDocument.Parse(response.Json);
        Assert.Equal(BitflyerLiveSettings.DefaultProductCode.Value, json.RootElement.GetProperty("product_code").GetString());
        Assert.True(json.RootElement.GetProperty("ltp").GetDecimal() > 0m);
    }

    [BitflyerLivePublicFact]
    [Trait("Category", "Live")]
    [Trait("Flow", "PublicGet")]
    [Trait("Layer", "Wire")]
    public async Task GetBoard_Returns200AndBookLevels()
    {
        using var restClient = BitflyerLiveClientFactory.CreatePublicRestClient();
        var wire = new WireTransport(restClient);

        var call = await wire.SendAsync(BitflyerLiveWireSpecs.GetBoard(BitflyerLiveSettings.DefaultProductCode));
        var response = BitflyerLiveAssert.RequireWireSuccess(call);

        using var json = JsonDocument.Parse(response.Json);
        Assert.True(json.RootElement.GetProperty("bids").GetArrayLength() > 0);
        Assert.True(json.RootElement.GetProperty("asks").GetArrayLength() > 0);
    }

    [BitflyerLivePublicFact]
    [Trait("Category", "Live")]
    [Trait("Flow", "PublicGet")]
    [Trait("Layer", "Wire")]
    public async Task GetExecutionsPublic_Returns200AndArrayPayload()
    {
        using var restClient = BitflyerLiveClientFactory.CreatePublicRestClient();
        var wire = new WireTransport(restClient);

        var call = await wire.SendAsync(BitflyerLiveWireSpecs.GetExecutionsPublic(BitflyerLiveSettings.DefaultProductCode));
        var response = BitflyerLiveAssert.RequireWireSuccess(call);

        using var json = JsonDocument.Parse(response.Json);
        Assert.Equal(JsonValueKind.Array, json.RootElement.ValueKind);
        Assert.True(json.RootElement.GetArrayLength() > 0);
    }
}

public sealed class BitflyerRawPublicLiveTests
{
    [BitflyerLivePublicFact]
    [Trait("Category", "Live")]
    [Trait("Flow", "PublicGet")]
    [Trait("Layer", "Raw")]
    public async Task GetTicker_ReturnsOkResponse()
    {
        using var restClient = BitflyerLiveClientFactory.CreatePublicRestClient();
        var raw = BitflyerLiveClientFactory.CreateRawApi(restClient);

        var call = await raw.GetTickerCallAsync(new RawPublicRequests.GetTickerRequest(BitflyerLiveSettings.DefaultProductCode));
        var response = BitflyerLiveAssert.RequireOk(call);

        Assert.Equal(BitflyerLiveSettings.DefaultProductCode.Value, response.ProductCode);
        Assert.True(response.LastTradedPrice > 0m);
        Assert.True(response.TickId > 0L);
    }

    [BitflyerLivePublicFact]
    [Trait("Category", "Live")]
    [Trait("Flow", "PublicGet")]
    [Trait("Layer", "Raw")]
    public async Task GetBoard_ReturnsOkResponse()
    {
        using var restClient = BitflyerLiveClientFactory.CreatePublicRestClient();
        var raw = BitflyerLiveClientFactory.CreateRawApi(restClient);

        var call = await raw.GetBoardCallAsync(new RawPublicRequests.GetBoardRequest(BitflyerLiveSettings.DefaultProductCode));
        var response = BitflyerLiveAssert.RequireOk(call);

        Assert.True(response.Bids.Count > 0);
        Assert.True(response.Asks.Count > 0);
        Assert.True(response.MidPrice > 0m);
    }

    [BitflyerLivePublicFact]
    [Trait("Category", "Live")]
    [Trait("Flow", "PublicGet")]
    [Trait("Layer", "Raw")]
    public async Task GetExecutionsPublic_ReturnsOkResponse()
    {
        using var restClient = BitflyerLiveClientFactory.CreatePublicRestClient();
        var raw = BitflyerLiveClientFactory.CreateRawApi(restClient);

        var call = await raw.GetExecutionsPublicCallAsync(
            new RawPublicRequests.GetExecutionsPublicRequest(BitflyerLiveSettings.DefaultProductCode, Count: 10));
        var response = BitflyerLiveAssert.RequireOk(call);

        Assert.True(response.Count > 0);
        Assert.All(response, item =>
        {
            Assert.True(item.Id > 0L);
            Assert.False(string.IsNullOrWhiteSpace(item.Side));
            Assert.True(item.Price > 0m);
            Assert.True(item.Size > 0m);
        });
    }
}

public sealed class BitflyerNormalizedPublicLiveTests
{
    [BitflyerLiveAuthFact]
    [Trait("Category", "Live")]
    [Trait("Flow", "PublicGet")]
    [Trait("Layer", "Normalized")]
    public async Task GetTicker_ReturnsOkResponse()
    {
        var api = BitflyerLiveClientFactory.CreateNormalizedApi();

        var call = await api.GetTickerCallAsync(BitflyerLiveSettings.DefaultProductCode);
        var response = BitflyerLiveAssert.RequireOk(call);

        Assert.Equal(BitflyerLiveSettings.DefaultProductCode, response.ProductCode);
        Assert.True(response.LastTradedPrice > 0m);
    }

    [BitflyerLiveAuthFact]
    [Trait("Category", "Live")]
    [Trait("Flow", "PublicGet")]
    [Trait("Layer", "Normalized")]
    public async Task GetBoard_ReturnsOkResponse()
    {
        var api = BitflyerLiveClientFactory.CreateNormalizedApi();

        var call = await api.GetBoardCallAsync(BitflyerLiveSettings.DefaultProductCode);
        var response = BitflyerLiveAssert.RequireOk(call);

        Assert.True(response.Bids.Count > 0);
        Assert.True(response.Asks.Count > 0);
    }

    [BitflyerLiveAuthFact]
    [Trait("Category", "Live")]
    [Trait("Flow", "PublicGet")]
    [Trait("Layer", "Normalized")]
    public async Task GetExecutionsPublic_ReturnsOkResponse()
    {
        var api = BitflyerLiveClientFactory.CreateNormalizedApi();

        var call = await api.GetExecutionsPublicCallAsync(BitflyerLiveSettings.DefaultProductCode, count: 10);
        var response = BitflyerLiveAssert.RequireOk(call);

        Assert.True(response.Items.Count > 0);
        Assert.All(response.Items, item =>
        {
            Assert.True(item.Value.Id > 0L);
            Assert.True(item.Value.Price > 0m);
            Assert.True(item.Value.Size > 0m);
        });
    }
}
