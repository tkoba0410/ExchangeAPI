using ExchangeApi.Exchanges.Bittrade.Normalize.Apis;
using ExchangeApi.Exchanges.Bittrade.Raw.Types;
using ExchangeApi.Exchanges.Bittrade.Raw;
namespace ExchangeApi.Exchanges.Bittrade.Normalize;

internal sealed class BittradeNormalizeBundle
{
    public IBittradeNormalizedMarketDataApi MarketData { get; }
    public IBittradeNormalizedExchangeInfoApi ExchangeInfo { get; }
    public IBittradeNormalizedAccountApi? Account { get; }
    public object RawBundle => _rawBundle;
    public string? AccountId { get; }

    private readonly IBittradeRawApi _rawBundle;

    public BittradeNormalizeBundle(
        IBittradeNormalizedMarketDataApi marketData,
        IBittradeNormalizedExchangeInfoApi exchangeInfo,
        IBittradeNormalizedAccountApi? account,
        IBittradeRawApi rawBundle,
        string? accountId)
    {
        MarketData = marketData;
        ExchangeInfo = exchangeInfo;
        Account = account;
        _rawBundle = rawBundle;
        AccountId = accountId;
    }
}
