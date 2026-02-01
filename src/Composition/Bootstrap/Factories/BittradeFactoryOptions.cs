using System;
using System.Net.Http;
using ExchangeApi.Composition.Dtos;
using ExchangeApi.Composition.Abstractions;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Transport.Http;
using ExchangeApi.Transport.Observability;
using ExchangeApi.Transport.Policy;
using ExchangeApi.Transport.Protocol;

namespace ExchangeApi.Composition.Bootstrap.Factories;

/// <summary>
/// Bittrade 用の生成オプション。
/// </summary>
public sealed class BittradeFactoryOptions
{
    /// <summary>アクセス/シークレットキーを直接指定する。</summary>
    public ApiCredentials? Credentials { get; init; }

    /// <summary>CredentialProvider を使う場合に設定。</summary>
    public IApiCredentialProvider? CredentialProvider { get; init; }

    public ExchangeCode Exchange { get; init; } = ExchangeCode.Bittrade;

    /// <summary>Bittrade API で利用する account-id。既定は "default"。</summary>
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
}
