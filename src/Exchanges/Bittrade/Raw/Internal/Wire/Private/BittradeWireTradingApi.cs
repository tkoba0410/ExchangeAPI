using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Converters;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private.Requests;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private;

internal sealed class BittradeWireTradingApi : IBittradeWireTradingApi
{
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

        RawCreateOrderRequest rawRequest;
        try
        {
            rawRequest = BittradeWireTradingMapper.ToRaw(_accountId, request);
        }
        catch (FormatException ex)
        {
            throw BittradeWireErrors.UnexpectedMessage(operation, ex.Message);
        }

        var raw = await _raw.CreateOrderAsync(rawRequest, ct).ConfigureAwait(false);
        BittradeWireErrors.RequireOk(raw.Status, null, null, operation);

        return BittradeWireTradingMapper.ToWire(raw, request);
    }

    public async Task CancelOrderAsync(string orderId, CancellationToken ct = default)
    {
        var operation = BittradeWireOperations.Trading.CancelOrder;
        var raw = await _raw.CancelOrderAsync(new RawOrderId(orderId), ct).ConfigureAwait(false);
        BittradeWireErrors.RequireOk(raw.Status, null, null, operation);
    }

    public async Task<IReadOnlyList<BittradeWireOpenOrder>> GetOpenOrdersAsync(string symbol, CancellationToken ct = default)
    {
        var operation = BittradeWireOperations.Trading.GetOpenOrders;
        var raw = await _raw.GetOpenOrdersAsync(RawSymbol.From(symbol), _accountId, ct).ConfigureAwait(false);
        BittradeWireErrors.RequireOk(raw.Status, null, null, operation);

        if (raw.Data is null)
        {
            throw BittradeWireErrors.Missing(operation, "data");
        }

        return raw.Data.Select(MapOpenOrder).ToArray();
    }

    public async Task<BittradeWireOrder> GetOrderAsync(string orderId, CancellationToken ct = default)
    {
        var operation = BittradeWireOperations.Trading.GetOrder;
        var raw = await _raw.GetOrderAsync(new RawOrderId(orderId), ct).ConfigureAwait(false);
        BittradeWireErrors.RequireOk(raw.Status, null, null, operation);

        var data = raw.Data ?? throw BittradeWireErrors.Missing(operation, "data");

        try
        {
            return BittradeWireTradingMapper.ToWire(data);
        }
        catch (FormatException ex)
        {
            throw BittradeWireErrors.UnexpectedMessage(operation, ex.Message);
        }
    }

    private static BittradeWireOpenOrder MapOpenOrder(RawOrderSummary raw)
    {
        try
        {
            return BittradeWireTradingMapper.ToWireOpenOrder(raw);
        }
        catch (FormatException ex)
        {
            throw BittradeWireErrors.UnexpectedMessage(BittradeWireOperations.Trading.GetOpenOrders, ex.Message);
        }
    }
}
