using System;
using System.Net.Http;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Shared.Transport.Http;
using ExchangeApi.Shared.Transport.Observability;
using ExchangeApi.Shared.Transport.Policy;
using ExchangeApi.Shared.Transport.Protocol;

namespace ExchangeApi.Shared.Composition.Bootstrap.Factories;

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
