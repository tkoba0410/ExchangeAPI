using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;

namespace ExchangeApi.Exchanges.Bittrade.Normalized;

internal sealed class BittradeNormalizedComponents
{
    public BittradeNormalizedMarketDataApi MarketData { get; }
    public BittradeNormalizedExchangeInfoApi ExchangeInfo { get; }
    public BittradeNormalizedAccountApi Account { get; }
    public BittradeNormalizedTradingApi Trading { get; }
    public string? AccountId { get; }

    public BittradeNormalizedComponents(
        BittradeNormalizedMarketDataApi marketData,
        BittradeNormalizedExchangeInfoApi exchangeInfo,
        BittradeNormalizedAccountApi account,
        BittradeNormalizedTradingApi trading,
        string? accountId)
    {
        MarketData = marketData;
        ExchangeInfo = exchangeInfo;
        Account = account;
        Trading = trading;
        AccountId = string.IsNullOrWhiteSpace(accountId) ? null : accountId;
    }
}
