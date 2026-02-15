using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Operations;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;
using OrderRequest = ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests.OrderRequest;
using NormalizedRequests = ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Mappers;
using ExchangeApi.Exchanges.Common.Application.Adapter.Internal;
using ExchangeApi.Primitives.CallCommon;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Private.Api;

/// <summary>
/// Bittrade Private トレード/アカウント API（最小スコープ: Balance, Order, Cancel, OpenOrders, Status）。
/// </summary>
internal sealed class TradingApi
{
    private static readonly string OpPlaceOrder = OperationComponent.WithExchange("Bittrade", ContractOperations.Trading.PlaceOrder);
    private static readonly string OpCancelOrder = OperationComponent.WithExchange("Bittrade", ContractOperations.Trading.CancelOrder);

    private readonly NormalizedPrivateApi _trading;

    public TradingApi(NormalizedPrivateApi trading)
    {
        _trading = trading ?? throw new ArgumentNullException(nameof(trading));
    }

    public async Task<Call<OrderLimitRequest, OrderLimitResponse>> OrderLimitAsync(
        OrderLimitRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        return await AdapterCallExecutor.ExecuteMapCallAsync(
                request,
                OpPlaceOrder,
                ct => _trading.PostOrdersPlaceCallAsync(
                    new NormalizedRequests.PostOrdersPlaceRequest(
                        new OrderRequest(
                            Symbol: request.Symbol,
                            Side: request.Side,
                            OrderType: OrderType.Limit,
                            Size: request.Size,
                            Price: request.Price)),
                    ct),
                ok => new OrderLimitResponse(
                    Key: ok.Key,
                    ExchangeOrderId: ok.ExchangeOrderId,
                    AcceptanceId: ok.AcceptanceId),
                cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<Call<CancelOrderRequest, CancelOrderResponse>> CancelOrderAsync(
        CancelOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        return await AdapterCallExecutor.ExecuteMapCallAsync(
                request,
                OpCancelOrder,
                ct => _trading.PostOrdersSubmitCancelByOrderIdCallAsync(
                    new NormalizedRequests.PostOrdersSubmitCancelByOrderIdRequest(request.Symbol, request.OrderKey),
                    ct),
                ok => new CancelOrderResponse(ok.IsSuccess),
                cancellationToken)
            .ConfigureAwait(false);
    }


}
