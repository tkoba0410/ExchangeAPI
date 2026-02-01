using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Public.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Transport.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Api;

/// <summary>
/// Bittrade の Raw API アクセス（Public/Private/Trading をまとめた単一入口）。
/// </summary>
public sealed class BittradeRawApi : IBittradeRawApi
{
    private readonly BittradePublicApi _publicApi;
    private readonly BittradeRawPrivateClient _privateClient;

    public BittradeRawApi(IWireTransport wire)
    {
        if (wire is null) throw new ArgumentNullException(nameof(wire));
        var executor = new BittradeRawCallExecutor(wire);
        _publicApi = new BittradePublicApi(executor);
        _privateClient = new BittradeRawPrivateClient(executor);
    }

    internal BittradeRawApi(
        BittradePublicApi publicApi,
        BittradeRawPrivateClient privateClient)
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        _privateClient = privateClient ?? throw new ArgumentNullException(nameof(privateClient));
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

    public Task<Call<GetAccountsRequest, RawAccountsResponse>> GetAccountsCallAsync(
        GetAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetAccountsCallAsync(request, cancellationToken);

    public Task<Call<GetAccountBalanceRequest, RawBalancesResponse>> GetAccountsBalanceByAccountIdCallAsync(
        GetAccountBalanceRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetAccountsBalanceByAccountIdCallAsync(request, cancellationToken);

    public Task<Call<GetOpenOrdersRequest, RawOpenOrdersResponse>> GetOpenOrdersCallAsync(
        GetOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetOpenOrdersCallAsync(request, cancellationToken);

    public Task<Call<GetOrdersRequest, RawOrdersResponse>> GetOrdersCallAsync(
        GetOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetOrdersCallAsync(request, cancellationToken);

    public Task<Call<GetOrderRequest, RawOrderDetailResponse>> GetOrdersByOrderIdCallAsync(
        GetOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetOrdersByOrderIdCallAsync(request, cancellationToken);

    public Task<Call<GetOrderMatchResultsRequest, RawOrderMatchResultsResponse>> GetOrdersMatchResultsByOrderIdCallAsync(
        GetOrderMatchResultsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetOrdersMatchResultsByOrderIdCallAsync(request, cancellationToken);

    public Task<Call<GetMatchResultsRequest, RawMatchResultsResponse>> GetMatchResultsCallAsync(
        GetMatchResultsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetMatchResultsCallAsync(request, cancellationToken);

    public Task<Call<GetDepositWithdrawsRequest, RawDepositWithdrawsResponse>> GetDepositWithdrawCallAsync(
        GetDepositWithdrawsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetDepositWithdrawCallAsync(request, cancellationToken);

    public Task<Call<GetWithdrawVirtualAddressesRequest, RawWithdrawVirtualAddressesResponse>> GetWithdrawVirtualAddressesCallAsync(
        GetWithdrawVirtualAddressesRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetWithdrawVirtualAddressesCallAsync(request, cancellationToken);

    public Task<Call<GetRetailOrdersRequest, RawRetailOrdersResponse>> GetRetailOrderListCallAsync(
        GetRetailOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetRetailOrderListCallAsync(request, cancellationToken);

    public Task<Call<GetRetailOrderDetailByOrderIdRequest, RawRetailOrderDetailResponse>> GetRetailOrderDetailByOrderIdCallAsync(
        GetRetailOrderDetailByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetRetailOrderDetailByOrderIdCallAsync(request, cancellationToken);

    public Task<Call<GetRetailAccountBalanceRequest, RawRetailAccountBalanceResponse>> GetRetailAccountBalanceCallAsync(
        GetRetailAccountBalanceRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetRetailAccountBalanceCallAsync(request, cancellationToken);

    public Task<Call<CreateOrderRequest, RawPlaceOrderResponse>> PostOrdersPlaceCallAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostOrdersPlaceCallAsync(request, cancellationToken);

    public Task<Call<CancelOrderRequest, RawCancelOrderResponse>> PostOrdersSubmitCancelByOrderIdCallAsync(
        CancelOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostOrdersSubmitCancelByOrderIdCallAsync(request, cancellationToken);

    public Task<Call<CancelOrdersRequest, RawCancelOrdersResponse>> PostOrdersBatchCancelCallAsync(
        CancelOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostOrdersBatchCancelCallAsync(request, cancellationToken);

    public Task<Call<CancelOpenOrdersRequest, RawCancelOpenOrdersResponse>> PostOrdersBatchCancelOpenOrdersCallAsync(
        CancelOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostOrdersBatchCancelOpenOrdersCallAsync(request, cancellationToken);

    public Task<Call<CreateWithdrawRequest, RawCreateWithdrawResponse>> PostWithdrawApiCreateCallAsync(
        CreateWithdrawRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostWithdrawApiCreateCallAsync(request, cancellationToken);

    public Task<Call<CreateWithdrawVirtualByAddressIdRequest, RawCreateWithdrawResponse>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        CreateWithdrawVirtualByAddressIdRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostWithdrawVirtualByAddressIdCreateCallAsync(request, cancellationToken);

    public Task<Call<CancelWithdrawRequest, RawCancelWithdrawResponse>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        CancelWithdrawRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostWithdrawVirtualByWithdrawIdCancelCallAsync(request, cancellationToken);

    public Task<Call<PlaceWithdrawVirtualRequest, RawCreateWithdrawResponse>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        PlaceWithdrawVirtualRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostWithdrawVirtualByWithdrawIdPlaceCallAsync(request, cancellationToken);

    public Task<Call<CreateRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderPlaceCallAsync(
        CreateRetailOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostRetailOrderPlaceCallAsync(request, cancellationToken);

    public Task<Call<CancelRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderCancelByOrderIdCallAsync(
        CancelRetailOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostRetailOrderCancelByOrderIdCallAsync(request, cancellationToken);

    public Task<Call<PostRetailOrderHistoryRequest, RawRetailOrdersResponse>> PostRetailOrderHistoryCallAsync(
        PostRetailOrderHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostRetailOrderHistoryCallAsync(request, cancellationToken);

    public Task<Call<PostRetailOrderDetailRequest, RawRetailOrderDetailResponse>> PostRetailOrderDetailCallAsync(
        PostRetailOrderDetailRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostRetailOrderDetailCallAsync(request, cancellationToken);

    public Task<Call<CreateRetailOrderRequest, RawRetailOrderResponse>> PostRetailOrderCreateCallAsync(
        CreateRetailOrderRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostRetailOrderCreateCallAsync(request, cancellationToken);
}
