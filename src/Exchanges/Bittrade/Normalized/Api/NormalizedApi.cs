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
    public FreeText AccountId { get; }

    private NormalizedApi(
        NormalizedPublicApi publicApi,
        NormalizedPrivateApi privateApi,
        FreeText accountId)
    {
        _publicApi = publicApi ?? throw new ArgumentNullException(nameof(publicApi));
        _privateApi = privateApi ?? throw new ArgumentNullException(nameof(privateApi));
        AccountId = accountId;
    }

    internal static NormalizedApi FromRaw(
        IBittradeRawApi raw,
        IBittradeMarketResolver markets,
        FreeText accountId)
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
        CancellationToken ct = default) =>
        _publicApi.GetDetailMergedCallAsync(productCode, ct);

    public Task<Call<GetDepthRequest, GetDepthResponse>> GetDepthCallAsync(
        ProductCode productCode,
        DepthType? depthType = null,
        CancellationToken ct = default) =>
        _publicApi.GetDepthCallAsync(productCode, depthType, ct);

    public Task<Call<GetTradeRequest, GetTradeResponse>> GetTradeCallAsync(
        ProductCode productCode,
        CancellationToken ct = default) =>
        _publicApi.GetTradeCallAsync(productCode, ct);

    public Task<Call<GetSymbolsRequest, GetSymbolsResponse>> GetSymbolsCallAsync(
        CancellationToken ct = default) =>
        _publicApi.GetSymbolsCallAsync(ct);

    public Task<Call<GetCurrencysRequest, GetCurrencysResponse>> GetCurrencysCallAsync(
        CancellationToken ct = default) =>
        _publicApi.GetCurrencysCallAsync(ct);

    public Task<Call<GetTimestampRequest, GetTimestampResponse>> GetTimestampCallAsync(
        CancellationToken ct = default) =>
        _publicApi.GetTimestampCallAsync(ct);

    public Task<Call<GetHistoryKlineRequest, GetHistoryKlineResponse>> GetHistoryKlineCallAsync(
        ProductCode productCode,
        Period period,
        int? size = null,
        CancellationToken ct = default) =>
        _publicApi.GetHistoryKlineCallAsync(productCode, period, size, ct);

    public Task<Call<GetTickersRequest, GetTickersResponse>> GetTickersCallAsync(
        CancellationToken ct = default) =>
        _publicApi.GetTickersCallAsync(ct);

    public Task<Call<GetHistoryTradeRequest, GetHistoryTradeResponse>> GetHistoryTradeCallAsync(
        ProductCode productCode,
        CancellationToken ct = default) =>
        _publicApi.GetHistoryTradeCallAsync(productCode, ct);

    public Task<Call<GetAccountsRequest, GetAccountsResponse>> GetAccountsCallAsync(
        CancellationToken ct = default) =>
        _privateApi.GetAccountsCallAsync(ct);

    public Task<Call<GetAccountsBalanceByAccountIdRequest, GetAccountsBalanceByAccountIdResponse>> GetAccountsBalanceByAccountIdCallAsync(
        CancellationToken ct = default) =>
        _privateApi.GetAccountsBalanceByAccountIdCallAsync(ct);

    public Task<Call<GetDepositWithdrawRequest, GetDepositWithdrawResponse>> GetDepositWithdrawCallAsync(
        GetDepositWithdrawRequest request,
        CancellationToken ct = default) =>
        _privateApi.GetDepositWithdrawCallAsync(request, ct);

    public Task<Call<GetWithdrawVirtualAddressesRequest, GetWithdrawVirtualAddressesResponse>> GetWithdrawVirtualAddressesCallAsync(
        CancellationToken ct = default) =>
        _privateApi.GetWithdrawVirtualAddressesCallAsync(ct);

    public Task<Call<GetRetailAccountBalanceRequest, GetRetailAccountBalanceResponse>> GetRetailAccountBalanceCallAsync(
        CancellationToken ct = default) =>
        _privateApi.GetRetailAccountBalanceCallAsync(ct);

    public Task<Call<PostOrdersPlaceRequest, PostOrdersPlaceResponse>> PostOrdersPlaceCallAsync(
        PostOrdersPlaceRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostOrdersPlaceCallAsync(request, ct);

    public Task<Call<GetOrdersRequest, GetOrdersResponse>> GetOrdersCallAsync(
        GetOrdersRequest request,
        CancellationToken ct = default) =>
        _privateApi.GetOrdersCallAsync(ct);

    public Task<Call<PostOrdersSubmitCancelByOrderIdRequest, PostOrdersSubmitCancelByOrderIdResponse>> PostOrdersSubmitCancelByOrderIdCallAsync(
        PostOrdersSubmitCancelByOrderIdRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostOrdersSubmitCancelByOrderIdCallAsync(request.Symbol, request.OrderKey, ct);

    public Task<Call<PostOrdersBatchCancelRequest, PostOrdersBatchCancelResponse>> PostOrdersBatchCancelCallAsync(
        PostOrdersBatchCancelRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostOrdersBatchCancelCallAsync(request, ct);

    public Task<Call<PostOrdersBatchCancelOpenOrdersRequest, PostOrdersBatchCancelOpenOrdersResponse>> PostOrdersBatchCancelOpenOrdersCallAsync(
        PostOrdersBatchCancelOpenOrdersRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostOrdersBatchCancelOpenOrdersCallAsync(request, ct);

    public Task<Call<GetOpenOrdersRequest, GetOpenOrdersResponse>> GetOpenOrdersCallAsync(
        GetOpenOrdersRequest request,
        CancellationToken ct = default) =>
        _privateApi.GetOpenOrdersCallAsync(request.Symbol, ct);

    public Task<Call<GetOrdersByOrderIdRequest, GetOrdersByOrderIdResponse>> GetOrdersByOrderIdCallAsync(
        GetOrdersByOrderIdRequest request,
        CancellationToken ct = default) =>
        _privateApi.GetOrdersByOrderIdCallAsync(request.Symbol, request.OrderKey, ct);

    public Task<Call<GetOrdersMatchResultsByOrderIdRequest, GetOrdersMatchResultsByOrderIdResponse>> GetOrdersMatchResultsByOrderIdCallAsync(
        GetOrdersMatchResultsByOrderIdRequest request,
        CancellationToken ct = default) =>
        _privateApi.GetOrdersMatchResultsByOrderIdCallAsync(request, ct);

    public Task<Call<GetMatchResultsRequest, GetMatchResultsResponse>> GetMatchResultsCallAsync(
        GetMatchResultsRequest request,
        CancellationToken ct = default) =>
        _privateApi.GetMatchResultsCallAsync(request.Symbol, request.Limit, ct);

    public Task<Call<PostWithdrawApiCreateRequest, PostWithdrawApiCreateResponse>> PostWithdrawApiCreateCallAsync(
        PostWithdrawApiCreateRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostWithdrawApiCreateCallAsync(request, ct);

    public Task<Call<PostWithdrawVirtualByAddressIdCreateRequest, PostWithdrawVirtualByAddressIdCreateResponse>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        PostWithdrawVirtualByAddressIdCreateRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostWithdrawVirtualByAddressIdCreateCallAsync(request, ct);

    public Task<Call<PostWithdrawVirtualByWithdrawIdPlaceRequest, PostWithdrawVirtualByWithdrawIdPlaceResponse>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        PostWithdrawVirtualByWithdrawIdPlaceRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostWithdrawVirtualByWithdrawIdPlaceCallAsync(request, ct);

    public Task<Call<PostWithdrawVirtualByWithdrawIdCancelRequest, PostWithdrawVirtualByWithdrawIdCancelResponse>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        PostWithdrawVirtualByWithdrawIdCancelRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostWithdrawVirtualByWithdrawIdCancelCallAsync(request, ct);

    public Task<Call<PostRetailOrderPlaceRequest, PostRetailOrderPlaceResponse>> PostRetailOrderPlaceCallAsync(
        PostRetailOrderPlaceRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostRetailOrderPlaceCallAsync(request, ct);

    public Task<Call<GetRetailOrderListRequest, GetRetailOrderListResponse>> GetRetailOrderListCallAsync(
        GetRetailOrderListRequest request,
        CancellationToken ct = default) =>
        _privateApi.GetRetailOrderListCallAsync(request, ct);

    public Task<Call<GetRetailOrderDetailByOrderIdRequest, GetRetailOrderDetailByOrderIdResponse>> GetRetailOrderDetailByOrderIdCallAsync(
        GetRetailOrderDetailByOrderIdRequest request,
        CancellationToken ct = default) =>
        _privateApi.GetRetailOrderDetailByOrderIdCallAsync(request, ct);

    public Task<Call<PostRetailOrderHistoryRequest, PostRetailOrderHistoryResponse>> PostRetailOrderHistoryCallAsync(
        PostRetailOrderHistoryRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostRetailOrderHistoryCallAsync(request, ct);

    public Task<Call<PostRetailOrderDetailRequest, PostRetailOrderDetailResponse>> PostRetailOrderDetailCallAsync(
        PostRetailOrderDetailRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostRetailOrderDetailCallAsync(request, ct);

    public Task<Call<PostRetailOrderCreateRequest, PostRetailOrderCreateResponse>> PostRetailOrderCreateCallAsync(
        PostRetailOrderCreateRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostRetailOrderCreateCallAsync(request, ct);

    public Task<Call<PostRetailOrderCancelByOrderIdRequest, PostRetailOrderCancelByOrderIdResponse>> PostRetailOrderCancelByOrderIdCallAsync(
        PostRetailOrderCancelByOrderIdRequest request,
        CancellationToken ct = default) =>
        _privateApi.PostRetailOrderCancelByOrderIdCallAsync(request, ct);
}
