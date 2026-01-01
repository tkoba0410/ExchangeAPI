using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalize;
using ExchangeApi.Exchanges.Bittrade.Normalize.Models;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Apis;

internal interface IBittradeNormalizedMarketDataApi
{
    Task<BittradeTickerNormalized> GetTickerAsync(string symbol, CancellationToken ct = default);
    Task<BittradeOrderBookNormalized> GetOrderBookAsync(string symbol, CancellationToken ct = default);
    Task<IReadOnlyList<BittradeExecutionNormalized>> GetExecutionsAsync(string symbol, CancellationToken ct = default);

    Task<BittradeNormalizedCall<BittradeTickerNormalized, JsonElement>> GetTickerCallAsync(
        string symbol,
        CancellationToken ct = default);

    Task<BittradeNormalizedCall<BittradeOrderBookNormalized, JsonElement>> GetOrderBookCallAsync(
        string symbol,
        CancellationToken ct = default);

    Task<BittradeNormalizedCall<IReadOnlyList<BittradeExecutionNormalized>, JsonElement>> GetExecutionsCallAsync(
        string symbol,
        CancellationToken ct = default);
}
