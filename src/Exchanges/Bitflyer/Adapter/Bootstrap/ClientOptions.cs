using System;
using ExchangeApi.Transport.Http;
using ExchangeApi.Transport.Observability;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Bootstrap;

/// <summary>
/// bitFlyer クライアントの構成オプション。
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

public static class ClientOptionsExtensions
{
    /// <summary>
    /// 観測性に関するオプションをまとめて設定するヘルパ。
    /// </summary>
    public static ClientOptions WithObservability(
        this ClientOptions options,
        IRestCallObserver observer,
        IRestClientLogger? logger = null)
    {
        return new ClientOptions
        {
            BaseUri = options.BaseUri,
            TransportConfig = options.TransportConfig,
            Policy = options.Policy,
            PolicyOptions = options.PolicyOptions,
            Logger = logger ?? options.Logger,
            Observer = observer,
            ErrorClassifier = options.ErrorClassifier
        };
    }
}
