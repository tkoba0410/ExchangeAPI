using System;

namespace ExchangeApi.Transport.Policy;

/// <summary>
/// Http ポリシーのデフォルト構成を組み立てるファクトリ。
/// </summary>
public static class HttpPolicyFactory
{
    public static IHttpPolicy CreateDefault(HttpPolicyOptions? options = null, IPolicyObserver? observer = null)
    {
        var opts = options ?? HttpPolicyOptions.BitflyerDefaults();
        var obs = observer ?? NoOpPolicyObserver.Instance;

        return new HttpPolicyPipeline(
            new RateLimitHttpPolicy(opts.RequestsPerSecond, opts.RateLimitBurst, observer: obs),
            new CircuitBreakerHttpPolicy(opts.CircuitBreakerFailureThreshold, opts.CircuitBreakerOpenDuration, observer: obs),
            new RetryHttpPolicy(
                opts.MaxRetryAttemptsForGet,
                opts.MaxRetryAttemptsForOther,
                opts.RetryBaseDelay,
                opts.RetryMaxDelay,
                observer: obs),
            new TimeoutHttpPolicy(opts.Timeout));
    }
}
