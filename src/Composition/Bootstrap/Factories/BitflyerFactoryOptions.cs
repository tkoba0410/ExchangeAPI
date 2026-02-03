using System;
using System.Net.Http;
using ExchangeApi.Composition.Dtos;
using ExchangeApi.Composition.Abstractions;
using ExchangeApi.Transport.Http;
using ExchangeApi.Transport.Observability;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Time;

namespace ExchangeApi.Composition.Bootstrap.Factories;

/// <summary>
/// bitFlyer 用の生成オプション。すべて任意で、設定しなければ既定値（公開 API のみ）が使われる。
/// </summary>
public sealed class BitflyerFactoryOptions
{
    /// <summary>資格情報を直接指定する場合に利用。</summary>
    public ApiCredentials? Credentials { get; init; }

    /// <summary>環境変数などから資格情報を引く場合に利用。</summary>
    public IApiCredentialProvider? CredentialProvider { get; init; }

    public string AccountId { get; init; } = "default";

    public Uri? BaseUri { get; init; }

    public HttpClient? HttpClient { get; init; }

    public IHttpTransport? Transport { get; init; }

    public IHttpPolicy? Policy { get; init; }

    public HttpPolicyOptions? PolicyOptions { get; init; }

    public IRestClientLogger? Logger { get; init; }

    public IRestCallObserver? Observer { get; init; }

    public IExchangeErrorClassifier? ErrorClassifier { get; init; }

    public IRequestSigner? RequestSigner { get; init; }

    public IExchangeClock? Clock { get; init; }
}
