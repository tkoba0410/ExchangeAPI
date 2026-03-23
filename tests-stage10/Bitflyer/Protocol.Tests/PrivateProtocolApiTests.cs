using ExchangeApi.Stage10.Bitflyer.Protocol.Private.Api;
using ExchangeApi.Stage10.Bitflyer.Protocol.Private.Endpoints.CancelChildOrder;
using ExchangeApi.Stage10.Bitflyer.Protocol.Private.Endpoints.GetBalance;
using ExchangeApi.Stage10.Bitflyer.Protocol.Private.Endpoints.SendChildOrder;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Tests.Stage10.Bitflyer.Protocol.Tests;

public sealed class PrivateProtocolApiTests
{
    [Fact]
    public async Task GetBalanceCallAsync_BuildsCanonicalRequest()
    {
        var transport = new CaptureWireTransport();
        var api = CreateApi(transport);

        var call = await api.GetBalanceCallAsync();

        Assert.Equal("GET", call.Request.Method);
        Assert.Equal("/v1/me/getbalance", call.Request.Path);
        Assert.Equal("GetBalance", call.Request.EndpointId);
        Assert.Null(call.Request.Query);
        Assert.Null(call.Request.BodyJson);
        Assert.Null(call.Request.Headers);
    }

    [Fact]
    public async Task SendChildOrderCallAsync_BuildsCanonicalRequest()
    {
        const string bodyJson = "{\"product_code\":\"BTC_JPY\"}";
        var transport = new CaptureWireTransport();
        var api = CreateApi(transport);

        var call = await api.SendChildOrderCallAsync(bodyJson);

        Assert.Equal("POST", call.Request.Method);
        Assert.Equal("/v1/me/sendchildorder", call.Request.Path);
        Assert.Equal("SendChildOrder", call.Request.EndpointId);
        Assert.Null(call.Request.Query);
        Assert.Equal(bodyJson, call.Request.BodyJson);
        Assert.Null(call.Request.Headers);
    }

    [Fact]
    public async Task CancelChildOrderCallAsync_BuildsCanonicalRequest()
    {
        const string bodyJson = "{\"product_code\":\"BTC_JPY\",\"child_order_acceptance_id\":\"JRF-1\"}";
        var transport = new CaptureWireTransport();
        var api = CreateApi(transport);

        var call = await api.CancelChildOrderCallAsync(bodyJson);

        Assert.Equal("POST", call.Request.Method);
        Assert.Equal("/v1/me/cancelchildorder", call.Request.Path);
        Assert.Equal("CancelChildOrder", call.Request.EndpointId);
        Assert.Null(call.Request.Query);
        Assert.Equal(bodyJson, call.Request.BodyJson);
        Assert.Null(call.Request.Headers);
    }

    private sealed class CaptureWireTransport : IWireTransport
    {
        public Task<Call<WireCallSpec, WireResponse>> SendAsync(
            WireCallSpec request,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(
                new Call<WireCallSpec, WireResponse>(
                    Id: CallId.New(),
                    StartedAt: DateTimeOffset.UtcNow,
                    Duration: TimeSpan.Zero,
                    Request: request,
                    Result: new CallResult<WireResponse>.Ok(new WireResponse(200, "{}")),
                    Meta: new CallMeta("Protocol", "Transport", request.EndpointId)));
        }
    }

    private static BitflyerPrivateProtocolApi CreateApi(CaptureWireTransport transport) =>
        new(
            new GetBalanceProtocolEndpoint(transport),
            new SendChildOrderProtocolEndpoint(transport),
            new CancelChildOrderProtocolEndpoint(transport));
}
