using System;
using System.Net.Http;
using ExchangeApi.Transport.Observability;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Factory;

/// <summary>
/// Bittrade クライアントの構成オプション。
/// </summary>
public sealed class ClientOptions
{
    public Uri? BaseUri { get; init; }
    public TimeSpan? Timeout { get; init; }
    public HttpClient? HttpClient { get; init; }
    public IHttpPolicy? Policy { get; init; }
    public HttpPolicyOptions? PolicyOptions { get; init; }
    public IRestClientLogger? Logger { get; init; }
    public IRestCallObserver? Observer { get; init; }
    public IExchangeErrorClassifier? ErrorClassifier { get; init; }
}
