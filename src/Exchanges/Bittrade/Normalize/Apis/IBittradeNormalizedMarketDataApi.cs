using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalize.Models;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Apis;

internal interface IBittradeNormalizedMarketDataApi
{
    Task<BittradeTickerNormalized> GetTickerAsync(string symbol, CancellationToken ct = default);
    Task<BittradeOrderBookNormalized> GetOrderBookAsync(string symbol, CancellationToken ct = default);
    Task<IReadOnlyList<BittradeExecutionNormalized>> GetExecutionsAsync(string symbol, CancellationToken ct = default);
}
