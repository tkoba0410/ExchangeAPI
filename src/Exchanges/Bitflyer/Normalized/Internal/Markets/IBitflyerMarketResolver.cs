using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Markets;

public interface IBitflyerMarketResolver
{
    Task<Call<ResolveBitflyerMarketRequest, BitflyerMarketInfo>> ResolveCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);
}
