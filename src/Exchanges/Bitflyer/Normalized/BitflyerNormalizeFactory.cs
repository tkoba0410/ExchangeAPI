using System;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Apis;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Call;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Markets;
using ExchangeApi.Exchanges.Bitflyer.Raw.Call;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized;

internal static class BitflyerNormalizeFactory
{
    public static BitflyerNormalizeBundle FromRestClient(
        IRestClient restClient,
        IBitflyerMarketResolver markets)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        if (markets is null) throw new ArgumentNullException(nameof(markets));

        var normalized = BitflyerNormalizedApi.FromRestClient(restClient);
        var account = CreateAccountApi(restClient, markets);
        var trading = CreateTradingApi(restClient, markets);

        return new BitflyerNormalizeBundle(
            marketData: normalized.MarketData,
            exchangeInfo: normalized.ExchangeInfo,
            account: account,
            trading: trading);
    }

    public static IBitflyerNormalizedAccountApi CreateAccountApi(IRestClient restClient, IBitflyerMarketResolver markets)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        if (markets is null) throw new ArgumentNullException(nameof(markets));

        var wire = new WireTransport(restClient);
        var accountApi = new BitflyerPrivateApi(wire);
        return new BitflyerNormalizedAccountApi(accountApi, markets);
    }

    public static IBitflyerNormalizedTradingApi CreateTradingApi(IRestClient restClient, IBitflyerMarketResolver markets)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        if (markets is null) throw new ArgumentNullException(nameof(markets));

        var wire = new WireTransport(restClient);
        var accountApi = new BitflyerPrivateApi(wire);
        var tradingApi = new BitflyerPrivateTradingApi(wire);
        return new BitflyerNormalizedTradingApi(tradingApi, accountApi, markets);
    }
}
