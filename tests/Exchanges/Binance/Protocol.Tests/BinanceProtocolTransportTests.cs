using System.Net;
using ExchangeApi.Exchanges.Binance.Protocol.Internal.Runtime;
using ExchangeApi.Exchanges.Binance.Protocol.Internal.Shared;
using ExchangeApi.Primitives.Calls;
using ExchangeApi.Primitives.Protocol;

namespace ExchangeApi.Tests.Exchanges.Binance.Protocol.Tests;

public sealed class BinanceProtocolTransportTests
{
    [Fact]
    public async Task SendAsync_ResolvesAbsoluteRequestUri_FromBaseUri()
    {
        var handler = new RecordingHandler();
        using var httpClient = new HttpClient(handler);
        var transport = new BinanceProtocolTransport(
            httpClient,
            new Uri("https://api.binance.com"),
            new NoOpProtocolDebugLogger());

        var result = await transport.SendAsync(new ProtocolRequest
        {
            EndpointId = "GetKlines",
            Method = "GET",
            Path = "/api/v3/klines",
            Query = new Dictionary<string, string>
            {
                ["interval"] = "1h",
                ["symbol"] = "BTCJPY",
            },
        }, ProtocolTransportAuthMode.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(new Uri("https://api.binance.com/api/v3/klines?interval=1h&symbol=BTCJPY"), handler.LastRequestUri);
    }

    [Fact]
    public async Task SendAsync_WithRequestTimeout_ReturnsTransportTimeout()
    {
        var handler = new PendingHandler();
        using var httpClient = new HttpClient(handler);
        var requestTimeout = TimeSpan.FromMilliseconds(50);
        var transport = new BinanceProtocolTransport(
            httpClient,
            new Uri("https://api.binance.com"),
            new NoOpProtocolDebugLogger(),
            requestTimeout);

        var result = await transport.SendAsync(new ProtocolRequest
        {
            EndpointId = "GetKlines",
            Method = "GET",
            Path = "/api/v3/klines",
        }, ProtocolTransportAuthMode.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(CallErrorKinds.Transport, result.Error!.Kind);
        Assert.Equal($"Request timed out after {requestTimeout:c}.", result.Error.Message);
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        public Uri? LastRequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequestUri = request.RequestUri;

            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]"),
            });
        }
    }

    private sealed class PendingHandler : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
