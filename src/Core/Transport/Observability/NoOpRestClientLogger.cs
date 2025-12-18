using System;
using System.Net.Http;
namespace ExchangeApi.Core.Transport.Observability;

/// <summary>
/// 何も記録しないダミー実装。
/// </summary>
public sealed class NoOpRestClientLogger : IRestClientLogger
{
    public static readonly NoOpRestClientLogger Instance = new();

    private NoOpRestClientLogger() { }

    public void LogRequest(HttpRequestMessage request) { }

    public void LogResponse(HttpResponseMessage response, string content) { }

    public void LogError(Exception exception, HttpRequestMessage request) { }
}
