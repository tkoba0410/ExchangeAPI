using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;
using ExchangeApi.Exchanges.Bittrade.Raw;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private;

internal sealed class BittradeWireTradingApiNotSupported : IBittradeWireTradingApi
{
    private static NotSupportedException NotSupported() =>
        new("Bittrade wire trading is not supported.");

    public Task<WireCall> PlaceOrderAsync(RawCreateOrderRequest request, CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireCall> CancelOrderAsync(string orderId, CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireCall> CancelOrdersAsync(RawCancelOrdersRequest request, CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireCall> CancelOpenOrdersAsync(RawCancelOpenOrdersRequest request, CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireCall> CreateWithdrawAsync(RawCreateWithdrawRequest request, CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireCall> CancelWithdrawAsync(string withdrawId, CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireCall> CreateRetailOrderAsync(RawCreateRetailOrderRequest request, CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireCall> GetOpenOrdersAsync(string symbol, CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireCall> GetOrderAsync(string orderId, CancellationToken ct = default) =>
        throw NotSupported();
}
