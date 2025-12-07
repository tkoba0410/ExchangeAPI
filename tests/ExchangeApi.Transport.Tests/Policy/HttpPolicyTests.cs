using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Errors;
using ExchangeApi.Transport.Policy;
using Xunit;

namespace ExchangeApi.Transport.Tests.Policy;

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
                async ct =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), ct);
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

        await Assert.ThrowsAsync<ExchangeApiException>(() =>
            policy.ExecuteAsync(
                request,
                _ =>
                {
                    attempts++;
                    throw new ExchangeApiException("bad request", errorCategory: ExchangeErrorCategory.Request);
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
                    throw new ExchangeApiException("rate limit", errorCategory: ExchangeErrorCategory.RateLimit);
                }
                return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, attempts); // retried once
    }
}
