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

    public TimeSpan RetryBaseDelay { get; init; } = TimeSpan.FromMilliseconds(500);

    public TimeSpan RetryMaxDelay { get; init; } = TimeSpan.FromSeconds(4);

    public double RequestsPerSecond { get; init; } = 3d;

    public int CircuitBreakerFailureThreshold { get; init; } = 5;

    public TimeSpan CircuitBreakerOpenDuration { get; init; } = TimeSpan.FromSeconds(5);

    public static HttpPolicyOptions BitflyerDefaults() => new();
}
