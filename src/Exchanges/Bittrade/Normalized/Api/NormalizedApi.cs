using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Markets;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ExchangeApi.Exchanges.Bittrade.Raw.Api;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Api;

public sealed class NormalizedApi : IBittradeNormalizedApi
{
    private readonly NormalizedPublicApi _publicApi;
    private readonly NormalizedPrivateApi _privateApi;
    public AccountId AccountId { get; }

    private NormalizedApi(
        NormalizedPublicApi publicApi,
        NormalizedPrivateApi privateApi,
        AccountId accountId)
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
        AccountId = accountId;
    }

    internal static NormalizedApi FromRaw(
        IBittradeRawApi raw,
        IBittradeMarketResolver markets,
        AccountId accountId)
    {
        if (raw is null) throw new ArgumentNullException(nameof(raw));
        if (markets is null) throw new ArgumentNullException(nameof(markets));

        if (accountId.IsEmpty)
        {
            throw new ArgumentException("accountId is required.", nameof(accountId));
        }

        var normalizedAccountId = accountId;
        var publicApi = new NormalizedPublicApi(raw);
        var privateApi = new NormalizedPrivateApi(raw, markets, normalizedAccountId);

        return new NormalizedApi(
            publicApi: publicApi,
            privateApi: privateApi,
            accountId: normalizedAccountId);
    }

    public Task<Call<GetDetailMergedRequest, GetDetailMergedResponse>> GetDetailMergedCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetDetailMergedCallAsync(productCode, cancellationToken);

    public Task<Call<GetDepthRequest, GetDepthResponse>> GetDepthCallAsync(
        ProductCode productCode,
        DepthType? depthType = null,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetDepthCallAsync(productCode, depthType, cancellationToken);

    public Task<Call<GetTradeRequest, GetTradeResponse>> GetTradeCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetTradeCallAsync(productCode, cancellationToken);

    public Task<Call<GetSymbolsRequest, GetSymbolsResponse>> GetSymbolsCallAsync(
        CancellationToken cancellationToken = default) =>
        _publicApi.GetSymbolsCallAsync(cancellationToken);

    public Task<Call<GetCurrencysRequest, GetCurrencysResponse>> GetCurrencysCallAsync(
        CancellationToken cancellationToken = default) =>
        _publicApi.GetCurrencysCallAsync(cancellationToken);

    public Task<Call<GetTimestampRequest, GetTimestampResponse>> GetTimestampCallAsync(
        CancellationToken cancellationToken = default) =>
        _publicApi.GetTimestampCallAsync(cancellationToken);

    public Task<Call<GetHistoryKlineRequest, GetHistoryKlineResponse>> GetHistoryKlineCallAsync(
        ProductCode productCode,
        Period period,
        RequestSize? size = null,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetHistoryKlineCallAsync(productCode, period, size, cancellationToken);

    public Task<Call<GetTickersRequest, GetTickersResponse>> GetTickersCallAsync(
        CancellationToken cancellationToken = default) =>
        _publicApi.GetTickersCallAsync(cancellationToken);

    public Task<Call<GetHistoryTradeRequest, GetHistoryTradeResponse>> GetHistoryTradeCallAsync(
        ProductCode productCode,
        CancellationToken cancellationToken = default) =>
        _publicApi.GetHistoryTradeCallAsync(productCode, cancellationToken);

    public Task<Call<GetAccountsRequest, GetAccountsResponse>> GetAccountsCallAsync(
        CancellationToken cancellationToken = default) =>
        _privateApi.GetAccountsCallAsync(cancellationToken);

    public Task<Call<GetAccountsBalanceByAccountIdRequest, GetAccountsBalanceByAccountIdResponse>> GetAccountsBalanceByAccountIdCallAsync(
        CancellationToken cancellationToken = default) =>
        _privateApi.GetAccountsBalanceByAccountIdCallAsync(cancellationToken);

    public Task<Call<GetDepositWithdrawRequest, GetDepositWithdrawResponse>> GetDepositWithdrawCallAsync(
        GetDepositWithdrawRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetDepositWithdrawCallAsync(request, cancellationToken);

    public Task<Call<GetWithdrawVirtualAddressesRequest, GetWithdrawVirtualAddressesResponse>> GetWithdrawVirtualAddressesCallAsync(
        CancellationToken cancellationToken = default) =>
        _privateApi.GetWithdrawVirtualAddressesCallAsync(cancellationToken);

    public Task<Call<GetRetailAccountBalanceRequest, GetRetailAccountBalanceResponse>> GetRetailAccountBalanceCallAsync(
        CancellationToken cancellationToken = default) =>
        _privateApi.GetRetailAccountBalanceCallAsync(cancellationToken);

    public Task<Call<PostOrdersPlaceRequest, PostOrdersPlaceResponse>> PostOrdersPlaceCallAsync(
        PostOrdersPlaceRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.PostOrdersPlaceCallAsync(request, cancellationToken);

    public Task<Call<GetOrdersRequest, GetOrdersResponse>> GetOrdersCallAsync(
        GetOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetOrdersCallAsync(request, cancellationToken);

    public Task<Call<PostOrdersSubmitCancelByOrderIdRequest, PostOrdersSubmitCancelByOrderIdResponse>> PostOrdersSubmitCancelByOrderIdCallAsync(
        PostOrdersSubmitCancelByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.PostOrdersSubmitCancelByOrderIdCallAsync(request, cancellationToken);

    public Task<Call<PostOrdersBatchCancelRequest, PostOrdersBatchCancelResponse>> PostOrdersBatchCancelCallAsync(
        PostOrdersBatchCancelRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.PostOrdersBatchCancelCallAsync(request, cancellationToken);

    public Task<Call<PostOrdersBatchCancelOpenOrdersRequest, PostOrdersBatchCancelOpenOrdersResponse>> PostOrdersBatchCancelOpenOrdersCallAsync(
        PostOrdersBatchCancelOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.PostOrdersBatchCancelOpenOrdersCallAsync(request, cancellationToken);

    public Task<Call<GetOpenOrdersRequest, GetOpenOrdersResponse>> GetOpenOrdersCallAsync(
        GetOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetOpenOrdersCallAsync(request, cancellationToken);

    public Task<Call<GetOrdersByOrderIdRequest, GetOrdersByOrderIdResponse>> GetOrdersByOrderIdCallAsync(
        GetOrdersByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetOrdersByOrderIdCallAsync(request, cancellationToken);

    public Task<Call<GetOrdersMatchResultsByOrderIdRequest, GetOrdersMatchResultsByOrderIdResponse>> GetOrdersMatchResultsByOrderIdCallAsync(
        GetOrdersMatchResultsByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetOrdersMatchResultsByOrderIdCallAsync(request, cancellationToken);

    public Task<Call<GetMatchResultsRequest, GetMatchResultsResponse>> GetMatchResultsCallAsync(
        GetMatchResultsRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetMatchResultsCallAsync(request, cancellationToken);

    public Task<Call<PostWithdrawApiCreateRequest, PostWithdrawApiCreateResponse>> PostWithdrawApiCreateCallAsync(
        PostWithdrawApiCreateRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.PostWithdrawApiCreateCallAsync(request, cancellationToken);

    public Task<Call<PostWithdrawVirtualByAddressIdCreateRequest, PostWithdrawVirtualByAddressIdCreateResponse>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        PostWithdrawVirtualByAddressIdCreateRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.PostWithdrawVirtualByAddressIdCreateCallAsync(request, cancellationToken);

    public Task<Call<PostWithdrawVirtualByWithdrawIdPlaceRequest, PostWithdrawVirtualByWithdrawIdPlaceResponse>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        PostWithdrawVirtualByWithdrawIdPlaceRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.PostWithdrawVirtualByWithdrawIdPlaceCallAsync(request, cancellationToken);

    public Task<Call<PostWithdrawVirtualByWithdrawIdCancelRequest, PostWithdrawVirtualByWithdrawIdCancelResponse>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        PostWithdrawVirtualByWithdrawIdCancelRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.PostWithdrawVirtualByWithdrawIdCancelCallAsync(request, cancellationToken);

    public Task<Call<PostRetailOrderPlaceRequest, PostRetailOrderPlaceResponse>> PostRetailOrderPlaceCallAsync(
        PostRetailOrderPlaceRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.PostRetailOrderPlaceCallAsync(request, cancellationToken);

    public Task<Call<GetRetailOrderListRequest, GetRetailOrderListResponse>> GetRetailOrderListCallAsync(
        GetRetailOrderListRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetRetailOrderListCallAsync(request, cancellationToken);

    public Task<Call<GetRetailOrderDetailByOrderIdRequest, GetRetailOrderDetailByOrderIdResponse>> GetRetailOrderDetailByOrderIdCallAsync(
        GetRetailOrderDetailByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.GetRetailOrderDetailByOrderIdCallAsync(request, cancellationToken);

    public Task<Call<PostRetailOrderHistoryRequest, PostRetailOrderHistoryResponse>> PostRetailOrderHistoryCallAsync(
        PostRetailOrderHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.PostRetailOrderHistoryCallAsync(request, cancellationToken);

    public Task<Call<PostRetailOrderDetailRequest, PostRetailOrderDetailResponse>> PostRetailOrderDetailCallAsync(
        PostRetailOrderDetailRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.PostRetailOrderDetailCallAsync(request, cancellationToken);

    public Task<Call<PostRetailOrderCreateRequest, PostRetailOrderCreateResponse>> PostRetailOrderCreateCallAsync(
        PostRetailOrderCreateRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.PostRetailOrderCreateCallAsync(request, cancellationToken);

    public Task<Call<PostRetailOrderCancelByOrderIdRequest, PostRetailOrderCancelByOrderIdResponse>> PostRetailOrderCancelByOrderIdCallAsync(
        PostRetailOrderCancelByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        _privateApi.PostRetailOrderCancelByOrderIdCallAsync(request, cancellationToken);
}
