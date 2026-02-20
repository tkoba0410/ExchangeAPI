using System;
using ExchangeApi.Transport.Http;
using ExchangeApi.Transport.Observability;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Bootstrap;

/// <summary>
/// Bittrade クライアントの構成オプション。
/// </summary>
public sealed class ClientOptions
{
    public Uri? BaseUri { get; init; }
    public TransportConfig TransportConfig { get; init; } = new TransportConfig.ManagedHttp();
    public IHttpPolicy? Policy { get; init; }
    public HttpPolicyOptions? PolicyOptions { get; init; }
    public IRestClientLogger? Logger { get; init; }
    public IRestCallObserver? Observer { get; init; }
    public IExchangeErrorClassifier? ErrorClassifier { get; init; }
}
