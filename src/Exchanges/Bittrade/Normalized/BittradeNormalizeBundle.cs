using System;
using ExchangeApi.Exchanges.Bittrade.Normalized.Apis;
using ExchangeApi.Exchanges.Bittrade.Raw;
namespace ExchangeApi.Exchanges.Bittrade.Normalized;

internal sealed class BittradeNormalizeBundle
{
    public IBittradeNormalizedMarketDataApi MarketData { get; }
    public IBittradeNormalizedExchangeInfoApi ExchangeInfo { get; }
    public IBittradeNormalizedAccountApi Account { get; }
    public string? AccountId { get; }

    public BittradeNormalizeBundle(
        IBittradeNormalizedMarketDataApi marketData,
        IBittradeNormalizedExchangeInfoApi exchangeInfo,
        IBittradeNormalizedAccountApi account,
        string? accountId)
    {
        MarketData = marketData;
        ExchangeInfo = exchangeInfo;
        Account = account ?? throw new ArgumentNullException(nameof(account));
        AccountId = accountId;
    }
}
