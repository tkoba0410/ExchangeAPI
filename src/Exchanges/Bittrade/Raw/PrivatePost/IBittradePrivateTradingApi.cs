using System.Threading;
using System.Threading.Tasks;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

/// <summary>
/// Bittrade Private REST API（取引系 POST）の Raw アクセス。
/// </summary>
internal interface IBittradePrivateTradingApi
{
    Task<RawPlaceOrderResponse> CreateOrderAsync(RawCreateOrderRequest request, CancellationToken cancellationToken = default);

    Task<RawCancelOrderResponse> CancelOrderAsync(RawOrderId orderId, CancellationToken cancellationToken = default);

    Task<RawCancelOrdersResponse> CancelOrdersAsync(RawCancelOrdersRequest request, CancellationToken cancellationToken = default);

    Task<RawCancelOpenOrdersResponse> CancelOpenOrdersAsync(RawCancelOpenOrdersRequest request, CancellationToken cancellationToken = default);

    Task<RawCreateWithdrawResponse> CreateWithdrawAsync(RawCreateWithdrawRequest request, CancellationToken cancellationToken = default);

    Task<RawCancelWithdrawResponse> CancelWithdrawAsync(string withdrawId, CancellationToken cancellationToken = default);

    Task<RawRetailOrderResponse> CreateRetailOrderAsync(RawCreateRetailOrderRequest request, CancellationToken cancellationToken = default);
}
