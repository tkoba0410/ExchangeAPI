using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Transport.Http;
using Xunit;

namespace ExchangeApi.Tests.Common.Tests.Transport;

public sealed class TransportConfigResolverTests
{
    [Fact]
    public void Resolve_ExternalTransport_ReturnsProvidedInstance()
    {
        var transport = new StubTransport();

        var resolved = TransportConfigResolver.Resolve(
            new Uri("https://example.com"),
            new TransportConfig.ExternalTransport(transport));

        Assert.Same(transport, resolved.Transport);
        Assert.False(resolved.DisposeTransport);
    }

    [Fact]
    public async Task Resolve_ExternalHttpClient_UsesProvidedClient()
    {
        var handler = new CountingHandler();
        using var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.com"),
        };

        var resolved = TransportConfigResolver.Resolve(
            new Uri("https://example.com"),
            new TransportConfig.ExternalHttpClient(httpClient));

        using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/ping");
        using var response = await resolved.Transport.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, handler.CallCount);
        Assert.True(resolved.DisposeTransport);
    }

    [Fact]
    public void Resolve_ManagedHttp_WithNonPositiveTimeout_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            TransportConfigResolver.Resolve(
                new Uri("https://example.com"),
                new TransportConfig.ManagedHttp(TimeSpan.Zero)));
    }

    private sealed class StubTransport : IHttpTransport
    {
        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }

    private sealed class CountingHandler : HttpMessageHandler
    {
        public int CallCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CallCount++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("ok"),
            });
        }
    }
}
