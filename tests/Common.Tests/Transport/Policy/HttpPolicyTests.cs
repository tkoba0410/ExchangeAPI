using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using Xunit;

namespace ExchangeApi.Tests.Common.Tests.Transport.Policy;

public class HttpPolicyTests
{
    [Fact]
    public async Task Retry_Retries_OnTransientException()
    {
        var policy = new RetryHttpPolicy(maxAttemptsForGet: 3, maxAttemptsForOther: 2, TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(5));
        var attempts = 0;
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");

        var response = await policy.ExecuteAsync(
            request,
            _ =>
            {
                attempts++;
                if (attempts < 3) throw new HttpRequestException("boom");
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            });

        Assert.Equal(3, attempts);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CircuitBreaker_Opens_OnConsecutiveFailures()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        var policy = new CircuitBreakerHttpPolicy(failureThreshold: 1, openDuration: TimeSpan.FromMilliseconds(50), clock: () => DateTimeOffset.UtcNow);

        await Assert.ThrowsAsync<HttpRequestException>(() =>
            policy.ExecuteAsync(request, _ => throw new HttpRequestException("fail")));

        var openEx = await Assert.ThrowsAsync<HttpRequestException>(() =>
            policy.ExecuteAsync(request, _ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK))));

        Assert.Equal(HttpStatusCode.ServiceUnavailable, openEx.StatusCode);
    }

    [Fact]
    public async Task Timeout_Cancels_LongRunningRequest()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        var policy = new TimeoutHttpPolicy(TimeSpan.FromMilliseconds(10));

        await Assert.ThrowsAsync<TaskCanceledException>(() =>
            policy.ExecuteAsync(
                request,
                async cancellationToken =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                    return new HttpResponseMessage(HttpStatusCode.OK);
                },
                CancellationToken.None));
    }

    [Fact]
    public async Task Retry_Uses_ErrorCategory_ForDecision()
    {
        var policy = new RetryHttpPolicy(maxAttemptsForGet: 3, maxAttemptsForOther: 2, TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(5));
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        var attempts = 0;

        await Assert.ThrowsAsync<TransportException>(() =>
            policy.ExecuteAsync(
                request,
                _ =>
                {
                    attempts++;
                    throw new TransportException("bad request", errorCategory: TransportErrorCategory.Request);
                }));

        Assert.Equal(1, attempts); // no retry on Request category

        attempts = 0;
        var response = await policy.ExecuteAsync(
            request,
            _ =>
            {
                attempts++;
                if (attempts == 1)
                {
                    throw new TransportException("rate limit", errorCategory: TransportErrorCategory.RateLimit);
                }
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, attempts); // retried once
    }

    [Fact]
    public async Task RateLimit_AllowsBurstThenDelays()
    {
        var policy = new RateLimitHttpPolicy(requestsPerSecond: 2, burst: 2);
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        var sequence = new System.Collections.Generic.List<TimeSpan>();
        var sw = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 0; i < 3; i++)
        {
            await policy.ExecuteAsync(
                request,
                _ =>
                {
                    sequence.Add(sw.Elapsed);
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
                });
        }

        Assert.Equal(3, sequence.Count);
        // 最初の2回はほぼ同時、3回目は0.5s以上後（2 rps）を想定
        Assert.True((sequence[1] - sequence[0]) < TimeSpan.FromMilliseconds(50));
        Assert.True((sequence[2] - sequence[1]) >= TimeSpan.FromMilliseconds(450));
    }
}
