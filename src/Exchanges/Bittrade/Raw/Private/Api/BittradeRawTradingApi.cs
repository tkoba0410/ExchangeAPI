using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Api;

internal sealed class BittradeRawTradingApi : IBittradeRawTradingApi
{
    private readonly IBittradePrivateApi _privateApi;
    private readonly IBittradePrivateTradingApi _privateTradingApi;

    public BittradeRawTradingApi(
        IBittradePrivateApi privateApi,
        IBittradePrivateTradingApi privateTradingApi)
    {
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
        _privateTradingApi = privateTradingApi ?? throw new ArgumentNullException(nameof(privateTradingApi));
    }

    public Task<Call<CreateOrderRequest, RawPlaceOrderResponse>> PostOrdersPlaceCallAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.PostOrdersPlaceCallAsync(request, cancellationToken);

    public Task<Call<CancelOrderRequest, RawCancelOrderResponse>> PostOrdersSubmitCancelByOrderIdCallAsync(
        CancelOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.PostOrdersSubmitCancelByOrderIdCallAsync(request, cancellationToken);

    public Task<Call<CancelOrdersRequest, RawCancelOrdersResponse>> PostOrdersBatchCancelCallAsync(
        CancelOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.PostOrdersBatchCancelCallAsync(request, cancellationToken);

    public Task<Call<CancelOpenOrdersRequest, RawCancelOpenOrdersResponse>> PostOrdersBatchCancelOpenOrdersCallAsync(
        CancelOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.PostOrdersBatchCancelOpenOrdersCallAsync(request, cancellationToken);

    public Task<Call<CreateWithdrawRequest, RawCreateWithdrawResponse>> PostWithdrawApiCreateCallAsync(
        CreateWithdrawRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.PostWithdrawApiCreateCallAsync(request, cancellationToken);

    public Task<Call<CreateWithdrawVirtualByAddressIdRequest, RawCreateWithdrawResponse>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        CreateWithdrawVirtualByAddressIdRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.PostWithdrawVirtualByAddressIdCreateCallAsync(request, cancellationToken);

    public Task<Call<CancelWithdrawRequest, RawCancelWithdrawResponse>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        CancelWithdrawRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.PostWithdrawVirtualByWithdrawIdCancelCallAsync(request, cancellationToken);

    public Task<Call<PlaceWithdrawVirtualRequest, RawCreateWithdrawResponse>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        PlaceWithdrawVirtualRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.PostWithdrawVirtualByWithdrawIdPlaceCallAsync(request, cancellationToken);

    public Task<Call<CreateRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderPlaceCallAsync(
        CreateRetailOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.PostRetailOrderPlaceCallAsync(request, cancellationToken);

    public Task<Call<CancelRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderCancelByOrderIdCallAsync(
        CancelRetailOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.PostRetailOrderCancelByOrderIdCallAsync(request, cancellationToken);

    public Task<Call<PostRetailOrderHistoryRequest, RawRetailOrdersResponse>> PostRetailOrderHistoryCallAsync(
        PostRetailOrderHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.PostRetailOrderHistoryCallAsync(request, cancellationToken);

    public Task<Call<PostRetailOrderDetailRequest, RawRetailOrderDetailResponse>> PostRetailOrderDetailCallAsync(
        PostRetailOrderDetailRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.PostRetailOrderDetailCallAsync(request, cancellationToken);

    public Task<Call<CreateRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderCreateCallAsync(
        CreateRetailOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.PostRetailOrderCreateCallAsync(request, cancellationToken);

    public Task<Call<GetOpenOrdersRequest, RawOpenOrdersResponse>> GetOpenOrdersCallAsync(
        GetOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetOpenOrdersCallAsync(request, cancellationToken);

    public Task<Call<GetOrdersRequest, RawOrdersResponse>> GetOrdersCallAsync(
        GetOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetOrdersCallAsync(request, cancellationToken);

    public Task<Call<GetOrderRequest, RawOrderDetailResponse>> GetOrdersByOrderIdCallAsync(
        GetOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetOrdersByOrderIdCallAsync(request, cancellationToken);

    public Task<Call<GetMatchResultsRequest, RawMatchResultsResponse>> GetMatchResultsCallAsync(
        GetMatchResultsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetMatchResultsCallAsync(request, cancellationToken);

    public Task<Call<GetRetailOrderDetailByOrderIdRequest, RawRetailOrderDetailResponse>> GetRetailOrderDetailByOrderIdCallAsync(
        GetRetailOrderDetailByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetRetailOrderDetailByOrderIdCallAsync(request, cancellationToken);

    public Task<Call<GetRetailAccountBalanceRequest, RawRetailAccountBalanceResponse>> GetRetailAccountBalanceCallAsync(
        GetRetailAccountBalanceRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetRetailAccountBalanceCallAsync(request, cancellationToken);
}
