using System;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Apis;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivateGet;
using ExchangeApi.Exchanges.Bitflyer.Raw.PrivatePost;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize;

internal static class BitflyerNormalizeFactory
{
    public static IBitflyerNormalizedAccountApi CreateAccountApi(IRestClient restClient, IExchangeMarketResolver markets)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        if (markets is null) throw new ArgumentNullException(nameof(markets));

        var wire = new Raw.Internal.Wire.BitflyerWireApi(restClient);
        var accountApi = new BitflyerPrivateApi(wire.Account);
        return new BitflyerNormalizedAccountApi(accountApi, markets);
    }

    public static IBitflyerNormalizedMarginApi CreateMarginApi(IRestClient restClient, IExchangeMarketResolver markets)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        if (markets is null) throw new ArgumentNullException(nameof(markets));

        var wire = new Raw.Internal.Wire.BitflyerWireApi(restClient);
        var accountApi = new BitflyerPrivateApi(wire.Account);
        return new BitflyerNormalizedMarginApi(accountApi, markets);
    }

    public static IBitflyerNormalizedTradingApi CreateTradingApi(IRestClient restClient, IExchangeMarketResolver markets)
    {
        if (restClient is null) throw new ArgumentNullException(nameof(restClient));
        if (markets is null) throw new ArgumentNullException(nameof(markets));

        var wire = new Raw.Internal.Wire.BitflyerWireApi(restClient);
        var accountApi = new BitflyerPrivateApi(wire.Account);
        var tradingApi = new BitflyerPrivateTradingApi(wire.Trading);
        return new BitflyerNormalizedTradingApi(tradingApi, accountApi, markets);
    }
}
