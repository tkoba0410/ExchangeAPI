using System;
using System.Net.Http;
using System.Text.Json;
namespace Core.Transport.Logging;

/// <summary>
/// JSON で標準化されたログを出力するロガー（サンプル実装）。
/// 機密情報は出力しない前提で最小項目のみ記録する。
/// </summary>
public sealed class StructuredRestClientLogger : IRestClientLogger
{
    private readonly Action<string> _write;
    private readonly JsonSerializerOptions _serializerOptions;

    public StructuredRestClientLogger(Action<string> write)
    {
        _write = write ?? throw new ArgumentNullException(nameof(write));
        _serializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    }

    public void LogRequest(HttpRequestMessage request)
    {
        var payload = new
        {
            timestamp = DateTimeOffset.UtcNow,
            event_type = "request",
            method = request.Method.Method,
            uri = request.RequestUri?.ToString(),
        };
        _write(JsonSerializer.Serialize(payload, _serializerOptions));
    }

    public void LogResponse(HttpResponseMessage response, string content)
    {
        var payload = new
        {
            timestamp = DateTimeOffset.UtcNow,
            event_type = "response",
            status_code = (int)response.StatusCode,
            reason = response.ReasonPhrase,
            content_length = content?.Length ?? 0
        };
        _write(JsonSerializer.Serialize(payload, _serializerOptions));
    }

    public void LogError(Exception exception, HttpRequestMessage request)
    {
        var payload = new
        {
            timestamp = DateTimeOffset.UtcNow,
            event_type = "error",
            method = request.Method.Method,
            uri = request.RequestUri?.ToString(),
            error = exception.GetType().Name,
            message = exception.Message
        };
        _write(JsonSerializer.Serialize(payload, _serializerOptions));
    }
}
