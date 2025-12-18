using System;
using System.Net.Http;
namespace ExchangeApi.Core.Transport.Policy;

/// <summary>
/// ポリシーの挙動を観測するためのフック。
/// </summary>
public interface IPolicyObserver
{
    void OnRetry(HttpRequestMessage request, int attempt, int maxAttempts, TimeSpan delay, Exception? lastException, HttpResponseMessage? lastResponse);
    void OnRateLimitDelay(TimeSpan delay);
    void OnCircuitOpened(TimeSpan openDuration);
    void OnCircuitRejected();
}

public sealed class NoOpPolicyObserver : IPolicyObserver
{
    public static readonly NoOpPolicyObserver Instance = new();
    private NoOpPolicyObserver() { }
    public void OnRetry(HttpRequestMessage request, int attempt, int maxAttempts, TimeSpan delay, Exception? lastException, HttpResponseMessage? lastResponse) { }
    public void OnRateLimitDelay(TimeSpan delay) { }
    public void OnCircuitOpened(TimeSpan openDuration) { }
    public void OnCircuitRejected() { }
}
