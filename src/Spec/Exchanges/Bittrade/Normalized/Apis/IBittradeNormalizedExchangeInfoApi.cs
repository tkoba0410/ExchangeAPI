using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalize;
using ExchangeApi.Exchanges.Bittrade.Normalize.Models;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Apis;

internal interface IBittradeNormalizedExchangeInfoApi
{
    Task<IReadOnlyList<BittradeSymbolNormalized>> GetSymbolsAsync(CancellationToken ct = default);

    Task<BittradeNormalizedCall<IReadOnlyList<BittradeSymbolNormalized>, JsonElement>> GetSymbolsCallAsync(
        CancellationToken ct = default);
}
