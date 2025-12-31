using System;
using ExchangeApi.Composition.Transport;
using ExchangeApi.Core.Transport.Policy;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Adapter.Mappers;
using ExchangeApi.Exchanges.Bittrade.Adapter.Facade;
using ExchangeApi.Exchanges.Bittrade.Normalize.Facade;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;
using ExchangeApi.Contracts.Interfaces;

namespace ExchangeApi.Composition.Factory;

/// <summary>
/// Bittrade 用の標準配線ファクトリ。Raw を既定とし、Adapter は明示 opt-in。
/// </summary>
public static class BittradeFactory
{
    private static readonly Uri DefaultBaseUri = new("https://api-cloud.bittrade.co.jp/");

    public static BittradeRawApi CreateRaw(BittradeFactoryOptions? options = null)
    {
        var settings = options ?? new BittradeFactoryOptions();
        var restClient = CreateRestClient(settings, requireCredentials: false);

        var wire = new BittradeWireApi(restClient, settings.AccountId);
        return new BittradeRawApi(wire);
    }

    /// <summary>
    /// Bittrade の Normalized 入口を作成する。
    /// Raw -> Normalize までを提供し、Contracts/Adapter は返さない。
    /// </summary>
    public static BittradeNormalizedApi CreateExchange(BittradeFactoryOptions? options = null)
    {
        var settings = options ?? new BittradeFactoryOptions();
        var requireCredentials = !string.IsNullOrWhiteSpace(settings.AccountId);
        var restClient = CreateRestClient(settings, requireCredentials: requireCredentials);

        return BittradeNormalizedApi.FromRestClient(restClient, settings.AccountId);
    }

    public static IExchangeClient CreateClient(BittradeFactoryOptions? options = null)
    {
        var settings = options ?? new BittradeFactoryOptions();
        if (string.IsNullOrWhiteSpace(settings.AccountId))
        {
            throw new InvalidOperationException("BittradeFactoryOptions.AccountId must be specified to create a client.");
        }

        var restClient = CreateRestClient(settings, requireCredentials: true);
        var bundle = BittradeApiBundle.FromRestClient(restClient, settings.AccountId);
        return new BittradeExchangeClient(bundle);
    }

    [Obsolete("Use CreateClient(...) instead. This method will be removed in a future major release.")]
    internal static BittradeExchangeClient CreateAdapter(BittradeFactoryOptions? options = null) =>
        (BittradeExchangeClient)CreateClient(options);

    private static RestClient CreateRestClient(BittradeFactoryOptions settings, bool requireCredentials)
    {
        var baseUri = settings.BaseUri ?? DefaultBaseUri;
        var policy = settings.Policy ?? HttpPolicyFactory.CreateDefault(settings.PolicyOptions);

        return RestClientFactory.Create(
            baseUri,
            transport: settings.Transport,
            signer: settings.RequestSigner ?? CreateSigner(settings, requireCredentials),
            policy: policy,
            logger: settings.Logger,
            observer: settings.Observer,
            errorClassifier: settings.ErrorClassifier ?? new BittradeErrorClassifier(),
            httpClient: settings.HttpClient);
    }

    private static IRequestSigner? CreateSigner(BittradeFactoryOptions settings, bool requireCredentials)
    {
        var credentials = settings.Credentials
            ?? settings.CredentialProvider?.Get(settings.Exchange, settings.AccountId);

        if (credentials is null)
        {
            if (requireCredentials)
            {
                throw new InvalidOperationException("Bittrade credentials are required to create the client.");
            }

            return null;
        }

        return new BittradeRequestSigner(credentials.ApiKey, credentials.ApiSecret);
    }
}
