using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Facade.Extensions;

public static class CandlesticksApiExtensions
{
    public static Task<Call<CandlesticksRequest, CandlesticksResponse>> GetCandlesticksAsync(
        this ICandlesticksApi api,
        Symbol symbol,
        Period period,
        int? size = null,
        CancellationToken cancellationToken = default) =>
        api.GetCandlesticksAsync(new CandlesticksRequest(symbol, period, size), cancellationToken);
}
