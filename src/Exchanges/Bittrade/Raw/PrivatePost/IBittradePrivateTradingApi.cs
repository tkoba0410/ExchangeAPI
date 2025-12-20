using System.Threading;
using System.Threading.Tasks;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

/// <summary>
/// Bittrade Private REST API（取引系 POST）の Raw アクセス。
/// </summary>
internal interface IBittradePrivateTradingApi
{
    Task<PlaceOrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);

    Task<CancelOrderResponse> CancelOrderAsync(string orderId, CancellationToken cancellationToken = default);

    Task<CancelOrdersResponse> CancelOrdersAsync(CancelOrdersRequest request, CancellationToken cancellationToken = default);

    Task<CancelOpenOrdersResponse> CancelOpenOrdersAsync(CancelOpenOrdersRequest request, CancellationToken cancellationToken = default);

    Task<CreateWithdrawResponse> CreateWithdrawAsync(CreateWithdrawRequest request, CancellationToken cancellationToken = default);

    Task<CancelWithdrawResponse> CancelWithdrawAsync(string withdrawId, CancellationToken cancellationToken = default);

    Task<RetailOrderResponse> CreateRetailOrderAsync(CreateRetailOrderRequest request, CancellationToken cancellationToken = default);
}
