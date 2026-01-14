using System;
using System.Net.Http;
namespace ExchangeApi.Core.Transport.Observability;

/// <summary>
/// 標準化されたログを出力するロガー（サンプル実装）。
/// 機密情報は出力しない前提で最小項目のみ記録する。
/// </summary>
public sealed class StructuredRestClientLogger : IRestClientLogger
{
    private readonly Action<string> _write;

    public StructuredRestClientLogger(Action<string> write)
    {
        _write = write ?? throw new ArgumentNullException(nameof(write));
    }

    public void LogRequest(HttpRequestMessage request)
    {
        var uri = request.RequestUri?.ToString() ?? "<null>";
        _write($"timestamp={DateTimeOffset.UtcNow:O} event_type=request method={request.Method.Method} uri={uri}");
    }

    public void LogResponse(HttpResponseMessage response, string content)
    {
        var reason = response.ReasonPhrase ?? "";
        var contentLength = content?.Length ?? 0;
        _write($"timestamp={DateTimeOffset.UtcNow:O} event_type=response status={(int)response.StatusCode} reason={reason} content_length={contentLength}");
    }

    public void LogError(Exception exception, HttpRequestMessage request)
    {
        var uri = request.RequestUri?.ToString() ?? "<null>";
        _write($"timestamp={DateTimeOffset.UtcNow:O} event_type=error method={request.Method.Method} uri={uri} error={exception.GetType().Name} message={exception.Message}");
    }
}
