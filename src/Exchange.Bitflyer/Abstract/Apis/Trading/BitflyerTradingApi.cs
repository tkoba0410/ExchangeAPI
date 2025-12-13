using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Adapter.Bitflyer.Adapters;
using ExchangeApi.Adapter.Bitflyer.Models;
using ExchangeApi.Contracts.Contracts;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Errors;

namespace ExchangeApi.Adapter.Bitflyer.Apis.Trading;

/// <summary>
/// bitFlyer の Trading API 実装（REST）。
/// </summary>
public sealed class BitflyerTradingApi : ITradingApi
{
    private readonly IBitflyerPrivateTradingApi _privateTradingApi;
    private readonly IBitflyerPrivateApi _privateAccountApi;
    private readonly string _exchangeId;

    public BitflyerTradingApi(
        IBitflyerPrivateTradingApi privateTradingApi,
        IBitflyerPrivateApi privateAccountApi,
        string exchangeId = "bitFlyer")
    {
        _privateTradingApi = privateTradingApi ?? throw new ArgumentNullException(nameof(privateTradingApi));
        _privateAccountApi = privateAccountApi ?? throw new ArgumentNullException(nameof(privateAccountApi));
        _exchangeId = exchangeId;
    }

    public async Task<OrderResult> SendOrderAsync(
        OrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        BitflyerTradingMapper.ValidateOrderRequest(request);

        try
        {
            var dto = new BitflyerSendChildOrderRequest
            {
                ProductCode = request.ProductCode,
                Side = BitflyerCommonMapper.MapSideToExchange(request.Side),
                ChildOrderType = BitflyerTradingMapper.MapOrderType(request.OrderType, request.Price),
                Size = request.Size,
                Price = request.Price,
                TriggerPrice = request.TriggerPrice,
                MinuteToExpire = request.MinuteToExpire,
                TimeInForce = BitflyerTradingMapper.MapTimeInForce(request.TimeInForce),
            };

            var response = await _privateTradingApi
                .SendChildOrderAsync(dto, cancellationToken)
                .ConfigureAwait(false);

            return new OrderResult(response.ChildOrderAcceptanceId, request.ClientOrderId);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchangeId, "SendOrder");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer sendchildorder API.",
                exchangeId: _exchangeId,
                operation: "SendOrder",
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<CancelResult> CancelOrderAsync(
        string productCode,
        string childOrderAcceptanceId,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        if (string.IsNullOrWhiteSpace(childOrderAcceptanceId))
        {
            throw new ArgumentException("childOrderAcceptanceId is required.", nameof(childOrderAcceptanceId));
        }

        try
        {
            var dto = new BitflyerCancelChildOrderRequest
            {
                ProductCode = productCode,
                ChildOrderAcceptanceId = childOrderAcceptanceId,
            };

            var response = await _privateTradingApi
                .CancelChildOrderAsync(dto, cancellationToken)
                .ConfigureAwait(false);

            if (response is null)
            {
                throw new ExchangeApiException(
                    message: "bitFlyer cancelchildorder returned no response.",
                    exchangeId: _exchangeId,
                    operation: "CancelOrder",
                    statusCode: null);
            }

            return new CancelResult(true);
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchangeId, "CancelOrder");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer cancelchildorder API.",
                exchangeId: _exchangeId,
                operation: "CancelOrder",
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<IReadOnlyList<OpenOrder>> GetOpenOrdersAsync(
        string productCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        try
        {
            var rawOrders = await _privateAccountApi
                .GetChildOrdersAsync(productCode, childOrderState: "ACTIVE", childOrderAcceptanceId: null, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var mapped = rawOrders.Select(o => new OpenOrder(
                ProductCode: o.ProductCode,
                OrderId: o.ChildOrderId,
                OrderAcceptanceId: o.ChildOrderAcceptanceId,
                Side: BitflyerCommonMapper.MapSide(o.Side),
                OrderType: BitflyerTradingMapper.MapOrderTypeFromExchange(o.ChildOrderType),
                Size: o.Size,
                OutstandingSize: o.OutstandingSize,
                ExecutedSize: o.ExecutedSize,
                Price: o.Price == 0 ? null : o.Price,
                ClientOrderId: null)).ToArray();

            return mapped;
        }
        catch (ExchangeApiException ex)
        {
            throw BitflyerErrorMapper.EnrichBitflyerException(ex, _exchangeId, "GetOpenOrders");
        }
        catch (Exception ex)
        {
            throw new ExchangeApiException(
                message: "Failed to call bitFlyer getchildorders API.",
                exchangeId: _exchangeId,
                operation: "GetOpenOrders",
                statusCode: null,
                innerException: ex);
        }
    }

    public async Task<OrderStatus> PollOrderStatusAsync(
        string productCode,
        string childOrderAcceptanceId,
        TimeSpan? pollInterval = null,
        int maxAttempts = 30,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(productCode))
        {
            throw new ArgumentException("productCode is required.", nameof(productCode));
        }

        if (string.IsNullOrWhiteSpace(childOrderAcceptanceId))
        {
            throw new ArgumentException("childOrderAcceptanceId is required.", nameof(childOrderAcceptanceId));
        }

        var interval = pollInterval ?? TimeSpan.FromSeconds(1);

        for (var attempt = 0; attempt < maxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var orders = await _privateAccountApi
                .GetChildOrdersAsync(productCode, childOrderState: null, childOrderAcceptanceId, cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var order = orders.FirstOrDefault();

            if (order is null)
            {
                return new OrderStatus(
                    ProductCode: productCode,
                    OrderAcceptanceId: childOrderAcceptanceId,
                    Status: OrderStatusType.Completed,
                    ExecutedSize: 0m,
                    OutstandingSize: 0m,
                    Price: null,
                    AveragePrice: null);
            }

            var status = BitflyerCommonMapper.MapOrderStatusType(order.ChildOrderState);
            var mapped = new OrderStatus(
                ProductCode: order.ProductCode,
                OrderAcceptanceId: order.ChildOrderAcceptanceId,
                Status: status,
                ExecutedSize: order.ExecutedSize,
                OutstandingSize: order.OutstandingSize,
                Price: order.Price == 0 ? null : order.Price,
                AveragePrice: order.AveragePrice == 0 ? null : order.AveragePrice);

            if (status is OrderStatusType.Completed or OrderStatusType.Canceled or OrderStatusType.Expired)
            {
                return mapped;
            }

            if (attempt == maxAttempts - 1)
            {
                return mapped with { Status = OrderStatusType.Active };
            }

            await Task.Delay(interval, cancellationToken).ConfigureAwait(false);
        }

        throw new InvalidOperationException("Polling loop exited unexpectedly.");
    }
}
