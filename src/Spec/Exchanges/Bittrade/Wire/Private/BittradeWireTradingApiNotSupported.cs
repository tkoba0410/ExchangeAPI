using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private.Requests;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private;

internal sealed class BittradeWireTradingApiNotSupported : IBittradeWireTradingApi
{
    private static NotSupportedException NotSupported() =>
        new("Bittrade wire trading is not supported.");

    public Task<WireResponse<BittradeWireOrder>> PlaceOrderAsync(BittradeWireCreateOrderRequest request, CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireResponse<bool>> CancelOrderAsync(string orderId, CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireResponse<IReadOnlyList<BittradeWireOpenOrder>>> GetOpenOrdersAsync(string symbol, CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireResponse<BittradeWireOrder>> GetOrderAsync(string orderId, CancellationToken ct = default) =>
        throw NotSupported();
}
