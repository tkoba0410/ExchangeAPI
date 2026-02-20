using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Requests;
using ExchangeApi.Exchanges.Common.Raw.Api;
using ExchangeApi.Exchanges.Bittrade.Wire.Internal;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Api;

/// <summary>
/// Bittrade の Raw API アクセス（Public/Private をまとめた単一入口）。
/// </summary>
public sealed class RawApi : IRawApi
{
    private readonly PublicApi _publicApi;
    private readonly RawPrivateClient _privateClient;

    public RawApi(IWireCallExecutor wire)
    {
        if (wire is null) throw new ArgumentNullException(nameof(wire));
        var executor = new RawCallExecutor();
        _publicApi = new PublicApi(wire, executor);
        _privateClient = new RawPrivateClient(wire, executor);
    }

    internal RawApi(
        PublicApi publicApi,
        RawPrivateClient privateClient)
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        _privateClient = privateClient ?? throw new ArgumentNullException(nameof(privateClient));
    }

    public Task<Call<GetDetailMergedRequest, GetDetailMergedResponse>> GetDetailMergedCallAsync(
        GetDetailMergedRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetDetailMergedCallAsync(request, cancellationToken);

    public Task<Call<GetDepthRequest, GetDepthResponse>> GetDepthCallAsync(
        GetDepthRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetDepthCallAsync(request, cancellationToken);

    public Task<Call<GetTradeRequest, GetTradeResponse>> GetTradeCallAsync(
        GetTradeRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetTradeCallAsync(request, cancellationToken);

    public Task<Call<GetSymbolsRequest, GetSymbolsResponse>> GetSymbolsCallAsync(
        GetSymbolsRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetSymbolsCallAsync(request, cancellationToken);

    public Task<Call<GetCurrenciesRequest, GetCurrenciesResponse>> GetCurrenciesCallAsync(
        GetCurrenciesRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetCurrenciesCallAsync(request, cancellationToken);

    public Task<Call<GetTimestampRequest, GetTimestampResponse>> GetTimestampCallAsync(
        GetTimestampRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetTimestampCallAsync(request, cancellationToken);

    public Task<Call<GetHistoryKlineRequest, GetHistoryKlineResponse>> GetHistoryKlineCallAsync(
        GetHistoryKlineRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetHistoryKlineCallAsync(request, cancellationToken);

    public Task<Call<GetTickersRequest, GetTickersResponse>> GetTickersCallAsync(
        GetTickersRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetTickersCallAsync(request, cancellationToken);

    public Task<Call<GetHistoryTradeRequest, GetHistoryTradeResponse>> GetHistoryTradeCallAsync(
        GetHistoryTradeRequest request,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetHistoryTradeCallAsync(request, cancellationToken);

    public Task<Call<GetAccountsRequest, GetAccountsResponse>> GetAccountsCallAsync(
        GetAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetAccountsCallAsync(request, cancellationToken);

    public Task<Call<GetAccountsBalanceByAccountIdRequest, GetAccountsBalanceByAccountIdResponse>> GetAccountsBalanceByAccountIdCallAsync(
        GetAccountsBalanceByAccountIdRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetAccountsBalanceByAccountIdCallAsync(request, cancellationToken);

    public Task<Call<GetOpenOrdersRequest, GetOpenOrdersResponse>> GetOpenOrdersCallAsync(
        GetOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetOpenOrdersCallAsync(request, cancellationToken);

    public Task<Call<GetOrdersRequest, GetOrdersResponse>> GetOrdersCallAsync(
        GetOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetOrdersCallAsync(request, cancellationToken);

    public Task<Call<GetOrdersByOrderIdRequest, GetOrdersByOrderIdResponse>> GetOrdersByOrderIdCallAsync(
        GetOrdersByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetOrdersByOrderIdCallAsync(request, cancellationToken);

    public Task<Call<GetOrdersMatchResultsByOrderIdRequest, GetOrdersMatchResultsByOrderIdResponse>> GetOrdersMatchResultsByOrderIdCallAsync(
        GetOrdersMatchResultsByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetOrdersMatchResultsByOrderIdCallAsync(request, cancellationToken);

    public Task<Call<GetMatchResultsRequest, GetMatchResultsResponse>> GetMatchResultsCallAsync(
        GetMatchResultsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetMatchResultsCallAsync(request, cancellationToken);

    public Task<Call<GetDepositWithdrawRequest, GetDepositWithdrawResponse>> GetDepositWithdrawCallAsync(
        GetDepositWithdrawRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetDepositWithdrawCallAsync(request, cancellationToken);

    public Task<Call<GetWithdrawVirtualAddressesRequest, GetWithdrawVirtualAddressesResponse>> GetWithdrawVirtualAddressesCallAsync(
        GetWithdrawVirtualAddressesRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetWithdrawVirtualAddressesCallAsync(request, cancellationToken);

    public Task<Call<GetRetailOrderListRequest, GetRetailOrderListResponse>> GetRetailOrderListCallAsync(
        GetRetailOrderListRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetRetailOrderListCallAsync(request, cancellationToken);

    public Task<Call<GetRetailOrderDetailByOrderIdRequest, GetRetailOrderDetailByOrderIdResponse>> GetRetailOrderDetailByOrderIdCallAsync(
        GetRetailOrderDetailByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetRetailOrderDetailByOrderIdCallAsync(request, cancellationToken);

    public Task<Call<GetRetailAccountBalanceRequest, GetRetailAccountBalanceResponse>> GetRetailAccountBalanceCallAsync(
        GetRetailAccountBalanceRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.GetRetailAccountBalanceCallAsync(request, cancellationToken);

    public Task<Call<PostOrdersPlaceRequest, PostOrdersPlaceResponse>> PostOrdersPlaceCallAsync(
        PostOrdersPlaceRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostOrdersPlaceCallAsync(request, cancellationToken);

    public Task<Call<PostOrdersSubmitCancelByOrderIdRequest, PostOrdersSubmitCancelByOrderIdResponse>> PostOrdersSubmitCancelByOrderIdCallAsync(
        PostOrdersSubmitCancelByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostOrdersSubmitCancelByOrderIdCallAsync(request, cancellationToken);

    public Task<Call<PostOrdersBatchCancelRequest, PostOrdersBatchCancelResponse>> PostOrdersBatchCancelCallAsync(
        PostOrdersBatchCancelRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostOrdersBatchCancelCallAsync(request, cancellationToken);

    public Task<Call<PostOrdersBatchCancelOpenOrdersRequest, PostOrdersBatchCancelOpenOrdersResponse>> PostOrdersBatchCancelOpenOrdersCallAsync(
        PostOrdersBatchCancelOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostOrdersBatchCancelOpenOrdersCallAsync(request, cancellationToken);

    public Task<Call<PostWithdrawApiCreateRequest, PostWithdrawApiCreateResponse>> PostWithdrawApiCreateCallAsync(
        PostWithdrawApiCreateRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostWithdrawApiCreateCallAsync(request, cancellationToken);

    public Task<Call<PostWithdrawVirtualByAddressIdCreateRequest, PostWithdrawVirtualByAddressIdCreateResponse>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        PostWithdrawVirtualByAddressIdCreateRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostWithdrawVirtualByAddressIdCreateCallAsync(request, cancellationToken);

    public Task<Call<PostWithdrawVirtualByWithdrawIdCancelRequest, PostWithdrawVirtualByWithdrawIdCancelResponse>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        PostWithdrawVirtualByWithdrawIdCancelRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostWithdrawVirtualByWithdrawIdCancelCallAsync(request, cancellationToken);

    public Task<Call<PostWithdrawVirtualByWithdrawIdPlaceRequest, PostWithdrawVirtualByWithdrawIdPlaceResponse>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        PostWithdrawVirtualByWithdrawIdPlaceRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostWithdrawVirtualByWithdrawIdPlaceCallAsync(request, cancellationToken);

    public Task<Call<PostRetailOrderPlaceRequest, PostRetailOrderPlaceResponse>> PostRetailOrderPlaceCallAsync(
        PostRetailOrderPlaceRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostRetailOrderPlaceCallAsync(request, cancellationToken);

    public Task<Call<PostRetailOrderCancelByOrderIdRequest, PostRetailOrderCancelByOrderIdResponse>> PostRetailOrderCancelByOrderIdCallAsync(
        PostRetailOrderCancelByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostRetailOrderCancelByOrderIdCallAsync(request, cancellationToken);

    public Task<Call<PostRetailOrderHistoryRequest, PostRetailOrderHistoryResponse>> PostRetailOrderHistoryCallAsync(
        PostRetailOrderHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostRetailOrderHistoryCallAsync(request, cancellationToken);

    public Task<Call<PostRetailOrderDetailRequest, PostRetailOrderDetailResponse>> PostRetailOrderDetailCallAsync(
        PostRetailOrderDetailRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostRetailOrderDetailCallAsync(request, cancellationToken);

    public Task<Call<PostRetailOrderCreateRequest, PostRetailOrderCreateResponse>> PostRetailOrderCreateCallAsync(
        PostRetailOrderCreateRequest request,
        CancellationToken cancellationToken = default) =>
        _privateClient.PostRetailOrderCreateCallAsync(request, cancellationToken);
}
