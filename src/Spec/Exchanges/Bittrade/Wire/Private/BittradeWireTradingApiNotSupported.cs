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

    public Task<WireResponse> PlaceOrderAsync(RawCreateOrderRequest request, CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireResponse> CancelOrderAsync(string orderId, CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireResponse> CancelOrdersAsync(RawCancelOrdersRequest request, CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireResponse> CancelOpenOrdersAsync(RawCancelOpenOrdersRequest request, CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireResponse> CreateWithdrawAsync(RawCreateWithdrawRequest request, CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireResponse> CancelWithdrawAsync(string withdrawId, CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireResponse> CreateRetailOrderAsync(RawCreateRetailOrderRequest request, CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireResponse> GetOpenOrdersAsync(string symbol, CancellationToken ct = default) =>
        throw NotSupported();

    public Task<WireResponse> GetOrderAsync(string orderId, CancellationToken ct = default) =>
        throw NotSupported();
}
