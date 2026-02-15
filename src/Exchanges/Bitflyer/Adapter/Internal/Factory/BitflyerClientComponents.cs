using System;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Internal;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Raw.Api;
using ExchangeApi.Exchanges.Bitflyer.Wire.Internal;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Adapter.Internal.Factory;

internal sealed class BitflyerClientComponents
{
    public INormalizedApi Normalized { get; }
    public IExchangeMarketResolver Markets { get; }

    private BitflyerClientComponents(
        INormalizedApi normalized,
        IExchangeMarketResolver markets)
    {
        Normalized = normalized ?? throw new ArgumentNullException(nameof(normalized));
        Markets = markets ?? throw new ArgumentNullException(nameof(markets));
    }

    public static BitflyerClientComponents FromRestClient(IRestClient restClient)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));

        var wireTransport = new WireTransport(restClient);
        var wire = new WireCallExecutor(wireTransport);
        var raw = new RawApi(wire);
        var publicApi = new NormalizedPublicApi(raw);
        var contractMarkets = new BitflyerMarketCatalogResolver();
        var markets = new NormalizedMarketResolver(contractMarkets);
        var normalized = NormalizedApi.FromRaw(raw, markets);
        return new BitflyerClientComponents(normalized, contractMarkets);
    }
}
