using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Public;

internal interface IBitflyerWireExchangeInfoApi
{
    Task<WireResponse<IReadOnlyList<Market>>> GetMarketsAsync(
        string? region = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<WireResponse<IReadOnlyList<Chat>>> GetChatsAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken cancellationToken = default);
}
