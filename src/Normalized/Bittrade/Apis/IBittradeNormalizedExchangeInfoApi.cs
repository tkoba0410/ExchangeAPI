using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalize.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalize.Requests;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Apis;

internal interface IBittradeNormalizedExchangeInfoApi
{
    Task<Call<GetSymbolsRequest, IReadOnlyList<BittradeSymbolNormalized>>> GetSymbolsCallAsync(
        CancellationToken ct = default);
}
