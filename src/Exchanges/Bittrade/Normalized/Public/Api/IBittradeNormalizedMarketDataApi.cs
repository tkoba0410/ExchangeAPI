using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;

internal interface IBittradeNormalizedMarketDataApi
{
    Task<Call<GetTickerRequest, BittradeTickerNormalized>> GetDetailMergedCallAsync(
        string productCode,
        CancellationToken ct = default);

    Task<Call<GetOrderBookRequest, BittradeOrderBookNormalized>> GetDepthCallAsync(
        string productCode,
        BittradeDepthType? depthType = null,
        CancellationToken ct = default);

    Task<Call<GetExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetTradeCallAsync(
        string productCode,
        CancellationToken ct = default);

    Task<Call<GetHistoryKlineRequest, IReadOnlyList<BittradeKlineNormalized>>> GetHistoryKlineCallAsync(
        string productCode,
        string period,
        int? size = null,
        CancellationToken ct = default);

    Task<Call<GetTickersRequest, IReadOnlyList<BittradeTickerEntryNormalized>>> GetTickersCallAsync(
        CancellationToken ct = default);

    Task<Call<GetHistoryTradeRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetHistoryTradeCallAsync(
        string productCode,
        CancellationToken ct = default);
}
