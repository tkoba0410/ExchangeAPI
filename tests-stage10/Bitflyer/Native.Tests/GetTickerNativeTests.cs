using ExchangeApi.Stage10.Bitflyer.Vocabulary;
using ExchangeApi.Stage10.Bitflyer.Native.Public.Endpoints.GetTicker;
using ExchangeApi.Stage10.Bitflyer.Native.Public.Requests;
using ExchangeApi.Stage10.Bitflyer.Protocol.Public.Endpoints.GetTicker;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Tests.Stage10.Bitflyer.Native.Tests;

public sealed class GetTickerNativeTests
{
    [Fact]
    public async Task GetTickerCallAsync_SuccessfullyMapsResponse()
    {
        var protocol = new StubGetTickerProtocolEndpoint(CreateWireOk(
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
        var endpoint = new GetTickerNativeEndpoint(protocol);

        var call = await endpoint.CallAsync(new GetTickerRequest { ProductCode = ProductCodes.BtcJpy });
        var ok = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Native.Public.Dtos.GetTickerResponse>.Ok>(call.Result);

        Assert.Equal(ProductCodes.BtcJpy, ok.Response.ProductCode);
        Assert.Equal("RUNNING", ok.Response.State);
        Assert.Equal(100.5m, ok.Response.Ltp);
        Assert.Equal(ProductCodes.BtcJpy, protocol.LastProductCode);
    }

    [Fact]
    public async Task GetTickerCallAsync_WithEmptyProductCode_ReturnsSemanticErrorWithoutCallingProtocol()
    {
        var protocol = new StubGetTickerProtocolEndpoint(CreateWireOk("GetTicker", "{}"));
        var endpoint = new GetTickerNativeEndpoint(protocol);

        var call = await endpoint.CallAsync(new GetTickerRequest { ProductCode = "" });
        var err = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Native.Public.Dtos.GetTickerResponse>.Err>(call.Result);

        Assert.Equal(CallErrorKind.Semantic, err.Error.Kind);
        Assert.Null(protocol.LastProductCode);
    }

    [Fact]
    public async Task GetTickerCallAsync_WithHttpFailure_ReturnsHttpError()
    {
        var protocol = new StubGetTickerProtocolEndpoint(CreateWireOk("GetTicker", "{\"status\":\"ng\"}", statusCode: 400));
        var endpoint = new GetTickerNativeEndpoint(protocol);

        var call = await endpoint.CallAsync(new GetTickerRequest());
        var err = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Native.Public.Dtos.GetTickerResponse>.Err>(call.Result);

        Assert.Equal(CallErrorKind.Http, err.Error.Kind);
        Assert.Equal(400, err.Error.HttpStatus);
        Assert.Contains("\"status\":\"ng\"", err.Error.BodySnippet);
    }

    [Fact]
    public async Task GetTickerCallAsync_WithInvalidJson_ReturnsCodecError()
    {
        var protocol = new StubGetTickerProtocolEndpoint(CreateWireOk("GetTicker", "{"));
        var endpoint = new GetTickerNativeEndpoint(protocol);

        var call = await endpoint.CallAsync(new GetTickerRequest());
        var err = Assert.IsType<CallResult<ExchangeApi.Stage10.Bitflyer.Native.Public.Dtos.GetTickerResponse>.Err>(call.Result);

        Assert.Equal(CallErrorKind.Codec, err.Error.Kind);
    }

    private static Call<WireCallSpec, WireResponse> CreateWireOk(string endpointId, string json, int statusCode = 200) =>
        new(
            Id: CallId.New(),
            StartedAt: DateTimeOffset.UtcNow,
            Duration: TimeSpan.Zero,
            Request: new WireCallSpec("GET", "/test", endpointId),
            Result: new CallResult<WireResponse>.Ok(new WireResponse(statusCode, json)),
            Meta: new CallMeta("Protocol", "Transport", endpointId));

    private sealed class StubGetTickerProtocolEndpoint : IGetTickerProtocolEndpoint
    {
        private readonly Call<WireCallSpec, WireResponse> _call;

        public StubGetTickerProtocolEndpoint(Call<WireCallSpec, WireResponse> call)
        {
            _call = call;
        }

        public string? LastProductCode { get; private set; }

        public Task<Call<WireCallSpec, WireResponse>> SendAsync(
            string? productCode = null,
            CancellationToken cancellationToken = default)
        {
            LastProductCode = productCode;
            return Task.FromResult(_call);
        }
    }
}
