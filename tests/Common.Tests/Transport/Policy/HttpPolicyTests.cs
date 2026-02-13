using System;
using System.Collections.Generic;
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
        var policy = new RetryHttpPolicy(
            maxAttemptsForGet: 3,
            maxAttemptsForOther: 2,
            TimeSpan.FromMilliseconds(1),
            TimeSpan.FromMilliseconds(5));
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

        await Assert.ThrowsAsync<TimeoutException>(() =>
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
        var policy = new RetryHttpPolicy(
            maxAttemptsForGet: 3,
            maxAttemptsForOther: 2,
            TimeSpan.FromMilliseconds(1),
            TimeSpan.FromMilliseconds(5));
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
    public async Task Retry_Uses_RetryAfter_For429()
    {
        var observer = new RecordingPolicyObserver();
        var policy = new RetryHttpPolicy(
            maxAttemptsForGet: 3,
            maxAttemptsForOther: 1,
            baseDelay: TimeSpan.FromMilliseconds(1),
            maxDelay: TimeSpan.FromMilliseconds(2),
            observer: observer);
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        var attempts = 0;

        var response = await policy.ExecuteAsync(
            request,
            _ =>
            {
                attempts++;
                if (attempts == 1)
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.TooManyRequests)
                    {
                        Headers = { RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.FromMilliseconds(25)) }
                    });
                }

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single(observer.RetryDelays);
        Assert.Equal(TimeSpan.FromMilliseconds(25), observer.RetryDelays[0]);
    }

    [Fact]
    public async Task Retry_Backoff_Uses_Exponential_With_Jitter_And_TotalTimeCap()
    {
        var observer = new RecordingPolicyObserver();
        var policy = new RetryHttpPolicy(
            maxAttemptsForGet: 4,
            maxAttemptsForOther: 1,
            baseDelay: TimeSpan.FromMilliseconds(40),
            maxDelay: TimeSpan.FromMilliseconds(200),
            observer: observer,
            maxTotalRetryTime: TimeSpan.FromMilliseconds(60),
            nextJitter: () => 0.5d);
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        var attempts = 0;

        var response = await policy.ExecuteAsync(
            request,
            _ =>
            {
                attempts++;
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError));
            });

        Assert.Equal(2, attempts);
        Assert.Single(observer.RetryDelays);
        Assert.Equal(TimeSpan.FromMilliseconds(40), observer.RetryDelays[0]);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
    }

    [Fact]
    public async Task Retry_Backoff_Delay_Grows_When_NoRetryAfter()
    {
        var observer = new RecordingPolicyObserver();
        var policy = new RetryHttpPolicy(
            maxAttemptsForGet: 3,
            maxAttemptsForOther: 1,
            baseDelay: TimeSpan.FromMilliseconds(20),
            maxDelay: TimeSpan.FromMilliseconds(500),
            observer: observer,
            nextJitter: () => 0.0d);
        var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com");
        var attempts = 0;

        var response = await policy.ExecuteAsync(
            request,
            _ =>
            {
                attempts++;
                if (attempts < 3)
                {
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.TooManyRequests));
                }

                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, observer.RetryDelays.Count);
        Assert.Equal(TimeSpan.FromMilliseconds(16), observer.RetryDelays[0]); // 20ms * 0.8
        Assert.Equal(TimeSpan.FromMilliseconds(32), observer.RetryDelays[1]); // 40ms * 0.8
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

    private sealed class RecordingPolicyObserver : IPolicyObserver
    {
        public List<TimeSpan> RetryDelays { get; } = new();

        public void OnRetry(HttpRequestMessage request, int attempt, int maxAttempts, TimeSpan delay, Exception? lastException, HttpResponseMessage? lastResponse)
            => RetryDelays.Add(delay);

        public void OnRateLimitDelay(TimeSpan delay) { }
        public void OnCircuitOpened(TimeSpan openDuration) { }
        public void OnCircuitRejected() { }
    }
}
