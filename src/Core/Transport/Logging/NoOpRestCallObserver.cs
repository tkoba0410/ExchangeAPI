using System;
using System.Net;
using System.Net.Http;
namespace Core.Transport.Logging;

/// <summary>
/// 観測を行わないダミーオブザーバ。
/// </summary>
public sealed class NoOpRestCallObserver : IRestCallObserver
{
    public static readonly NoOpRestCallObserver Instance = new();

    private NoOpRestCallObserver() { }

    public void OnRequest(RestCallContext context) { }

    public void OnResponse(RestCallContext context, HttpResponseMessage response, string content, TimeSpan duration) { }

    public void OnError(RestCallContext context, Exception exception, TimeSpan duration, HttpStatusCode? statusCode = null) { }
}
