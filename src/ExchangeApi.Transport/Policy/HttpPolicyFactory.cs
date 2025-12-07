using System;

namespace ExchangeApi.Transport.Policy;

/// <summary>
/// Http ポリシーのデフォルト構成を組み立てるファクトリ。
/// </summary>
public static class HttpPolicyFactory
{
    public static IHttpPolicy CreateDefault(HttpPolicyOptions? options = null)
    {
        var opts = options ?? HttpPolicyOptions.BitflyerDefaults();

        return new HttpPolicyPipeline(
            new RateLimitHttpPolicy(opts.RequestsPerSecond, opts.RateLimitBurst),
            new CircuitBreakerHttpPolicy(opts.CircuitBreakerFailureThreshold, opts.CircuitBreakerOpenDuration),
            new RetryHttpPolicy(
                opts.MaxRetryAttemptsForGet,
                opts.MaxRetryAttemptsForOther,
                opts.RetryBaseDelay,
                opts.RetryMaxDelay),
            new TimeoutHttpPolicy(opts.Timeout));
    }
}
