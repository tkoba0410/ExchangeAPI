using ExchangeApi.Stage10.Bitflyer.Vocabulary;
using ExchangeApi.Stage10.Bitflyer.Normalized.Public.Api;
using ExchangeApi.Stage10.Bitflyer.Normalized.Public.Requests;
using ExchangeApi.Stage10.Bitflyer.Wire.Public.Api;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Tests.Stage10.Bitflyer.Normalized.Tests;

public sealed class GetTickerNormalizedTests
{
    [Fact]
    public async Task GetTickerAsync_SuccessfullyMapsResponse()
    {
        var wire = new StubPublicWireApi(CreateWireOk(
            "GetTicker",
            """
            {
              "product_code": "BTC_JPY",
              "state": "RUNNING",
              "timestamp": "2024-01-01T00:00:00Z",
              "tick_id": 123,
              "best_bid": 100,
              "best_ask": 101,
              "best_bid_size": 0.1,
              "best_ask_size": 0.2,
              "total_bid_depth": 10,
              "total_ask_depth": 20,
              "market_bid_size": 1.2,
              "market_ask_size": 1.3,
              "ltp": 100.5,
              "volume": 200,
              "volume_by_product": 300
            }
            """));
        var api = new BitflyerPublicNormalizedApi(wire);

        var call = await api.GetTickerAsync(new GetTickerRequest { ProductCode = ProductCodes.BtcJpy });
        var ok = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Normalized.Public.Dtos.GetTickerResponse>.Ok>(call.Result);

        Assert.Equal(ProductCodes.BtcJpy, ok.Response.ProductCode);
        Assert.Equal("RUNNING", ok.Response.State);
        Assert.Equal(100.5m, ok.Response.Ltp);
        Assert.Equal(ProductCodes.BtcJpy, wire.LastProductCode);
    }

    [Fact]
    public async Task GetTickerAsync_WithEmptyProductCode_ReturnsSemanticErrorWithoutCallingWire()
    {
        var wire = new StubPublicWireApi(CreateWireOk("GetTicker", "{}"));
        var api = new BitflyerPublicNormalizedApi(wire);

        var call = await api.GetTickerAsync(new GetTickerRequest { ProductCode = "" });
        var err = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Normalized.Public.Dtos.GetTickerResponse>.Err>(call.Result);

        Assert.Equal(CallErrorKind.Semantic, err.Error.Kind);
        Assert.Null(wire.LastProductCode);
    }

    [Fact]
    public async Task GetTickerAsync_WithHttpFailure_ReturnsHttpError()
    {
        var wire = new StubPublicWireApi(CreateWireOk("GetTicker", "{\"status\":\"ng\"}", statusCode: 400));
        var api = new BitflyerPublicNormalizedApi(wire);

        var call = await api.GetTickerAsync(new GetTickerRequest());
        var err = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Normalized.Public.Dtos.GetTickerResponse>.Err>(call.Result);

        Assert.Equal(CallErrorKind.Http, err.Error.Kind);
        Assert.Equal(400, err.Error.HttpStatus);
        Assert.Contains("\"status\":\"ng\"", err.Error.BodySnippet);
    }

    [Fact]
    public async Task GetTickerAsync_WithInvalidJson_ReturnsCodecError()
    {
        var wire = new StubPublicWireApi(CreateWireOk("GetTicker", "{"));
        var api = new BitflyerPublicNormalizedApi(wire);

        var call = await api.GetTickerAsync(new GetTickerRequest());
        var err = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Normalized.Public.Dtos.GetTickerResponse>.Err>(call.Result);

        Assert.Equal(CallErrorKind.Codec, err.Error.Kind);
    }

    private static Call<WireCallSpec, WireResponse> CreateWireOk(string endpointId, string json, int statusCode = 200) =>
        new(
            Id: CallId.New(),
            StartedAt: DateTimeOffset.UtcNow,
            Duration: TimeSpan.Zero,
            Request: new WireCallSpec("GET", "/test", endpointId),
            Result: new CallResult<WireResponse>.Ok(new WireResponse(statusCode, json)),
            Meta: new CallMeta("Wire", "Transport", endpointId));

    private sealed class StubPublicWireApi : IBitflyerPublicWireApi
    {
        private readonly Call<WireCallSpec, WireResponse> _call;

        public StubPublicWireApi(Call<WireCallSpec, WireResponse> call)
        {
            _call = call;
        }

        public string? LastProductCode { get; private set; }

        public Task<Call<WireCallSpec, WireResponse>> GetTickerAsync(
            string? productCode = null,
            CancellationToken cancellationToken = default)
        {
            LastProductCode = productCode;
            return Task.FromResult(_call);
        }
    }
}
