using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;

internal interface IBittradeNormalizedExchangeInfoApi
{
    Task<Call<GetSymbolsRequest, IReadOnlyList<BittradeSymbolNormalized>>> GetSymbolsCallAsync(
        CancellationToken ct = default);

    Task<Call<GetCurrencysRequest, IReadOnlyList<string>>> GetCurrencysCallAsync(
        CancellationToken ct = default);

    Task<Call<GetTimestampRequest, DateTimeOffset>> GetTimestampCallAsync(
        CancellationToken ct = default);
}
