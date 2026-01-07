using System;
using System.Net;
using System.Net.Http;
namespace ExchangeApi.Core.Transport.Observability;

/// <summary>
/// REST 呼び出しの観測データをフックする拡張ポイント。
/// </summary>
public interface IRestCallObserver
{
    void OnRequest(RestCallContext context);

    void OnResponse(RestCallContext context, HttpResponseMessage response, string content, TimeSpan duration);

    void OnError(RestCallContext context, Exception exception, TimeSpan duration, HttpStatusCode? statusCode = null);
}
