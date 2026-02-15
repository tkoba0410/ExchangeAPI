using System;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Application.ExchangeInfo.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized;
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
    public BittradeExchangeInfoApi ExchangeInfo { get; }
    public IExchangeMarketResolver Markets { get; }

    private BittradeClientComponents(
        NormalizedPublicApi publicApi,
        NormalizedPrivateApi? privateApi,
        BittradeExchangeInfoApi exchangeInfo,
        IExchangeMarketResolver markets)
    {
        Public = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        Private = privateApi;
        ExchangeInfo = exchangeInfo ?? throw new ArgumentNullException(nameof(exchangeInfo));
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
            var exchangeInfo = new BittradeExchangeInfoApi(publicApi);
            var markets = new BittradeMarketCatalogResolver();
            return new BittradeClientComponents(publicApi, privateApi: null, exchangeInfo, markets);
        }

        var normalizedAccountId = accountId!.Value;
        var normalizedComponents = NormalizedComponentFactory.FromRaw(
            raw,
            publicApi =>
            {
                return new NormalizedMarketResolver(new BittradeMarketCatalogResolver());
            },
            normalizedAccountId);

        var exchangeInfoFull = new BittradeExchangeInfoApi(normalizedComponents.Public);
        var marketsFull = new BittradeMarketCatalogResolver();
        return new BittradeClientComponents(normalizedComponents.Public, normalizedComponents.Private, exchangeInfoFull, marketsFull);
    }
}
