using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Converters;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private.Requests;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private;

internal sealed class BittradeWireTradingApi : IBittradeWireTradingApi
{
    private const ExchangeCode Exchange = ExchangeCode.Bittrade;
    private readonly IBittradeRawTradingApi _raw;
    private readonly string _accountId;

    public BittradeWireTradingApi(IBittradeRawTradingApi raw, string accountId)
    {
        _raw = raw ?? throw new ArgumentNullException(nameof(raw));
        _accountId = accountId ?? throw new ArgumentNullException(nameof(accountId));
    }

    public async Task<WireResponse<BittradeWireOrder>> PlaceOrderAsync(BittradeWireCreateOrderRequest request, CancellationToken ct = default)
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

        var response = BittradeWireTradingMapper.ToWire(raw, request);
        return new WireResponse<BittradeWireOrder>(Exchange, response);
    }

    public async Task<WireResponse<bool>> CancelOrderAsync(string orderId, CancellationToken ct = default)
    {
        var operation = BittradeWireOperations.Trading.CancelOrder;
        var raw = await _raw.CancelOrderAsync(new RawOrderId(orderId), ct).ConfigureAwait(false);
        BittradeWireErrors.RequireOk(raw.Status, null, null, operation);
        return new WireResponse<bool>(Exchange, true);
    }

    public async Task<WireResponse<IReadOnlyList<BittradeWireOpenOrder>>> GetOpenOrdersAsync(string symbol, CancellationToken ct = default)
    {
        var operation = BittradeWireOperations.Trading.GetOpenOrders;
        var raw = await _raw.GetOpenOrdersAsync(RawSymbol.From(symbol), _accountId, ct).ConfigureAwait(false);
        BittradeWireErrors.RequireOk(raw.Status, null, null, operation);

        if (raw.Data is null)
        {
            throw BittradeWireErrors.Missing(operation, "data");
        }

        var response = raw.Data.Select(MapOpenOrder).ToArray();
        return new WireResponse<IReadOnlyList<BittradeWireOpenOrder>>(Exchange, response);
    }

    public async Task<WireResponse<BittradeWireOrder>> GetOrderAsync(string orderId, CancellationToken ct = default)
    {
        var operation = BittradeWireOperations.Trading.GetOrder;
        var raw = await _raw.GetOrderAsync(new RawOrderId(orderId), ct).ConfigureAwait(false);
        BittradeWireErrors.RequireOk(raw.Status, null, null, operation);

        var data = raw.Data ?? throw BittradeWireErrors.Missing(operation, "data");

        try
        {
            var response = BittradeWireTradingMapper.ToWire(data);
            return new WireResponse<BittradeWireOrder>(Exchange, response);
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
