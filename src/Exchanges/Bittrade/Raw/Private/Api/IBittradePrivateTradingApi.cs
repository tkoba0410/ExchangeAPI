using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Api;

/// <summary>
/// Bittrade Private REST API（取引系 POST）の Raw アクセス。
/// </summary>
internal interface IBittradePrivateTradingApi
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

    Task<Call<CreateWithdrawVirtualByAddressIdRequest, RawCreateWithdrawResponse>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        CreateWithdrawVirtualByAddressIdRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelWithdrawRequest, RawCancelWithdrawResponse>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        CancelWithdrawRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<PlaceWithdrawVirtualRequest, RawCreateWithdrawResponse>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        PlaceWithdrawVirtualRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CreateRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderPlaceCallAsync(
        CreateRetailOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CancelRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderCancelByOrderIdCallAsync(
        CancelRetailOrderRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<PostRetailOrderHistoryRequest, RawRetailOrdersResponse>> PostRetailOrderHistoryCallAsync(
        PostRetailOrderHistoryRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<PostRetailOrderDetailRequest, RawRetailOrderDetailResponse>> PostRetailOrderDetailCallAsync(
        PostRetailOrderDetailRequest request,
        CancellationToken cancellationToken = default);

    Task<Call<CreateRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderCreateCallAsync(
        CreateRetailOrderRequest request,
        CancellationToken cancellationToken = default);
}
