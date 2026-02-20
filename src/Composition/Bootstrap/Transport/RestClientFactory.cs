using System;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Transport.Observability;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Time;
using ExchangeApi.Transport.Http;
namespace ExchangeApi.Composition.Bootstrap.Transport;

/// <summary>
/// RestClient を組み立てる共通ファクトリ。ポリシー/ロガー/署名の拡張ポイントを一箇所にまとめる。
/// </summary>
public static class RestClientFactory
{
    public static RestClient Create(
        Uri baseUri,
        TransportConfig transportConfig,
        IRequestSigner? signer = null,
        IHttpPolicy? policy = null,
        IRestClientLogger? logger = null,
        IRestCallObserver? observer = null,
        IExchangeErrorClassifier? errorClassifier = null)
    {
        if (baseUri is null) throw new ArgumentNullException(nameof(baseUri));
        if (transportConfig is null) throw new ArgumentNullException(nameof(transportConfig));

        var resolved = TransportConfigResolver.Resolve(baseUri, transportConfig);
        policy ??= HttpPolicyFactory.CreateDefault();

        return new RestClient(
            baseUri,
            resolved.Transport,
            requestSigner: signer,
            policy: policy,
            logger: logger,
            observer: observer,
            errorClassifier: errorClassifier,
            disposeTransport: resolved.DisposeTransport);
    }
}
