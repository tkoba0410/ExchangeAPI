using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Internal.Wire.Private;

public interface IBittradeWireTradingApi
{
    Task<WireResponse> PlaceOrderAsync(RawCreateOrderRequest request, CancellationToken ct = default);

    Task<WireResponse> CancelOrderAsync(string orderId, CancellationToken ct = default);

    Task<WireResponse> CancelOrdersAsync(RawCancelOrdersRequest request, CancellationToken ct = default);

    Task<WireResponse> CancelOpenOrdersAsync(RawCancelOpenOrdersRequest request, CancellationToken ct = default);

    Task<WireResponse> CreateWithdrawAsync(RawCreateWithdrawRequest request, CancellationToken ct = default);

    Task<WireResponse> CancelWithdrawAsync(string withdrawId, CancellationToken ct = default);

    Task<WireResponse> CreateRetailOrderAsync(RawCreateRetailOrderRequest request, CancellationToken ct = default);

    Task<WireResponse> GetOpenOrdersAsync(string symbol, CancellationToken ct = default);

    Task<WireResponse> GetOrderAsync(string orderId, CancellationToken ct = default);
}
