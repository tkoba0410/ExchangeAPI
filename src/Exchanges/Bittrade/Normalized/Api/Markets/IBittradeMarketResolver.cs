using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Api.Markets;

public interface IBittradeMarketResolver
{
    Task<Call<ResolveBittradeMarketRequest, MarketInfo>> ResolveCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);
}
