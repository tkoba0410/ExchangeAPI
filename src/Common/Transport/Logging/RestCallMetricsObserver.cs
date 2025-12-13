using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace Common.Transport.Logging;

/// <summary>
/// 簡易なメトリクス集計用オブザーバ（サンプル実装）。
/// </summary>
public sealed class RestCallMetricsObserver : IRestCallObserver
{
    private long _successCount;
    private long _errorCount;
    private long _totalDurationTicks;

    private readonly ConcurrentDictionary<string, EndpointMetrics> _byEndpoint = new(StringComparer.Ordinal);

    public RestCallMetricsSnapshot Snapshot()
    {
        return new RestCallMetricsSnapshot(
            successCount: Interlocked.Read(ref _successCount),
            errorCount: Interlocked.Read(ref _errorCount),
            averageDuration: _successCount + _errorCount == 0
                ? TimeSpan.Zero
                : TimeSpan.FromTicks(Interlocked.Read(ref _totalDurationTicks) / Math.Max(1, _successCount + _errorCount)),
            Endpoints: _byEndpoint);
    }

    public void OnRequest(RestCallContext context) { }

    public void OnResponse(RestCallContext context, HttpResponseMessage response, string content, TimeSpan duration)
    {
        Interlocked.Increment(ref _successCount);
        Interlocked.Add(ref _totalDurationTicks, duration.Ticks);
        UpdateEndpoint(context.Endpoint, duration, response.StatusCode, isError: false);
    }

    public void OnError(RestCallContext context, Exception exception, TimeSpan duration, HttpStatusCode? statusCode = null)
    {
        Interlocked.Increment(ref _errorCount);
        Interlocked.Add(ref _totalDurationTicks, duration.Ticks);
        UpdateEndpoint(context.Endpoint, duration, statusCode, isError: true);
    }

    private void UpdateEndpoint(string endpoint, TimeSpan duration, HttpStatusCode? statusCode, bool isError)
    {
        var metrics = _byEndpoint.GetOrAdd(endpoint ?? string.Empty, _ => new EndpointMetrics());
        metrics.Add(duration, statusCode, isError);
    }
}

public sealed record RestCallMetricsSnapshot(
    long successCount,
    long errorCount,
    TimeSpan averageDuration,
    ConcurrentDictionary<string, EndpointMetrics> Endpoints);

public sealed class EndpointMetrics
{
    private long _success;
    private long _error;
    private long _durationTicks;
    private HttpStatusCode? _lastStatus;

    public void Add(TimeSpan duration, HttpStatusCode? statusCode, bool isError)
    {
        if (isError)
        {
            Interlocked.Increment(ref _error);
        }
        else
        {
            Interlocked.Increment(ref _success);
        }

        Interlocked.Add(ref _durationTicks, duration.Ticks);
        _lastStatus = statusCode;
    }

    public EndpointMetricsSnapshot Snapshot()
    {
        var total = Math.Max(1, _success + _error);
        return new EndpointMetricsSnapshot(
            Success: Interlocked.Read(ref _success),
            Error: Interlocked.Read(ref _error),
            AverageDuration: TimeSpan.FromTicks(Interlocked.Read(ref _durationTicks) / total),
            LastStatus: _lastStatus);
    }
}

public sealed record EndpointMetricsSnapshot(long Success, long Error, TimeSpan AverageDuration, HttpStatusCode? LastStatus);
