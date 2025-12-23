using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bittrade.Wire.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Wire.Private.Requests;

namespace ExchangeApi.Exchanges.Bittrade.Wire.Private;

internal sealed class BittradeWireTradingApiNotSupported : IBittradeWireTradingApi
{
    private static ExchangeFeatureNotSupportedException NotSupported() =>
        new(ExchangeCode.Bittrade, "WireTrading");

    public Task<BittradeWireOrder> PlaceOrderAsync(BittradeWireCreateOrderRequest request, CancellationToken ct = default) =>
        throw NotSupported();

    public Task CancelOrderAsync(string orderId, CancellationToken ct = default) =>
        throw NotSupported();

    public Task<IReadOnlyList<BittradeWireOpenOrder>> GetOpenOrdersAsync(string symbol, CancellationToken ct = default) =>
        throw NotSupported();

    public Task<BittradeWireOrder> GetOrderAsync(string orderId, CancellationToken ct = default) =>
        throw NotSupported();
}
