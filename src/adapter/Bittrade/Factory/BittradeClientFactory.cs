using System;
using System.Net.Http;
using ExchangeApi.Adapter.Bittrade.Apis;
using ExchangeApi.Contracts.Contracts;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Transport;

namespace ExchangeApi.Adapter.Bittrade.Factory;

/// <summary>
/// Bittrade Public API クライアントを構築するファクトリ（Publicのみ）。
/// </summary>
public static class BittradeClientFactory
{
    private static readonly Uri BaseUri = new("https://api-cloud.bittrade.co.jp/");

    public static IMarketDataApi CreatePublic()
    {
        var handler = new HttpClientHandler();
        var transport = new HttpTransport(new HttpClient(handler, disposeHandler: true), disposeHttpClient: true);
        var policy = HttpPolicyFactory.CreateDefault();
        var restClient = new RestClient(BaseUri, transport, policy: policy);
        return new BittradeMarketDataApi(restClient);
    }
}
