using ExchangeApi.Exchanges.Binance.Composition.Factory;
using System.Net;

namespace ExchangeApi.Tests.Exchanges.Binance.Composition.Tests;

public sealed class BinanceClientFactoryTests
{
    [Fact]
    public void CreateProtocolClient_HasPublic()
    {
        var bundle = BinanceClientFactory.CreateProtocolClient();

        Assert.NotNull(bundle.Public);
    }

    [Fact]
    public void CreateNativeClient_WiresProtocolAndNative()
    {
        var bundle = BinanceClientFactory.CreateNativeClient();

        Assert.NotNull(bundle.Public);
        Assert.NotNull(bundle.Protocol);
        Assert.NotNull(bundle.Protocol.Public);
    }

    [Fact]
    public async Task CreateProtocolClient_WithExternalHttpClient_DoesNotDisposeCallerClient()
    {
        var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler);
        var bundle = BinanceClientFactory.CreateProtocolClient(httpClient);

        bundle.Dispose();
        bundle.Dispose();

        using var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://example.com/health"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, handler.RequestCount);
    }

    [Fact]
    public async Task CreateNativeClient_WithExternalHttpClient_DoesNotDisposeCallerClient_WhenNestedProtocolIsDisposed()
    {
        var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler);
        var bundle = BinanceClientFactory.CreateNativeClient(httpClient);

        bundle.Protocol.Dispose();
        bundle.Dispose();
        bundle.Dispose();

        using var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://example.com/health"));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, handler.RequestCount);
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public int RequestCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestCount++;

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("ok"),
            });
        }
    }
}
