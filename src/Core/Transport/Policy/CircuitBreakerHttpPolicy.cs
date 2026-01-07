using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Transport.Protocol;
namespace ExchangeApi.Core.Transport.Policy;

/// <summary>
/// 連続失敗を検知してフェイルファストする簡易サーキットブレーカ。
/// </summary>
public sealed class CircuitBreakerHttpPolicy : IHttpPolicy
{
    private readonly int _failureThreshold;
    private readonly TimeSpan _openDuration;
    private readonly Func<DateTimeOffset> _clock;
    private readonly IPolicyObserver _observer;

    private readonly object _gate = new();
    private CircuitState _state = CircuitState.Closed;
    private int _failureCount;
    private DateTimeOffset _openUntil;

    public CircuitBreakerHttpPolicy(int failureThreshold, TimeSpan openDuration, Func<DateTimeOffset>? clock = null, IPolicyObserver? observer = null)
    {
        if (failureThreshold < 1) throw new ArgumentOutOfRangeException(nameof(failureThreshold));
        if (openDuration <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(openDuration));

        _failureThreshold = failureThreshold;
        _openDuration = openDuration;
        _clock = clock ?? (() => DateTimeOffset.UtcNow);
        _observer = observer ?? NoOpPolicyObserver.Instance;
    }

    public async Task<HttpResponseMessage> ExecuteAsync(
        HttpRequestMessage request,
        Func<CancellationToken, Task<HttpResponseMessage>> sendAsync,
        CancellationToken cancellationToken = default)
    {
        EnsureAllowedOrThrow();

        try
        {
            var response = await sendAsync(cancellationToken).ConfigureAwait(false);

            if (IsFailureResponse(response))
            {
                RecordFailure();
            }
            else
            {
                RecordSuccess();
            }

            return response;
        }
        catch (Exception ex) when (IsFailureException(ex, cancellationToken))
        {
            RecordFailure();
            throw;
        }
    }

    private void EnsureAllowedOrThrow()
    {
        lock (_gate)
        {
            var now = _clock();

            if (_state == CircuitState.Open)
            {
                if (now >= _openUntil)
                {
                    _state = CircuitState.HalfOpen;
                    _failureCount = 0;
                }
                else
                {
                    _observer.OnCircuitRejected();
                    throw new HttpRequestException("Circuit breaker is open.", null, HttpStatusCode.ServiceUnavailable);
                }
            }
        }
    }

    private void RecordSuccess()
    {
        lock (_gate)
        {
            _failureCount = 0;
            _state = CircuitState.Closed;
        }
    }

    private void RecordFailure()
    {
        lock (_gate)
        {
            _failureCount++;

            if (_failureCount >= _failureThreshold)
            {
                _state = CircuitState.Open;
                _openUntil = _clock().Add(_openDuration);
                _observer.OnCircuitOpened(_openDuration);
            }
        }
    }

    private static bool IsFailureResponse(HttpResponseMessage response)
    {
        if (response is null) return false;
        var status = response.StatusCode;
        return status == HttpStatusCode.TooManyRequests || (int)status >= 500;
    }

    private static bool IsFailureException(Exception exception, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested) return false;
        if (exception is TransportException apiEx)
        {
            return apiEx.ErrorCategory is TransportErrorCategory.RateLimit
                or TransportErrorCategory.Network
                or TransportErrorCategory.Server
                or TransportErrorCategory.Unknown;
        }

        return exception is HttpRequestException or TaskCanceledException;
    }

    private enum CircuitState
    {
        Closed,
        Open,
        HalfOpen
    }
}
