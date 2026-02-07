using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Facade.Extensions;

public static class PrivateApiExtensions
{
    public static Task<Call<OrderLimitRequest, OrderLimitResponse>> OrderLimitAsync(
        this IPrivateApi api,
        Symbol symbol,
        Side side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default) =>
        api.OrderLimitAsync(new OrderLimitRequest(symbol, side, size, price), cancellationToken);

    public static Task<Call<CancelOrderRequest, CancelOrderResponse>> CancelOrderAsync(
        this IPrivateApi api,
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        api.CancelOrderAsync(new CancelOrderRequest(symbol, orderKey), cancellationToken);

    public static Task<Call<BalanceRequest, BalanceResponse>> GetBalanceAsync(
        this IPrivateApi api,
        CancellationToken cancellationToken = default) =>
        api.GetBalanceAsync(new BalanceRequest(), cancellationToken);

    public static Task<Call<OrdersRequest, OrdersResponse>> GetOrdersAsync(
        this IPrivateApi api,
        Symbol market,
        int? limit = null,
        Cursor? cursor = null,
        CancellationToken cancellationToken = default) =>
        api.GetOrdersAsync(new OrdersRequest(market, limit, cursor), cancellationToken);

    public static Task<Call<ExecutionsPrivateRequest, ExecutionsPrivateResponse>> GetExecutionsPrivateAsync(
        this IPrivateApi api,
        Symbol market,
        int? limit = null,
        Cursor? cursor = null,
        CancellationToken cancellationToken = default) =>
        api.GetExecutionsPrivateAsync(new ExecutionsPrivateRequest(market, limit, cursor), cancellationToken);
}
