using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Wire.Public;

public interface IBitflyerWireExchangeInfoApi
{
    Task<WireCall> GetMarketsAsync(
        string? region = null,
        bool useAliasPath = false,
        CancellationToken cancellationToken = default);

    Task<WireCall> GetChatsAsync(
        string? fromDate = null,
        string? region = null,
        CancellationToken cancellationToken = default);
}
