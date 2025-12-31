using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalize.Models;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Apis;

internal interface IBittradeNormalizedAccountApi
{
    Task<IReadOnlyList<BittradeBalanceEntryNormalized>> GetBalancesAsync(CancellationToken ct = default);
}
