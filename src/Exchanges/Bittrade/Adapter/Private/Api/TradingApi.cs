using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using CommonSymbol = ExchangeApi.Primitives.DomainCommon.Types.Symbol;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Primitives.Errors;
using ExchangeApi.Transport.Protocol;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;
using OrderRequest = ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests.OrderRequest;
using NormalizedRequests = ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Mappers;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Exchanges.Bittrade.Adapter.Internal.Operations;
namespace ExchangeApi.Exchanges.Bittrade.Adapter.Private.Api;

/// <summary>
/// Bittrade Private トレード/アカウント API（最小スコープ: Balance, Order, Cancel, OpenOrders, Status）。
/// </summary>
internal sealed class TradingApi
{
    private readonly NormalizedPrivateApi _trading;

    public TradingApi(NormalizedPrivateApi trading)
    {
        _trading = trading ?? throw new ArgumentNullException(nameof(trading));
    }

    public async Task<Call<OrderLimitRequest, OrderLimitResponse>> OrderLimitAsync(
        CommonSymbol symbol,
        Side side,
        Size size,
        Price price,
        CancellationToken cancellationToken = default)
    {
        var request = new OrderLimitRequest(symbol, side, size, price);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _trading
                .PostOrdersPlaceCallAsync(
                    new NormalizedRequests.PostOrdersPlaceRequest(
                        new OrderRequest(
                            Symbol: symbol,
                            Side: side,
                            OrderType: OrderType.Limit,
                            Size: size,
                            Price: price)),
                    cancellationToken)
                .ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                Operations.Trading.PlaceOrder,
                ok => new OrderLimitResponse(
                    Key: ok.Key,
                    ExchangeOrderId: ok.ExchangeOrderId,
                    AcceptanceId: ok.AcceptanceId));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<OrderLimitRequest, OrderLimitResponse>(
                request,
                startedAt,
                Operations.Trading.PlaceOrder,
                ex);
        }
    }

    public async Task<Call<CancelOrderRequest, CancelOrderResponse>> CancelOrderAsync(
        CommonSymbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default)
    {
        var request = new CancelOrderRequest(symbol, orderKey);
        var startedAt = DateTimeOffset.UtcNow;

        try
        {
            var call = await _trading
                .PostOrdersSubmitCancelByOrderIdCallAsync(symbol, orderKey, cancellationToken)
                .ConfigureAwait(false);
            return ApiCallMapper.MapCall(
                request,
                call,
                Operations.Trading.CancelOrder,
                ok => new CancelOrderResponse(ok.IsSuccess));
        }
        catch (Exception ex)
        {
            return ApiCallMapper.FromException<CancelOrderRequest, CancelOrderResponse>(
                request,
                startedAt,
                Operations.Trading.CancelOrder,
                ex);
        }
    }


}
