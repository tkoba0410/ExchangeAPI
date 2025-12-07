using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeApi.Transport.Policy;

/// <summary>
/// 固定間隔でリクエストを整流する簡易レートリミットポリシー。
/// </summary>
public sealed class RateLimitHttpPolicy : IHttpPolicy
{
    private readonly TimeSpan _minInterval;
    private DateTimeOffset _nextAllowed;
    private readonly object _gate = new();
    private readonly Func<DateTimeOffset> _clock;

    public RateLimitHttpPolicy(double requestsPerSecond, Func<DateTimeOffset>? clock = null)
    {
        if (requestsPerSecond <= 0) throw new ArgumentOutOfRangeException(nameof(requestsPerSecond));
        _minInterval = TimeSpan.FromSeconds(1d / requestsPerSecond);
        _clock = clock ?? (() => DateTimeOffset.UtcNow);
    }

    public async Task<HttpResponseMessage> ExecuteAsync(
        HttpRequestMessage request,
        Func<CancellationToken, Task<HttpResponseMessage>> sendAsync,
        CancellationToken cancellationToken = default)
    {
        if (sendAsync is null) throw new ArgumentNullException(nameof(sendAsync));

        var delay = GetDelay();
        if (delay > TimeSpan.Zero)
        {
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        }

        return await sendAsync(cancellationToken).ConfigureAwait(false);
    }

    private TimeSpan GetDelay()
    {
        lock (_gate)
        {
            var now = _clock();
            if (_nextAllowed <= now)
            {
                _nextAllowed = now + _minInterval;
                return TimeSpan.Zero;
            }

            var delay = _nextAllowed - now;
            _nextAllowed += _minInterval;
            return delay;
        }
    }
}
