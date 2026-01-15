using System;
using System.Net;
using System.Net.Http;
using ExchangeApi.Transport.Observability;

namespace ExchangeApi.Tests.Integration.Public.Tests;

/// <summary>
/// Public API 用の素朴なロガー（ヘッダはそのまま出さない）。ボディは短くトリムする。
/// </summary>
public sealed class PublicApiLoggingObserver : IRestCallObserver
{
    private readonly Action<string> _log;
    private const int MaxBodyLength = 800;

    public PublicApiLoggingObserver(Action<string> log)
    {
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    public void OnRequest(RestCallContext context)
    {
        _log($"REQ {context.Method} {context.Request.RequestUri}");
    }

    public void OnResponse(RestCallContext context, HttpResponseMessage response, string content, TimeSpan duration)
    {
        _log($"RES {context.Method} {context.Request.RequestUri} {(int)response.StatusCode} {response.StatusCode} {duration.TotalMilliseconds:F0}ms body={Trim(content)}");
    }

    public void OnError(RestCallContext context, Exception exception, TimeSpan duration, HttpStatusCode? statusCode = null)
    {
        var code = statusCode.HasValue ? $"{(int)statusCode} {statusCode}" : "n/a";
        _log($"ERR {context.Method} {context.Request.RequestUri} {code} {duration.TotalMilliseconds:F0}ms ex={exception.GetType().Name}: {exception.Message}");
    }

    private static string Trim(string content)
    {
        if (string.IsNullOrEmpty(content)) return "(empty)";
        return content.Length <= MaxBodyLength
            ? content
            : content[..MaxBodyLength] + "...(truncated)";
    }
}
