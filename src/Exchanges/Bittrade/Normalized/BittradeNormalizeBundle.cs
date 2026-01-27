using System;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Api;
namespace ExchangeApi.Exchanges.Bittrade.Normalized;

internal sealed class BittradeNormalizeBundle
{
    public IBittradeRawApi Raw { get; }
    public IBittradeNormalizedMarketDataApi MarketData { get; }
    public IBittradeNormalizedExchangeInfoApi ExchangeInfo { get; }
    public IBittradeNormalizedAccountApi Account { get; }
    public string? AccountId { get; }

    public BittradeNormalizeBundle(
        IBittradeRawApi raw,
        IBittradeNormalizedMarketDataApi marketData,
        IBittradeNormalizedExchangeInfoApi exchangeInfo,
        IBittradeNormalizedAccountApi account,
        string? accountId)
    {
        Raw = raw ?? throw new ArgumentNullException(nameof(raw));
        MarketData = marketData;
        ExchangeInfo = exchangeInfo;
        Account = account ?? throw new ArgumentNullException(nameof(account));
        AccountId = accountId;
    }
}
