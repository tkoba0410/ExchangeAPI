using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Core.Transport.Logging;
using Core.Transport.Policy;
using Core.Transport.Protocol;
using Core.Transport.Transport;
using Composition.Factory.Transport;

namespace Composition.Factory.Tests.Transport;

public class RestClientFactory_Tests
{
    private sealed record TestDto(string Value);

    [Fact]
    public async Task Create_AppliesSignerPolicyAndLogger()
    {
        var baseUri = new Uri("https://example.com");
        var transport = new FakeTransport();
        var signer = new FakeSigner();
        var policy = new FakePolicy();
        var logger = new FakeLogger();

        var client = RestClientFactory.Create(baseUri, transport, signer, policy, logger);

        var result = await client.GetAsync<TestDto>("/api");

        Assert.Equal("ok", result.Value);
        Assert.True(signer.Called);
        Assert.True(policy.Called);
        Assert.True(logger.RequestLogged);
        Assert.True(logger.ResponseLogged);
    }

    private sealed class FakeTransport : IHttpTransport
    {
        public HttpRequestMessage? LastRequest { get; private set; }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            LastRequest = request;
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"value\":\"ok\"}")
            };
            return Task.FromResult(response);
        }
    }

    private sealed class FakeSigner : IRequestSigner
    {
        public bool Called { get; private set; }

        public Task SignAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            Called = true;
            request.Headers.Add("X-Signed", "1");
            return Task.CompletedTask;
        }
    }

    private sealed class FakePolicy : IHttpPolicy
    {
        public bool Called { get; private set; }

        public Task<HttpResponseMessage> ExecuteAsync(
            HttpRequestMessage request,
            Func<CancellationToken, Task<HttpResponseMessage>> sendAsync,
            CancellationToken cancellationToken = default)
        {
            Called = true;
            return sendAsync(cancellationToken);
        }
    }

    private sealed class FakeLogger : IRestClientLogger
    {
        public bool RequestLogged { get; private set; }
        public bool ResponseLogged { get; private set; }

        public void LogRequest(HttpRequestMessage request) => RequestLogged = true;

        public void LogResponse(HttpResponseMessage response, string content) => ResponseLogged = true;

        public void LogError(Exception exception, HttpRequestMessage request) { }
    }
}
