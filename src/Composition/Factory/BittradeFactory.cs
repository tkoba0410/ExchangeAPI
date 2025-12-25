using System;
using ExchangeApi.Composition.Transport;
using ExchangeApi.Core.Transport.Policy;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Adapter.Mappers;
using ExchangeApi.Exchanges.Bittrade.Adapter.Facade;
using ExchangeApi.Exchanges.Bittrade.Raw;

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

        return new BittradeRawApi(
            new BittradePublicApi(restClient),
            new BittradePrivateApi(restClient),
            new BittradePrivateTradingApi(restClient));
    }

    public static BittradeExchangeClient CreateAdapter(BittradeFactoryOptions? options = null)
    {
        var settings = options ?? new BittradeFactoryOptions();
        if (string.IsNullOrWhiteSpace(settings.AccountId))
        {
            throw new InvalidOperationException("BittradeFactoryOptions.AccountId must be specified to create an adapter.");
        }

        var restClient = CreateRestClient(settings, requireCredentials: true);
        var bundle = BittradeApiBundle.FromRestClient(restClient, settings.AccountId);
        return new BittradeExchangeClient(bundle);
    }

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
                throw new InvalidOperationException("Bittrade credentials are required to create the adapter.");
            }

            return null;
        }

        return new BittradeRequestSigner(credentials.ApiKey, credentials.ApiSecret);
    }
}
