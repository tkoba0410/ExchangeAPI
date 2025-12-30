using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Public;

internal interface IBitflyerWireExchangeInfoApi
{
    Task<IReadOnlyList<Market>> GetMarketsAsync(
        string? region = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Chat>> GetChatsAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken cancellationToken = default);
}
