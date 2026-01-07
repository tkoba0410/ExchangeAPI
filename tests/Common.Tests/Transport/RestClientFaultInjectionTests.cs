using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Transport.Policy;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Core.Transport.Http;
using Xunit;

namespace ExchangeApi.Core.Transport.Tests;

public class RestClientFaultInjectionTests
{
    [Fact]
    public async Task RestClient_Retries_On429_ThenSucceeds()
    {
        var transport = new SequenceTransport(
            new HttpResponseMessage((HttpStatusCode)429)
            {
                Content = new StringContent("{\"error_message\":\"too many\"}")
            },
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"Value\":\"ok\"}")
            });

        var options = new HttpPolicyOptions
        {
            MaxRetryAttemptsForGet = 3,
            MaxRetryAttemptsForOther = 2,
            RetryBaseDelay = TimeSpan.FromMilliseconds(1),
            RetryMaxDelay = TimeSpan.FromMilliseconds(5),
            Timeout = TimeSpan.FromSeconds(2),
            RequestsPerSecond = 100
        };

        var rest = new RestClient(
            new Uri("https://example.com"),
            transport,
            policy: HttpPolicyFactory.CreateDefault(options));

        var result = await rest.GetAsync<TestDto>("/api");

        Assert.Equal("ok", result.Value);
        Assert.Equal(2, transport.SendCount);
    }

    [Fact]
    public async Task RestClient_Retries_OnTransientException()
    {
        var transport = new ThrowThenSucceedTransport(
            new HttpRequestException("transient", null, HttpStatusCode.BadGateway),
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"Value\":\"ok\"}")
            });

        var options = new HttpPolicyOptions
        {
            MaxRetryAttemptsForGet = 3,
            RetryBaseDelay = TimeSpan.FromMilliseconds(1),
            RetryMaxDelay = TimeSpan.FromMilliseconds(5),
            Timeout = TimeSpan.FromSeconds(2),
            RequestsPerSecond = 100
        };

        var rest = new RestClient(
            new Uri("https://example.com"),
            transport,
            policy: HttpPolicyFactory.CreateDefault(options));

        var result = await rest.GetAsync<TestDto>("/api");

        Assert.Equal("ok", result.Value);
        Assert.Equal(2, transport.SendCount);
    }

    [Fact]
    public async Task RestClient_TimesOut_OnSlowResponse()
    {
        var transport = new DelayTransport(TimeSpan.FromMilliseconds(200));
        var options = new HttpPolicyOptions
        {
            Timeout = TimeSpan.FromMilliseconds(50),
            RequestsPerSecond = 100,
            RetryBaseDelay = TimeSpan.FromMilliseconds(1),
            RetryMaxDelay = TimeSpan.FromMilliseconds(5),
            MaxRetryAttemptsForGet = 1,
            MaxRetryAttemptsForOther = 1,
            CircuitBreakerFailureThreshold = 10
        };

        var rest = new RestClient(
            new Uri("https://example.com"),
            transport,
            policy: HttpPolicyFactory.CreateDefault(options));

        await Assert.ThrowsAsync<TransportException>(() => rest.GetAsync<TestDto>("/api"));
        Assert.Equal(1, transport.SendCount);
    }

    [Fact]
    public async Task RestClient_CircuitBreaker_Opens_AfterFailures()
    {
        var transport = new SequenceTransport(
            new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent("server error")
            });

        var clock = new TestClock();
        var policy = new HttpPolicyPipeline(
            new RateLimitHttpPolicy(100),
            new CircuitBreakerHttpPolicy(failureThreshold: 1, openDuration: TimeSpan.FromSeconds(10), clock: clock.Now),
            new TimeoutHttpPolicy(TimeSpan.FromSeconds(2)));

        var rest = new RestClient(
            new Uri("https://example.com"),
            transport,
            policy: policy);

        var ex1 = await Assert.ThrowsAsync<TransportException>(() => rest.GetAsync<TestDto>("/api"));
        Assert.Equal(HttpStatusCode.InternalServerError, ex1.StatusCode);

        var ex2 = await Assert.ThrowsAsync<TransportException>(() => rest.GetAsync<TestDto>("/api"));
        Assert.Equal(HttpStatusCode.ServiceUnavailable, ex2.StatusCode); // CB opens
        Assert.Equal(1, transport.SendCount); // second call short-circuited
    }

    private sealed record TestDto(string Value);

    private sealed class SequenceTransport : IHttpTransport
    {
        private readonly HttpResponseMessage[] _responses;
        private int _index;

        public int SendCount => _index;

        public SequenceTransport(params HttpResponseMessage[] responses)
        {
            _responses = responses;
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            if (_index >= _responses.Length)
            {
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"Value\":\"ok\"}")
                });
            }

            var response = _responses[_index++];
            return Task.FromResult(response);
        }
    }

    private sealed class ThrowThenSucceedTransport : IHttpTransport
    {
        private readonly Exception _first;
        private readonly HttpResponseMessage _second;
        private int _count;
        public int SendCount => _count;

        public ThrowThenSucceedTransport(Exception first, HttpResponseMessage second)
        {
            _first = first;
            _second = second;
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            _count++;
            if (_count == 1)
            {
                throw _first;
            }

            return Task.FromResult(_second);
        }
    }

    private sealed class DelayTransport : IHttpTransport
    {
        private readonly TimeSpan _delay;
        public int SendCount { get; private set; }

        public DelayTransport(TimeSpan delay)
        {
            _delay = delay;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            SendCount++;
            await Task.Delay(_delay, cancellationToken);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"Value\":\"ok\"}")
            };
        }
    }

    private sealed class TestClock
    {
        private DateTimeOffset _now = DateTimeOffset.UtcNow;
        public DateTimeOffset Now() => _now;
        public void Advance(TimeSpan span) => _now = _now.Add(span);
    }
}
