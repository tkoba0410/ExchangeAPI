using System;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Resolve;
using ExchangeApi.Exchanges.Bittrade.Normalized;
using ExchangeApi.Exchanges.Bittrade.Normalized.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Api;
using ExchangeApi.Exchanges.Bittrade.Wire.Internal;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Factory;

internal sealed class BittradeClientComponents
{
    public NormalizedPublicApi Public { get; }
    public NormalizedPrivateApi? Private { get; }
    public INormalizedApi? Normalized { get; }
    public IExchangeMarketResolver Markets { get; }

    private BittradeClientComponents(
        NormalizedPublicApi publicApi,
        NormalizedPrivateApi? privateApi,
        INormalizedApi? normalizedApi,
        IExchangeMarketResolver markets)
    {
        Public = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        Private = privateApi;
        Normalized = normalizedApi;
        Markets = markets ?? throw new ArgumentNullException(nameof(markets));
    }

    public static BittradeClientComponents FromRestClient(IRestClient restClient, AccountId? accountId = null)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        var hasAccountId = accountId is { IsEmpty: false };

        var wireTransport = new WireTransport(restClient);
        var wire = new WireCallExecutor(wireTransport);
        var raw = new RawApi(wire);

        if (!hasAccountId)
        {
            var publicApi = new NormalizedPublicApi(raw);
            var markets = new ExchangeRequestResolver();
            return new BittradeClientComponents(publicApi, privateApi: null, normalizedApi: null, markets);
        }

        var normalizedAccountId = accountId!.Value;
        var marketsFull = new ExchangeRequestResolver();
        var normalizedComponents = NormalizedComponentFactory.FromRaw(
            raw,
            publicApi =>
            {
                return new NormalizedRequestResolver(marketsFull);
            },
            normalizedAccountId);

        var normalizedApi = NormalizeFactory.FromRaw(
            raw,
            new NormalizedRequestResolver(marketsFull),
            normalizedAccountId);

        return new BittradeClientComponents(
            normalizedComponents.Public,
            normalizedComponents.Private,
            normalizedApi,
            marketsFull);
    }
}
