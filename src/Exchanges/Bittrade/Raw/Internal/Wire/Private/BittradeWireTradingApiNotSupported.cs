using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private.Requests;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private;

internal sealed class BittradeWireTradingApiNotSupported : IBittradeWireTradingApi
{
    private static NotSupportedException NotSupported() =>
        new("Bittrade wire trading is not supported.");

    public Task<BittradeWireOrder> PlaceOrderAsync(BittradeWireCreateOrderRequest request, CancellationToken ct = default) =>
        throw NotSupported();

    public Task CancelOrderAsync(string orderId, CancellationToken ct = default) =>
        throw NotSupported();

    public Task<IReadOnlyList<BittradeWireOpenOrder>> GetOpenOrdersAsync(string symbol, CancellationToken ct = default) =>
        throw NotSupported();

    public Task<BittradeWireOrder> GetOrderAsync(string orderId, CancellationToken ct = default) =>
        throw NotSupported();
}
