using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Api;

internal interface IBittradeRawTradingApi
{
    Task<Call<CreateOrderRequest, RawPlaceOrderResponse>> PostOrdersPlaceCallAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelOrderRequest, RawCancelOrderResponse>> PostOrdersSubmitCancelByOrderIdCallAsync(
        CancelOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelOrdersRequest, RawCancelOrdersResponse>> PostOrdersBatchCancelCallAsync(
        CancelOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelOpenOrdersRequest, RawCancelOpenOrdersResponse>> PostOrdersBatchCancelOpenOrdersCallAsync(
        CancelOpenOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CreateWithdrawRequest, RawCreateWithdrawResponse>> PostWithdrawApiCreateCallAsync(
        CreateWithdrawRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelWithdrawRequest, RawCancelWithdrawResponse>> PostWithdrawVirtualCancelByWithdrawIdCallAsync(
        CancelWithdrawRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CreateRetailOrderRequest, RawRetailOrderResponse>> PostOrderPlaceCallAsync(
        CreateRetailOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetOpenOrdersRequest, RawOpenOrdersResponse>> GetOpenOrdersCallAsync(
        GetOpenOrdersRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetOrderRequest, RawOrderDetailResponse>> GetOrdersByOrderIdCallAsync(
        GetOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<GetMatchResultsRequest, RawMatchResultsResponse>> GetMatchResultsCallAsync(
        GetMatchResultsRequest request,
        CancellationToken cancellationToken = default);
}
