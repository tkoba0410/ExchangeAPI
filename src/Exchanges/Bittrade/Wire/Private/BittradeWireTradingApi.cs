using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Common.Enums;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Wire.Private.Mappers;
using ExchangeApi.Exchanges.Bittrade.Wire.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Wire.Private.Requests;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Private;

internal sealed class BittradeWireTradingApi : IBittradeWireTradingApi
{
    private const string OkStatus = "ok";
    private readonly IBittradeRawTradingApi _raw;
    private readonly string _accountId;

    public BittradeWireTradingApi(IBittradeRawTradingApi raw, string accountId)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
        _accountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
    }

    public async Task<BittradeWireOrder> PlaceOrderAsync(BittradeWireCreateOrderRequest request, CancellationToken ct = default)
    {
        var operation = BittradeWireOperations.Trading.PlaceOrder;

        CreateOrderRequest rawRequest;
        try
        {
            rawRequest = BittradeWireTradingMapper.ToRaw(_accountId, request);
        }
        catch (FormatException ex)
        {
            throw new ExchangeApiException(
                $"{operation}: {ex.Message}",
                exchange: ExchangeCode.Bittrade,
                operation: operation,
                errorCategory: ExchangeErrorCategory.Unknown);
        }

        var raw = await _raw.CreateOrderAsync(rawRequest, ct).ConfigureAwait(false);
        RequireOk(raw.Status, operation);

        return BittradeWireTradingMapper.ToWire(raw, request);
    }

    public async Task CancelOrderAsync(string orderId, CancellationToken ct = default)
    {
        var operation = BittradeWireOperations.Trading.CancelOrder;
        var raw = await _raw.CancelOrderAsync(new OrderId(orderId), ct).ConfigureAwait(false);
        RequireOk(raw.Status, operation);
    }

    public async Task<IReadOnlyList<BittradeWireOpenOrder>> GetOpenOrdersAsync(string symbol, CancellationToken ct = default)
    {
        var operation = BittradeWireOperations.Trading.GetOpenOrders;
        var raw = await _raw.GetOpenOrdersAsync(Symbol.From(symbol), _accountId, ct).ConfigureAwait(false);
        RequireOk(raw.Status, operation);

        if (raw.Data is null)
        {
            throw new ExchangeApiException(
                $"{operation}: missing data.",
                exchange: ExchangeCode.Bittrade,
                operation: operation,
                errorCategory: ExchangeErrorCategory.Unknown);
        }

        return raw.Data.Select(MapOpenOrder).ToArray();
    }

    public async Task<BittradeWireOrder> GetOrderAsync(string orderId, CancellationToken ct = default)
    {
        var operation = BittradeWireOperations.Trading.GetOrder;
        var raw = await _raw.GetOrderAsync(new OrderId(orderId), ct).ConfigureAwait(false);
        RequireOk(raw.Status, operation);

        var data = raw.Data ?? throw new ExchangeApiException(
            $"{operation}: missing data.",
            exchange: ExchangeCode.Bittrade,
            operation: operation,
            errorCategory: ExchangeErrorCategory.Unknown);

        try
        {
            return BittradeWireTradingMapper.ToWire(data);
        }
        catch (FormatException ex)
        {
            throw new ExchangeApiException(
                $"{operation}: {ex.Message}",
                exchange: ExchangeCode.Bittrade,
                operation: operation,
                errorCategory: ExchangeErrorCategory.Unknown);
        }
    }

    private static BittradeWireOpenOrder MapOpenOrder(OrderSummary raw)
    {
        try
        {
            return BittradeWireTradingMapper.ToWireOpenOrder(raw);
        }
        catch (FormatException ex)
        {
            throw new ExchangeApiException(
                $"{BittradeWireOperations.Trading.GetOpenOrders}: {ex.Message}",
                exchange: ExchangeCode.Bittrade,
                operation: BittradeWireOperations.Trading.GetOpenOrders,
                errorCategory: ExchangeErrorCategory.Unknown);
        }
    }

    private static void RequireOk(string? status, string operation)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            throw new ExchangeApiException(
                $"{operation}: status is missing.",
                exchange: ExchangeCode.Bittrade,
                operation: operation,
                errorCategory: ExchangeErrorCategory.Unknown);
        }

        if (!string.Equals(status, OkStatus, StringComparison.OrdinalIgnoreCase))
        {
            throw new ExchangeApiException(
                $"{operation}: status={status}.",
                exchange: ExchangeCode.Bittrade,
                operation: operation,
                errorCategory: ExchangeErrorCategory.Request);
        }
    }
}
