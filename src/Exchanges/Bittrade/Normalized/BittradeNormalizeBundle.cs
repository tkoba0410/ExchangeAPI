using System;
using ExchangeApi.Exchanges.Bittrade.Normalized.Apis;
using ExchangeApi.Exchanges.Bittrade.Raw;
namespace ExchangeApi.Exchanges.Bittrade.Normalized;

internal sealed class BittradeNormalizeBundle
{
    public IBittradeNormalizedMarketDataApi MarketData { get; }
    public IBittradeNormalizedExchangeInfoApi ExchangeInfo { get; }
    public IBittradeNormalizedAccountApi Account { get; }
    public object RawBundle => _rawBundle;
    public string? AccountId { get; }

    private readonly IBittradeRawApi _rawBundle;

    public BittradeNormalizeBundle(
        IBittradeNormalizedMarketDataApi marketData,
        IBittradeNormalizedExchangeInfoApi exchangeInfo,
        IBittradeNormalizedAccountApi account,
        IBittradeRawApi rawBundle,
        string? accountId)
    {
        MarketData = marketData;
        ExchangeInfo = exchangeInfo;
        Account = account ?? throw new ArgumentNullException(nameof(account));
        _rawBundle = rawBundle;
        AccountId = accountId;
    }
}
