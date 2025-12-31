using System;
using ExchangeApi.Core.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Public;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;

public sealed class BittradeWireApi : IBittradeWireApi
{
    public IBittradeWireMarketDataApi MarketData { get; }
    public IBittradeWireTradingApi Trading { get; }
    public IBittradeWireAccountApi Account { get; }
    public IBittradeWireCommonApi Common { get; }

    public BittradeWireApi(IRestClient restClient, string? accountId = null)
        : this(
            new Public.BittradeWireMarketDataApi(restClient),
            string.IsNullOrWhiteSpace(accountId)
                ? new Private.BittradeWireTradingApiNotSupported()
                : new Private.BittradeWireTradingApi(restClient, accountId),
            new Private.BittradeWireAccountApi(restClient),
            new Public.BittradeWireCommonApi(restClient))
    {
    }

    public BittradeWireApi(IBittradeWireMarketDataApi marketData, IBittradeWireTradingApi trading)
        : this(marketData, trading, new Private.BittradeWireAccountApiNotSupported(), new BittradeWireCommonApiNotSupported())
    {
    }

    public BittradeWireApi(
        IBittradeWireMarketDataApi marketData,
        IBittradeWireTradingApi trading,
        IBittradeWireAccountApi account,
        IBittradeWireCommonApi common)
    {
        MarketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        Trading = trading ?? throw new ArgumentNullException(nameof(trading));
        Account = account ?? throw new ArgumentNullException(nameof(account));
        Common = common ?? throw new ArgumentNullException(nameof(common));
    }
}
