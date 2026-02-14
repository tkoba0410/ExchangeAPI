using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Transport.Protocol;
namespace ExchangeApi.Transport.Policy;

/// <summary>
/// 一時障害やレートリミットに対してリトライを行うポリシー。
/// </summary>
public sealed class RetryHttpPolicy : IHttpPolicy
{
    private readonly int _maxAttemptsForGet;
    private readonly int _maxAttemptsForOther;
    private readonly TimeSpan _baseDelay;
    private readonly TimeSpan _maxDelay;
    private readonly TimeSpan? _maxTotalRetryTime;
    private readonly Func<double> _nextJitter;
    private readonly Func<DateTimeOffset> _clock;
    private readonly IPolicyObserver _observer;

    public RetryHttpPolicy(
        int maxAttemptsForGet,
        int maxAttemptsForOther,
        TimeSpan baseDelay,
        TimeSpan maxDelay,
        IPolicyObserver? observer = null,
        TimeSpan? maxTotalRetryTime = null,
        Func<double>? nextJitter = null,
        Func<DateTimeOffset>? clock = null)
    {
        if (maxAttemptsForGet < 1) throw new ArgumentOutOfRangeException(nameof(maxAttemptsForGet));
        if (maxAttemptsForOther < 1) throw new ArgumentOutOfRangeException(nameof(maxAttemptsForOther));
        if (baseDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(baseDelay));
        if (maxDelay < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(maxDelay));
        if (maxTotalRetryTime is { } maxTotal && maxTotal <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(maxTotalRetryTime));

        _maxAttemptsForGet = maxAttemptsForGet;
        _maxAttemptsForOther = maxAttemptsForOther;
        _baseDelay = baseDelay;
        _maxDelay = maxDelay;
        _maxTotalRetryTime = maxTotalRetryTime;
        _nextJitter = nextJitter ?? Random.Shared.NextDouble;
        _clock = clock ?? (() => DateTimeOffset.UtcNow);
        _observer = observer ?? NoOpPolicyObserver.Instance;
    }

    public async Task<HttpResponseMessage> ExecuteAsync(
        HttpRequestMessage request,
        Func<CancellationToken, Task<HttpResponseMessage>> sendAsync,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (sendAsync is null) throw new ArgumentNullException(nameof(sendAsync));

        var maxAttempts = GetMaxAttempts(request.Method);
        var startedAt = _clock();

        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                var response = await sendAsync(cancellationToken).ConfigureAwait(false);

                if (ShouldRetryResponse(response)
                    && attempt < maxAttempts
                    && TryGetRetryDelay(request, response, attempt, startedAt, out var responseDelay))
                {
                    _observer.OnRetry(request, attempt, maxAttempts, responseDelay, null, response);
                    response.Dispose();
                    await Task.Delay(responseDelay, cancellationToken).ConfigureAwait(false);
                    continue;
                }

                return response;
            }
            catch (Exception ex) when (
                ShouldRetryException(ex, cancellationToken)
                && attempt < maxAttempts
                && TryGetRetryDelay(request, null, attempt, startedAt, out var exceptionDelay))
            {
                _observer.OnRetry(request, attempt, maxAttempts, exceptionDelay, ex, null);
                await Task.Delay(exceptionDelay, cancellationToken).ConfigureAwait(false);
                continue;
            }
        }

        throw new InvalidOperationException("Retry policy exhausted without response.");
    }

    private int GetMaxAttempts(HttpMethod method)
    {
        return method == HttpMethod.Get ? _maxAttemptsForGet : _maxAttemptsForOther;
    }

    private static bool ShouldRetryResponse(HttpResponseMessage response)
    {
        if (response is null) return false;

        var status = response.StatusCode;
        return status == HttpStatusCode.TooManyRequests
               || status == HttpStatusCode.RequestTimeout
               || status == HttpStatusCode.GatewayTimeout
               || (int)status >= 500;
    }

    private static bool ShouldRetryException(Exception exception, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested) return false;

        return exception switch
        {
            HttpRequestException => true,
            TimeoutException => true,
            TaskCanceledException => true,
            TransportException apiEx => apiEx.ErrorCategory is TransportErrorCategory.RateLimit
                or TransportErrorCategory.Network
                or TransportErrorCategory.Server,
            _ => false
        };
    }

    private bool TryGetRetryDelay(
        HttpRequestMessage request,
        HttpResponseMessage? response,
        int attempt,
        DateTimeOffset startedAt,
        out TimeSpan delay)
    {
        delay = response is not null
            ? GetDelayFromResponseOrBackoff(response, attempt)
            : GetExponentialBackoffDelay(attempt);

        if (delay < TimeSpan.Zero)
        {
            delay = TimeSpan.Zero;
        }

        if (_maxTotalRetryTime is null)
        {
            return true;
        }

        var elapsed = _clock() - startedAt;
        if (elapsed + delay <= _maxTotalRetryTime.Value)
        {
            return true;
        }

        return false;
    }

    private TimeSpan GetDelayFromResponseOrBackoff(HttpResponseMessage response, int attempt)
    {
        if (response.StatusCode == HttpStatusCode.TooManyRequests
            && TryGetRetryAfterDelay(response, out var retryAfter))
        {
            return retryAfter;
        }

        return GetExponentialBackoffDelay(attempt);
    }

    private TimeSpan GetExponentialBackoffDelay(int attempt)
    {
        var delay = TimeSpan.FromMilliseconds(_baseDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));
        var jitterFactor = 0.8d + (_nextJitter() * 0.4d); // 80% - 120%
        delay = TimeSpan.FromMilliseconds(delay.TotalMilliseconds * jitterFactor);
        if (delay > _maxDelay) delay = _maxDelay;
        return delay;
    }

    private static bool TryGetRetryAfterDelay(HttpResponseMessage response, out TimeSpan delay)
    {
        delay = TimeSpan.Zero;
        var retryAfter = response.Headers.RetryAfter;
        if (retryAfter is null)
        {
            return false;
        }

        if (retryAfter.Delta is { } delta)
        {
            if (delta < TimeSpan.Zero)
            {
                return false;
            }

            delay = delta;
            return true;
        }

        if (retryAfter.Date is { } date)
        {
            var remaining = date - DateTimeOffset.UtcNow;
            if (remaining < TimeSpan.Zero)
            {
                delay = TimeSpan.Zero;
                return true;
            }

            delay = remaining;
            return true;
        }

        return false;
    }
}
