using System;
using System.Net.Http;

namespace ExchangeApi.Transport.Observability;

internal sealed class SanitizingRestClientLogger : IRestClientLogger
{
    private readonly IRestClientLogger _inner;

    public SanitizingRestClientLogger(IRestClientLogger inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public void LogRequest(HttpRequestMessage request)
    {
        using var sanitizedRequest = RequestLogSanitizer.CreateSanitizedRequest(request);
        _inner.LogRequest(sanitizedRequest);
    }

    public void LogResponse(HttpResponseMessage response, string content)
    {
        _inner.LogResponse(response, content);
    }

    public void LogError(Exception exception, HttpRequestMessage request)
    {
        using var sanitizedRequest = RequestLogSanitizer.CreateSanitizedRequest(request);
        _inner.LogError(exception, sanitizedRequest);
    }
}
