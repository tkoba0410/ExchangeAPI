using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Facade.Interfaces;

/// <summary>
/// Private API (signature required). Trading + account + spot history.
/// </summary>
public interface IPrivateApi
{
    Task<Call<PlaceLimitOrderRequest, OrderResult>> OrderLimitCallAsync(
        Symbol symbol,
        Side side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default);

    Task<Call<CancelOrderRequest, CancelResult>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default);

    Task<Call<GetBalancesRequest, IReadOnlyList<Balance>>> GetBalanceCallAsync(
        CancellationToken cancellationToken = default);

    Task<Call<MarketLimitCursorRequest, Page<OrderSnapshotItem>>> GetOrdersCallAsync(
        MarketLimitCursorRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<MarketLimitCursorRequest, Page<ExecutionItem>>> GetExecutionsPrivateCallAsync(
        MarketLimitCursorRequest request,
        CancellationToken cancellationToken = default);
}
