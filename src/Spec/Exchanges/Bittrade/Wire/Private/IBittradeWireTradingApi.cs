using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private;

public interface IBittradeWireTradingApi
{
    Task<WireCall> PlaceOrderAsync(RawCreateOrderRequest request, CancellationToken ct = default);

    Task<WireCall> CancelOrderAsync(string orderId, CancellationToken ct = default);

    Task<WireCall> CancelOrdersAsync(RawCancelOrdersRequest request, CancellationToken ct = default);

    Task<WireCall> CancelOpenOrdersAsync(RawCancelOpenOrdersRequest request, CancellationToken ct = default);

    Task<WireCall> CreateWithdrawAsync(RawCreateWithdrawRequest request, CancellationToken ct = default);

    Task<WireCall> CancelWithdrawAsync(string withdrawId, CancellationToken ct = default);

    Task<WireCall> CreateRetailOrderAsync(RawCreateRetailOrderRequest request, CancellationToken ct = default);

    Task<WireCall> GetOpenOrdersAsync(string symbol, CancellationToken ct = default);

    Task<WireCall> GetOrderAsync(string orderId, CancellationToken ct = default);
}
