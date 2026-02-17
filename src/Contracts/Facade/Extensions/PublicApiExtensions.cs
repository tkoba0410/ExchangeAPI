using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Facade.Extensions;

public static class PublicApiExtensions
{
    public static Task<Call<TickerRequest, TickerResponse>> GetTickerAsync(
        this IPublicApi api,
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        api.GetTickerAsync(new TickerRequest(symbol), cancellationToken);

    public static Task<Call<BoardRequest, BoardResponse>> GetBoardAsync(
        this IPublicApi api,
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        api.GetBoardAsync(new BoardRequest(symbol), cancellationToken);

    public static Task<Call<ExecutionsPublicRequest, ExecutionsPublicResponse>> GetExecutionsPublicAsync(
        this IPublicApi api,
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        api.GetExecutionsPublicAsync(new ExecutionsPublicRequest(symbol), cancellationToken);

}
