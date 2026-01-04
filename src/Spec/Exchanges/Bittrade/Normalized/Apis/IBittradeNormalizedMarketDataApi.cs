using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalize.Models;
using ExchangeApi.Exchanges.Bittrade.Normalize.Requests;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Apis;

internal interface IBittradeNormalizedMarketDataApi
{
    Task<BittradeTickerNormalized> GetTickerAsync(string symbol, CancellationToken ct = default);
    Task<BittradeOrderBookNormalized> GetOrderBookAsync(string symbol, CancellationToken ct = default);
    Task<IReadOnlyList<BittradeExecutionNormalized>> GetExecutionsAsync(string symbol, CancellationToken ct = default);

    Task<Call<GetTickerRequest, BittradeTickerNormalized>> GetTickerCallAsync(
        string symbol,
        CancellationToken ct = default);

    Task<Call<GetOrderBookRequest, BittradeOrderBookNormalized>> GetOrderBookCallAsync(
        string symbol,
        CancellationToken ct = default);

    Task<Call<GetExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetExecutionsCallAsync(
        string symbol,
        CancellationToken ct = default);
}
