using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalize.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalize.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalize.Types;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Apis;

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
