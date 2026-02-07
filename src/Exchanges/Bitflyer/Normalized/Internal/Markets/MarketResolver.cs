using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Markets;

public interface IMarketResolver
{
    Task<Call<ResolveMarketRequest, MarketInfo>> ResolveCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);
}
