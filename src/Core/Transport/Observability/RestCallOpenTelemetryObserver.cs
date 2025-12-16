using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net;
using System.Net.Http;
namespace Core.Transport.Observability;

/// <summary>
/// Activity/Meter を用いた簡易 OpenTelemetry ブリッジのサンプル実装。
/// メトリクス命名: exchangeapi_requests_total, exchangeapi_request_duration_seconds
/// タグ: endpoint, method, status, product_code, error
/// </summary>
public sealed class RestCallOpenTelemetryObserver : IRestCallObserver, IDisposable
{
    private readonly ActivitySource _activitySource;
    private readonly Meter _meter;
    private readonly Counter<long> _requestsTotal;
    private readonly Histogram<double> _requestDuration;
    private readonly ConcurrentDictionary<Guid, Activity> _activities = new();
    private bool _disposed;

    public RestCallOpenTelemetryObserver(string sourceName = "ExchangeApi.RestClient")
    {
        _activitySource = new ActivitySource(sourceName);
        _meter = new Meter("exchangeapi");
        _requestsTotal = _meter.CreateCounter<long>("exchangeapi_requests_total");
        _requestDuration = _meter.CreateHistogram<double>("exchangeapi_request_duration_seconds");
    }

    public void OnRequest(RestCallContext context)
    {
        var activity = _activitySource.StartActivity("exchangeapi.request", ActivityKind.Client);
        if (activity is not null)
        {
            activity.SetTag("endpoint", context.Endpoint);
            activity.SetTag("method", context.Method);
            if (!string.IsNullOrWhiteSpace(context.ProductCode))
            {
                activity.SetTag("product_code", context.ProductCode);
            }
            _activities[context.RequestId] = activity;
        }
    }

    public void OnResponse(RestCallContext context, HttpResponseMessage response, string content, TimeSpan duration)
    {
        var tags = BuildTags(context, response.StatusCode, error: null);
        _requestsTotal.Add(1, tags);
        _requestDuration.Record(duration.TotalSeconds, tags);
        StopActivity(context, ActivityStatusCode.Ok, description: ((int)response.StatusCode).ToString());
    }

    public void OnError(RestCallContext context, Exception exception, TimeSpan duration, HttpStatusCode? statusCode = null)
    {
        var tags = BuildTags(context, statusCode, exception.GetType().Name);
        _requestsTotal.Add(1, tags);
        _requestDuration.Record(duration.TotalSeconds, tags);
        StopActivity(context, ActivityStatusCode.Error, exception.Message);
    }

    private static TagList BuildTags(RestCallContext context, HttpStatusCode? statusCode, string? error)
    {
        var tags = new TagList
        {
            { "endpoint", context.Endpoint },
            { "method", context.Method }
        };

        if (statusCode is not null)
        {
            tags.Add("status", (int)statusCode.Value);
        }

        if (!string.IsNullOrWhiteSpace(context.ProductCode))
        {
            tags.Add("product_code", context.ProductCode);
        }

        if (!string.IsNullOrWhiteSpace(error))
        {
            tags.Add("error", error);
        }

        return tags;
    }

    private void StopActivity(RestCallContext context, ActivityStatusCode status, string? description)
    {
        if (_activities.TryRemove(context.RequestId, out var activity) && activity is not null)
        {
            activity.SetStatus(status, description);
            activity.Dispose();
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _meter.Dispose();
        _activitySource.Dispose();
    }
}
