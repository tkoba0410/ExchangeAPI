using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Facade.Interfaces;

/// <summary>
/// Public API (no signature). Market data + exchange info.
/// </summary>
public interface IPublicApi
{
    Task<Call<GetTickerRequest, GetTickerResponse>> GetTickerCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<GetBoardRequest, GetBoardResponse>> GetBoardCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<GetExecutionsPublicRequest, GetExecutionsPublicResponse>> GetExecutionsPublicCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<GetExchangeInfoRequest, GetExchangeInfoResponse>> GetExchangeInfoCallAsync(
        CancellationToken cancellationToken = default);
}
