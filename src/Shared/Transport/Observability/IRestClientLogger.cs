using System;
using System.Net.Http;
namespace ExchangeApi.Shared.Transport.Observability;

/// <summary>
/// RestClient のリクエスト/レスポンスを記録するための拡張ポイント。
/// </summary>
public interface IRestClientLogger
{
    void LogRequest(HttpRequestMessage request);

    void LogResponse(HttpResponseMessage response, string content);

    void LogError(Exception exception, HttpRequestMessage request);
}
