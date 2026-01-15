using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalized.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Types;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Apis;

internal interface IBittradeNormalizedMarketDataApi
{
    Task<Call<GetTickerRequest, BittradeTickerNormalized>> GetTickerCallAsync(
        BittradeSymbol symbol,
        CancellationToken ct = default);

    Task<Call<GetOrderBookRequest, BittradeOrderBookNormalized>> GetOrderBookCallAsync(
        BittradeSymbol symbol,
        BittradeDepthType? depthType = null,
        CancellationToken ct = default);

    Task<Call<GetExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetExecutionsCallAsync(
        BittradeSymbol symbol,
        CancellationToken ct = default);
}
