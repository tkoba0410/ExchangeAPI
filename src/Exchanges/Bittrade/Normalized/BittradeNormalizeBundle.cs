using ExchangeApi.Exchanges.Bittrade.Normalized.Apis;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Call;
using ExchangeApi.Exchanges.Bittrade.Raw.Private;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Public;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;
namespace ExchangeApi.Exchanges.Bittrade.Normalized;

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
