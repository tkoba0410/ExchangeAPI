using System;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Shared.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Apis;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Call;
using ExchangeApi.Exchanges.Bitflyer.Raw.Call;
using ExchangeApi.Shared.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized;

internal static class BitflyerNormalizeFactory
{
    public static IBitflyerNormalizedAccountApi CreateAccountApi(IRestClient restClient, IExchangeMarketResolver markets)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        if (markets is null) throw new ArgumentNullException(nameof(markets));

        var wire = new WireTransport(restClient);
        var accountApi = new BitflyerPrivateApi(wire);
        return new BitflyerNormalizedAccountApi(accountApi, markets);
    }

    public static IBitflyerNormalizedTradingApi CreateTradingApi(IRestClient restClient, IExchangeMarketResolver markets)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        if (markets is null) throw new ArgumentNullException(nameof(markets));

        var wire = new WireTransport(restClient);
        var accountApi = new BitflyerPrivateApi(wire);
        var tradingApi = new BitflyerPrivateTradingApi(wire);
        return new BitflyerNormalizedTradingApi(tradingApi, accountApi, markets);
    }
}
