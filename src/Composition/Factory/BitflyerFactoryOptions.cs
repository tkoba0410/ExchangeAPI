using System;
using System.Net.Http;
using Common.Dtos;
using Common.Interfaces;
using Core.Transport.Http;
using Core.Transport.Observability;
using Core.Transport.Policy;
using Core.Transport.Protocol;
using Core.Transport.Time;

namespace Composition.Factory;

/// <summary>
/// bitFlyer 用の生成オプション。すべて任意で、設定しなければ既定値（公開 API のみ）が使われる。
/// </summary>
public sealed class BitflyerFactoryOptions
{
    /// <summary>資格情報を直接指定する場合に利用。</summary>
    public ApiCredentials? Credentials { get; init; }

    /// <summary>環境変数などから資格情報を引く場合に利用。</summary>
    public IApiCredentialProvider? CredentialProvider { get; init; }

    public string ExchangeId { get; init; } = "bitFlyer";

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
