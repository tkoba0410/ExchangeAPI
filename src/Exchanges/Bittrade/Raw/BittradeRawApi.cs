using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

/// <summary>
/// Bittrade の Raw API アクセス（Public/Private/Trading をまとめた単一入口）。
/// </summary>
public sealed class BittradeRawApi : IBittradeRawApi
{
    private readonly IBittradePublicApi _publicApi;
    private readonly IBittradePrivateApi _privateApi;
    private readonly IBittradePrivateTradingApi _privateTradingApi;

    public BittradeRawApi(IWireTransport wire)
        : this(
            publicApi: new BittradePublicApi(wire ?? throw new ArgumentNullException(nameof(wire))),
            privateApi: new BittradePrivateApi(wire),
            privateTradingApi: new BittradePrivateTradingApi(wire))
    {
    }

    internal BittradeRawApi(
        IBittradePublicApi publicApi,
        IBittradePrivateApi privateApi,
        IBittradePrivateTradingApi privateTradingApi)
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
        _privateTradingApi = privateTradingApi ?? throw new ArgumentNullException(nameof(privateTradingApi));
    }

    public Task<Call<GetMergedTickerRequest, RawMergedResponse>> GetDetailMergedCallAsync(
        GetMergedTickerRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetDetailMergedCallAsync(request, cancellationToken);

    public Task<Call<GetDepthRequest, RawDepthResponse>> GetDepthCallAsync(
        GetDepthRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetDepthCallAsync(request, cancellationToken);

    public Task<Call<GetTradesRequest, RawTradeResponse>> GetTradeCallAsync(
        GetTradesRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetTradeCallAsync(request, cancellationToken);

    public Task<Call<GetSymbolsRequest, RawSymbolsResponse>> GetSymbolsCallAsync(
        GetSymbolsRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetSymbolsCallAsync(request, cancellationToken);

    public Task<Call<GetCurrenciesRequest, RawCurrenciesResponse>> GetCurrencysCallAsync(
        GetCurrenciesRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetCurrencysCallAsync(request, cancellationToken);

    public Task<Call<GetTimestampRequest, RawTimestampResponse>> GetTimestampCallAsync(
        GetTimestampRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetTimestampCallAsync(request, cancellationToken);

    public Task<Call<GetKlinesRequest, RawKlinesResponse>> GetHistoryKlineCallAsync(
        GetKlinesRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetHistoryKlineCallAsync(request, cancellationToken);

    public Task<Call<GetTickersRequest, RawTickersResponse>> GetTickersCallAsync(
        GetTickersRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetTickersCallAsync(request, cancellationToken);

    public Task<Call<GetTradeHistoryRequest, RawTradeHistoryResponse>> GetHistoryTradeCallAsync(
        GetTradeHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetHistoryTradeCallAsync(request, cancellationToken);

    public Task<Call<GetRetailMaintainTimeRequest, RawRetailMaintainTimeResponse>> GetMaintainTimeCallAsync(
        GetRetailMaintainTimeRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetMaintainTimeCallAsync(request, cancellationToken);

    public Task<Call<GetAccountsRequest, RawAccountsResponse>> GetAccountsCallAsync(
        GetAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetAccountsCallAsync(request, cancellationToken);

    public Task<Call<GetAccountBalanceRequest, RawBalancesResponse>> GetAccountsBalanceByAccountIdCallAsync(
        GetAccountBalanceRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetAccountsBalanceByAccountIdCallAsync(request, cancellationToken);

    public Task<Call<GetOpenOrdersRequest, RawOpenOrdersResponse>> GetOpenOrdersCallAsync(
        GetOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetOpenOrdersCallAsync(request, cancellationToken);

    public Task<Call<GetOrderRequest, RawOrderDetailResponse>> GetOrdersByOrderIdCallAsync(
        GetOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetOrdersByOrderIdCallAsync(request, cancellationToken);

    public Task<Call<GetOrderMatchResultsRequest, RawOrderMatchResultsResponse>> GetOrdersMatchResultsByOrderIdCallAsync(
        GetOrderMatchResultsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetOrdersMatchResultsByOrderIdCallAsync(request, cancellationToken);

    public Task<Call<GetMatchResultsRequest, RawMatchResultsResponse>> GetMatchResultsCallAsync(
        GetMatchResultsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetMatchResultsCallAsync(request, cancellationToken);

    public Task<Call<GetDepositWithdrawsRequest, RawDepositWithdrawsResponse>> GetDepositWithdrawCallAsync(
        GetDepositWithdrawsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetDepositWithdrawCallAsync(request, cancellationToken);

    public Task<Call<GetRetailOrdersRequest, RawRetailOrdersResponse>> GetOrderListCallAsync(
        GetRetailOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetOrderListCallAsync(request, cancellationToken);

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

    public Task<Call<CancelWithdrawRequest, RawCancelWithdrawResponse>> PostWithdrawVirtualCancelByWithdrawIdCallAsync(
        CancelWithdrawRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.PostWithdrawVirtualCancelByWithdrawIdCallAsync(request, cancellationToken);

    public Task<Call<CreateRetailOrderRequest, RawRetailOrderResponse>> PostOrderPlaceCallAsync(
        CreateRetailOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateTradingApi.PostOrderPlaceCallAsync(request, cancellationToken);
}
