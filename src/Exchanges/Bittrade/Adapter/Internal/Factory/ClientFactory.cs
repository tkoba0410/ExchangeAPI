using System;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Mappers;
using ExchangeApi.Exchanges.Bittrade.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Adapter.Private.Api;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Transport.Observability;
using ExchangeApi.Primitives.DomainCommon.Types;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Factory;

/// <summary>
/// Bittrade API クライアントを構築するファクトリ。
/// </summary>
[Obsolete("Use ExchangeApi.Exchanges.Bittrade.Composition.Factory. This factory will be removed in a future major release.")]
public static class ClientFactory
{
    public static IPublicApi CreatePublic(ClientOptions options) => CreatePublicClient(options);

    public static PublicClient CreatePublicClient(ClientOptions options)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        var (components, restClient) = BittradeClientBootstrap.CreatePublicComponents(options);
        return new PublicClient(components, restClient);
    }

    [Obsolete("Pass ClientOptions explicitly. This overload will be removed in a future major release.")]
    public static IPublicApi CreatePublic() => CreatePublicClient(new ClientOptions());

    [Obsolete("Use CreatePublicClient(ClientOptions) instead. This overload will be removed in a future major release.")]
    public static PublicClient CreatePublicClient(
        IRestCallObserver? observer = null,
        IRestClientLogger? logger = null) =>
        CreatePublicClient(new ClientOptions { Observer = observer, Logger = logger });

    internal static (MarketApi Market, PrivateApi Private) CreatePrivate(
        string accessKey,
        string secretKey,
        AccountId accountId)
    {
        if (accountId.IsEmpty)
        {
            throw new ArgumentException("accountId is required.", nameof(accountId));
        }

        var (components, _, _) = BittradeClientBootstrap.CreatePrivateComponents(
            new ClientOptions(),
            new ClientCredentials(accessKey, secretKey),
            accountId.Value);
        if (components.Private is null)
        {
            throw new InvalidOperationException("Private components are required to create a private client.");
        }

        var privateApi = new PrivateApi(components.Private);
        return (new MarketApi(components.Public, components.Markets), privateApi);
    }

    public static ExchangeClient Create(
        ClientOptions options,
        ClientCredentials credentials,
        string accountId = "default")
    {
        var (components, normalizedAccountId, restClient) = BittradeClientBootstrap.CreatePrivateComponents(options, credentials, accountId);
        return new ExchangeClient(components, normalizedAccountId, restClient);
    }

    [Obsolete("Use Create(ClientOptions, ClientCredentials, ...) instead. This overload will be removed in a future major release.")]
    public static ExchangeClient CreateDefault(
        string accessKey,
        string secretKey,
        string accountId) =>
        Create(new ClientOptions(), new ClientCredentials(accessKey, secretKey), accountId);

}
