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
    private readonly int _burst;
    private double _tokens;
    private DateTimeOffset _lastRefill;
    private readonly object _gate = new();
    private readonly Func<DateTimeOffset> _clock;
    private readonly IPolicyObserver _observer;

    public RateLimitHttpPolicy(double requestsPerSecond, int burst = 1, Func<DateTimeOffset>? clock = null, IPolicyObserver? observer = null)
    {
        if (requestsPerSecond <= 0) throw new ArgumentOutOfRangeException(nameof(requestsPerSecond));
        if (burst < 1) throw new ArgumentOutOfRangeException(nameof(burst));
        _minInterval = TimeSpan.FromSeconds(1d / requestsPerSecond);
        _burst = burst;
        _tokens = burst;
        _clock = clock ?? (() => DateTimeOffset.UtcNow);
        _lastRefill = _clock();
        _observer = observer ?? NoOpPolicyObserver.Instance;
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
            _observer.OnRateLimitDelay(delay);
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        }

        return await sendAsync(cancellationToken).ConfigureAwait(false);
    }

    private TimeSpan GetDelay()
    {
        lock (_gate)
        {
            var now = _clock();

            // トークン補充
            var elapsed = now - _lastRefill;
            if (elapsed > TimeSpan.Zero)
            {
                var refill = elapsed.TotalSeconds / _minInterval.TotalSeconds;
                _tokens = Math.Min(_burst, _tokens + refill);
                _lastRefill = now;
            }

            if (_tokens >= 1d)
            {
                _tokens -= 1d;
                return TimeSpan.Zero;
            }

            // 足りない分の時間を待機
            var needed = 1d - _tokens;
            var delaySeconds = needed * _minInterval.TotalSeconds;
            _tokens = 0d;
            _lastRefill = now + TimeSpan.FromSeconds(delaySeconds);
            return TimeSpan.FromSeconds(delaySeconds);
        }
    }
}
