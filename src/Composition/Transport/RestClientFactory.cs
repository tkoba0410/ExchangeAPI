using System;
using System.Net.Http;
using Common.Interfaces;
using Core.Transport.Observability;
using Core.Transport.Policy;
using Core.Transport.Protocol;
using Core.Transport.Time;
using Core.Transport.Http;
namespace Composition.Transport;

/// <summary>
/// RestClient を組み立てる共通ファクトリ。ポリシー/ロガー/署名の拡張ポイントを一箇所にまとめる。
/// </summary>
public static class RestClientFactory
{
    public static RestClient Create(
        Uri baseUri,
        IHttpTransport? transport = null,
        IRequestSigner? signer = null,
        IHttpPolicy? policy = null,
        IRestClientLogger? logger = null,
        IRestCallObserver? observer = null,
        IExchangeErrorClassifier? errorClassifier = null,
        HttpClient? httpClient = null)
    {
        if (baseUri is null) throw new ArgumentNullException(nameof(baseUri));

        transport ??= new HttpTransport(httpClient ?? new HttpClient { BaseAddress = baseUri }, disposeHttpClient: httpClient is null);
        policy ??= HttpPolicyFactory.CreateDefault();

        return new RestClient(
            baseUri,
            transport,
            requestSigner: signer,
            policy: policy,
            logger: logger,
            observer: observer,
            errorClassifier: errorClassifier);
    }
}
