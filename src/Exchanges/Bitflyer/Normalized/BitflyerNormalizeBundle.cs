using System;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Apis;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Call;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized;

internal sealed class BitflyerNormalizeBundle
{
    public BitflyerNormalizedMarketDataFacade MarketData { get; }
    public BitflyerNormalizedExchangeInfoFacade ExchangeInfo { get; }
    public IBitflyerNormalizedAccountApi Account { get; }
    public IBitflyerNormalizedTradingApi Trading { get; }

    public BitflyerNormalizeBundle(
        BitflyerNormalizedMarketDataFacade marketData,
        BitflyerNormalizedExchangeInfoFacade exchangeInfo,
        IBitflyerNormalizedAccountApi account,
        IBitflyerNormalizedTradingApi trading)
    {
        MarketData = marketData ?? throw new ArgumentNullException(nameof(marketData));
        ExchangeInfo = exchangeInfo ?? throw new ArgumentNullException(nameof(exchangeInfo));
        Account = account ?? throw new ArgumentNullException(nameof(account));
        Trading = trading ?? throw new ArgumentNullException(nameof(trading));
    }
}
