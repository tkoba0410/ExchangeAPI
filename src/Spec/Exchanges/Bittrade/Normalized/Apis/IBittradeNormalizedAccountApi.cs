using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalize;
using ExchangeApi.Exchanges.Bittrade.Normalize.Models;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Apis;

internal interface IBittradeNormalizedAccountApi
{
    Task<IReadOnlyList<BittradeBalanceEntryNormalized>> GetBalancesAsync(CancellationToken ct = default);

    Task<BittradeNormalizedCall<IReadOnlyList<BittradeBalanceEntryNormalized>, JsonElement>> GetBalancesCallAsync(
        CancellationToken ct = default);
}
