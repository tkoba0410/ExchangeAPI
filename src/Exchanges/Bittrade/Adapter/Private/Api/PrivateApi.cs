using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Private.Api;

internal sealed class PrivateApi
{
    private readonly TradingApi _trading;
    private readonly AccountApi _account;
    private readonly SpotHistoryApi _history;

    public PrivateApi(NormalizedPrivateApi normalized)
    {
        if (normalized is null) throw new ArgumentNullException(nameof(normalized));
        _trading = new TradingApi(normalized);
        _account = new AccountApi(normalized);
        _history = new SpotHistoryApi(normalized);
    }

    internal PrivateApi(
        TradingApi trading,
        AccountApi account,
        SpotHistoryApi history)
    {
        _trading = trading ?? throw new ArgumentNullException(nameof(trading));
        _account = account ?? throw new ArgumentNullException(nameof(account));
        _history = history ?? throw new ArgumentNullException(nameof(history));
    }

    public Task<Call<BalanceRequest, BalanceResponse>> GetBalanceAsync(
        BalanceRequest request,
        CancellationToken cancellationToken = default) =>
        _account.GetBalanceAsync(request, cancellationToken);

    public Task<Call<OrderLimitRequest, OrderLimitResponse>> OrderLimitAsync(
        OrderLimitRequest request,
        CancellationToken cancellationToken = default) =>
        _trading.OrderLimitAsync(request, cancellationToken);

    public Task<Call<CancelOrderRequest, CancelOrderResponse>> CancelOrderAsync(
        CancelOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _trading.CancelOrderAsync(request, cancellationToken);

    public Task<Call<OrdersRequest, OrdersResponse>> GetOrdersAsync(
        OrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _history.GetOrdersAsync(request, cancellationToken);

    public Task<Call<ExecutionsPrivateRequest, ExecutionsPrivateResponse>> GetExecutionsPrivateAsync(
        ExecutionsPrivateRequest request,
        CancellationToken cancellationToken = default) =>
        _history.GetExecutionsPrivateAsync(request, cancellationToken);
}
