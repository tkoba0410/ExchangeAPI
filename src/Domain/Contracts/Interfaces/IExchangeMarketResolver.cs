using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Contracts.Interfaces;

public interface IExchangeMarketResolver
{
    Task<Call<ResolveExchangeMarketRequest, ExchangeMarketInfo>> ResolveCallAsync(
        Symbol symbol,
        CancellationToken ct = default);
}
