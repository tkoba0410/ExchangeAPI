using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Shared.Transport.Http;
using ExchangeApi.Shared.Transport.Protocol;

namespace ExchangeApi.Tests.Core.Tests;

public sealed class RestClientSendRawAsyncTests
{
    [Fact]
    public async Task SendRawAsync_does_not_throw_on_500()
    {
        var transport = new StubTransport((_, _) => Task.FromResult(CreateResponse(
            HttpStatusCode.InternalServerError,
            "{\"error\":\"x\"}")));
        var client = CreateClient(transport);

        var result = await client.SendRawAsync("GET", "/v1/test");

        Assert.Equal(500, result.StatusCode);
        Assert.Equal("{\"error\":\"x\"}", result.Body);
    }

    [Fact]
    public async Task SendRawAsync_does_not_throw_on_404()
    {
        var transport = new StubTransport((_, _) => Task.FromResult(CreateResponse(
            HttpStatusCode.NotFound,
            "{\"error\":\"missing\"}")));
        var client = CreateClient(transport);

        var result = await client.SendRawAsync("GET", "/v1/missing");

        Assert.Equal(404, result.StatusCode);
        Assert.Equal("{\"error\":\"missing\"}", result.Body);
    }

    [Fact]
    public async Task SendRawAsync_propagates_cancellation()
    {
        var transport = new StubTransport((_, ct) =>
        {
            ct.ThrowIfCancellationRequested();
            return Task.FromResult(CreateResponse(HttpStatusCode.OK, "{}"));
        });
        var client = CreateClient(transport);
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            client.SendRawAsync("GET", "/v1/test", cancellationToken: cts.Token));
    }

    private static RestClient CreateClient(IHttpTransport transport) =>
        new(new Uri("https://example.test"), transport);

    private static HttpResponseMessage CreateResponse(HttpStatusCode statusCode, string body) =>
        new(statusCode)
        {
            Content = new StringContent(body, Encoding.UTF8, "application/json"),
        };

    private sealed class StubTransport : IHttpTransport
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handler;

        public StubTransport(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler)
        {
            _handler = handler;
        }

        public Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken = default) =>
            _handler(request, cancellationToken);
    }
}
