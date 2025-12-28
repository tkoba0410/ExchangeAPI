using System.Threading;
using System.Threading.Tasks;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

public interface IBittradeRawApi
{
    IBittradeRawMarketDataApi MarketData { get; }
    IBittradeRawTradingApi Trading { get; }

    Task<RawSymbolsResponse> GetSymbolsAsync(CancellationToken cancellationToken = default);

    Task<RawBalancesResponse> GetAccountBalanceAsync(string accountId, CancellationToken cancellationToken = default);
}
