using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Contracts.Facade.Interfaces;

public interface ICandlesticksApi
{
    Task<Call<CandlesticksRequest, CandlesticksResponse>> GetCandlesticksAsync(
        CandlesticksRequest request,
        CancellationToken cancellationToken = default);
}
