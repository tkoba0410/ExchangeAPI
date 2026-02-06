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
    Task<Call<TickerRequest, TickerResponse>> GetTickerAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<BoardRequest, BoardResponse>> GetBoardAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<ExecutionsPublicRequest, ExecutionsPublicResponse>> GetExecutionsPublicAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default);

    Task<Call<ExchangeInfoRequest, ExchangeInfoResponse>> GetExchangeInfoAsync(
        CancellationToken cancellationToken = default);
}
