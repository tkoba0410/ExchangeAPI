using System;
using System.Net.Http;

namespace ExchangeApi.Transport.Http;

/// <summary>
/// HTTP 送信経路の設定を排他的に表現する。
/// 同時指定による優先順位競合を防ぐため、1つの構成だけを選択する。
/// </summary>
public abstract record TransportConfig
{
    private TransportConfig() { }

    /// <summary>
    /// 呼び出し側が管理する <see cref="IHttpTransport"/> を利用する。
    /// </summary>
    public sealed record ExternalTransport(IHttpTransport Transport) : TransportConfig;

    /// <summary>
    /// 呼び出し側が管理する <see cref="HttpClient"/> を利用する。
    /// </summary>
    public sealed record ExternalHttpClient(HttpClient HttpClient) : TransportConfig;

    /// <summary>
    /// ライブラリ側で <see cref="HttpClient"/> を生成して利用する。
    /// </summary>
    public sealed record ManagedHttp(TimeSpan? Timeout = null) : TransportConfig;
}
