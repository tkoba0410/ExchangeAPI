using ExchangeApi.Stage10.Bitflyer.Vocabulary;
using ExchangeApi.Stage10.Bitflyer.Wire.Public.Api;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Tests.Stage10.Bitflyer.Wire.Tests;

public sealed class PublicWireApiTests
{
    [Fact]
    public async Task GetTickerAsync_WithProductCode_BuildsCanonicalRequest()
    {
        var transport = new CaptureWireTransport();
        var api = new BitflyerPublicWireApi(transport);

        var call = await api.GetTickerAsync(ProductCodes.BtcJpy);

        Assert.Equal("GET", call.Request.Method);
        Assert.Equal("/v1/getticker", call.Request.Path);
        Assert.Equal("GetTicker", call.Request.EndpointId);
        Assert.Equal($"product_code={ProductCodes.BtcJpy}", call.Request.Query);
        Assert.Null(call.Request.BodyJson);
        Assert.Null(call.Request.Headers);
    }

    [Fact]
    public async Task GetTickerAsync_WithNullProductCode_OmitsQuery()
    {
        var transport = new CaptureWireTransport();
        var api = new BitflyerPublicWireApi(transport);

        var call = await api.GetTickerAsync();

        Assert.Equal("GET", call.Request.Method);
        Assert.Equal("/v1/getticker", call.Request.Path);
        Assert.Equal("GetTicker", call.Request.EndpointId);
        Assert.Null(call.Request.Query);
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
                    Meta: new CallMeta("Wire", "Transport", request.EndpointId)));
        }
    }
}
