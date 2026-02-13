using System;
namespace ExchangeApi.Transport.Policy;

/// <summary>
/// HTTP ポリシーのデフォルト設定。
/// </summary>
public sealed class HttpPolicyOptions
{
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(8);

    public int MaxRetryAttemptsForGet { get; init; } = 3;

    public int MaxRetryAttemptsForOther { get; init; } = 1;

    public TimeSpan RetryBaseDelay { get; init; } = TimeSpan.FromMilliseconds(200);

    public TimeSpan RetryMaxDelay { get; init; } = TimeSpan.FromSeconds(2);

    public TimeSpan MaxTotalRetryTime { get; init; } = TimeSpan.FromSeconds(10);

    public double RequestsPerSecond { get; init; } = 5d;

    public int RateLimitBurst { get; init; } = 2;

    public int CircuitBreakerFailureThreshold { get; init; } = 3;

    public TimeSpan CircuitBreakerOpenDuration { get; init; } = TimeSpan.FromSeconds(5);

}
