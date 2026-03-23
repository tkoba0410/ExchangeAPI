using ExchangeApi.Stage10.Bitflyer.Native.Private.Api;
using ExchangeApi.Stage10.Bitflyer.Native.Private.Requests;
using ExchangeApi.Stage10.Bitflyer.Protocol.Private.Api;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Tests.Stage10.Bitflyer.Native.Tests;

public sealed class GetBalanceNativeTests
{
    [Fact]
    public async Task GetBalanceAsync_SuccessfullyMapsTopLevelArray()
    {
        var protocol = new StubPrivateProtocolApi(
            getBalanceCall: CreateWireOk(
                "GetBalance",
                """
                [
                  { "currency_code": "JPY", "amount": 1000, "available": 900 },
                  { "currency_code": "BTC", "amount": 0.5, "available": 0.4 }
                ]
                """),
            sendChildOrderCall: CreateWireOk("SendChildOrder", "{}"),
            cancelChildOrderCall: CreateWireOk("CancelChildOrder", ""));
        var api = new BitflyerPrivateNativeApi(protocol);

        var call = await api.GetBalanceAsync(new GetBalanceRequest());
        var ok = Assert.IsType<CallResult<IReadOnlyList<ExchangeApi.Stage10.Bitflyer.Native.Private.Dtos.GetBalance.Item>>.Ok>(call.Result);

        Assert.Collection(
            ok.Response,
            item =>
            {
                Assert.Equal("JPY", item.CurrencyCode);
                Assert.Equal(1000m, item.Amount);
                Assert.Equal(900m, item.Available);
            },
            item =>
            {
                Assert.Equal("BTC", item.CurrencyCode);
                Assert.Equal(0.5m, item.Amount);
                Assert.Equal(0.4m, item.Available);
            });
    }

    [Fact]
    public async Task GetBalanceAsync_WithNonArrayJson_ReturnsCodecError()
    {
        var protocol = new StubPrivateProtocolApi(
            getBalanceCall: CreateWireOk("GetBalance", """{ "currency_code": "JPY" }"""),
            sendChildOrderCall: CreateWireOk("SendChildOrder", "{}"),
            cancelChildOrderCall: CreateWireOk("CancelChildOrder", ""));
        var api = new BitflyerPrivateNativeApi(protocol);

        var call = await api.GetBalanceAsync(new GetBalanceRequest());
        var err = Assert.IsType<CallResult<IReadOnlyList<ExchangeApi.Stage10.Bitflyer.Native.Private.Dtos.GetBalance.Item>>.Err>(call.Result);

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

    private sealed class StubPrivateProtocolApi : IBitflyerPrivateProtocolApi
    {
        private readonly Call<WireCallSpec, WireResponse> _getBalanceCall;
        private readonly Call<WireCallSpec, WireResponse> _sendChildOrderCall;
        private readonly Call<WireCallSpec, WireResponse> _cancelChildOrderCall;

        public StubPrivateProtocolApi(
            Call<WireCallSpec, WireResponse> getBalanceCall,
            Call<WireCallSpec, WireResponse> sendChildOrderCall,
            Call<WireCallSpec, WireResponse> cancelChildOrderCall)
        {
            _getBalanceCall = getBalanceCall;
            _sendChildOrderCall = sendChildOrderCall;
            _cancelChildOrderCall = cancelChildOrderCall;
        }

        public Task<Call<WireCallSpec, WireResponse>> GetBalanceAsync(
            CancellationToken cancellationToken = default) =>
            Task.FromResult(_getBalanceCall);

        public Task<Call<WireCallSpec, WireResponse>> SendChildOrderAsync(
            string bodyJson,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(_sendChildOrderCall);

        public Task<Call<WireCallSpec, WireResponse>> CancelChildOrderAsync(
            string bodyJson,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(_cancelChildOrderCall);
    }
}
