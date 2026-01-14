using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Requests;
using ExchangeApi.Exchanges.Bittrade.Raw.Private;
using ExchangeApi.Contracts.Common.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Call;

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

    public Task<Call<CreateOrderRequest, RawPlaceOrderResponse>> CreateOrderAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.CreateOrderAsync(request, cancellationToken);

    public Task<Call<CancelOrderRequest, RawCancelOrderResponse>> CancelOrderAsync(
        CancelOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.CancelOrderAsync(request, cancellationToken);

    public Task<Call<CancelOrdersRequest, RawCancelOrdersResponse>> CancelOrdersAsync(
        CancelOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.CancelOrdersAsync(request, cancellationToken);

    public Task<Call<CancelOpenOrdersRequest, RawCancelOpenOrdersResponse>> CancelOpenOrdersAsync(
        CancelOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.CancelOpenOrdersAsync(request, cancellationToken);

    public Task<Call<CreateWithdrawRequest, RawCreateWithdrawResponse>> CreateWithdrawAsync(
        CreateWithdrawRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.CreateWithdrawAsync(request, cancellationToken);

    public Task<Call<CancelWithdrawRequest, RawCancelWithdrawResponse>> CancelWithdrawAsync(
        CancelWithdrawRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.CancelWithdrawAsync(request, cancellationToken);

    public Task<Call<CreateRetailOrderRequest, RawRetailOrderResponse>> CreateRetailOrderAsync(
        CreateRetailOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.CreateRetailOrderAsync(request, cancellationToken);

    public Task<Call<GetOpenOrdersRequest, RawOpenOrdersResponse>> GetOpenOrdersAsync(
        GetOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetOpenOrdersAsync(request, cancellationToken);

    public Task<Call<GetOrderRequest, RawOrderDetailResponse>> GetOrderAsync(
        GetOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetOrderAsync(request, cancellationToken);

    public Task<Call<GetMatchResultsRequest, RawMatchResultsResponse>> GetMatchResultsAsync(
        GetMatchResultsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetMatchResultsAsync(request, cancellationToken);
}
