using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Api;

public interface IBittradeNormalizedApi
{
    Task<Call<GetDetailMergedRequest, GetDetailMergedResponse>> GetDetailMergedCallAsync(
        ProductCode productCode,
        CancellationToken ct = default);

    Task<Call<GetDepthRequest, GetDepthResponse>> GetDepthCallAsync(
        ProductCode productCode,
        DepthType? depthType = null,
        CancellationToken ct = default);

    Task<Call<GetTradeRequest, GetTradeResponse>> GetTradeCallAsync(
        ProductCode productCode,
        CancellationToken ct = default);

    Task<Call<GetSymbolsRequest, GetSymbolsResponse>> GetSymbolsCallAsync(
        CancellationToken ct = default);

    Task<Call<GetCurrencysRequest, GetCurrencysResponse>> GetCurrencysCallAsync(
        CancellationToken ct = default);

    Task<Call<GetTimestampRequest, GetTimestampResponse>> GetTimestampCallAsync(
        CancellationToken ct = default);

    Task<Call<GetHistoryKlineRequest, GetHistoryKlineResponse>> GetHistoryKlineCallAsync(
        ProductCode productCode,
        Period period,
        int? size = null,
        CancellationToken ct = default);

    Task<Call<GetTickersRequest, GetTickersResponse>> GetTickersCallAsync(
        CancellationToken ct = default);

    Task<Call<GetHistoryTradeRequest, GetHistoryTradeResponse>> GetHistoryTradeCallAsync(
        ProductCode productCode,
        CancellationToken ct = default);

    Task<Call<GetAccountsRequest, GetAccountsResponse>> GetAccountsCallAsync(
        CancellationToken ct = default);

    Task<Call<GetAccountsBalanceByAccountIdRequest, GetAccountsBalanceByAccountIdResponse>> GetAccountsBalanceByAccountIdCallAsync(
        CancellationToken ct = default);

    Task<Call<GetDepositWithdrawRequest, GetDepositWithdrawResponse>> GetDepositWithdrawCallAsync(
        GetDepositWithdrawRequest request,
        CancellationToken ct = default);

    Task<Call<GetWithdrawVirtualAddressesRequest, GetWithdrawVirtualAddressesResponse>> GetWithdrawVirtualAddressesCallAsync(
        CancellationToken ct = default);

    Task<Call<GetRetailAccountBalanceRequest, GetRetailAccountBalanceResponse>> GetRetailAccountBalanceCallAsync(
        CancellationToken ct = default);

    Task<Call<PostOrdersPlaceRequest, PostOrdersPlaceResponse>> PostOrdersPlaceCallAsync(
        PostOrdersPlaceRequest request,
        CancellationToken ct = default);

    Task<Call<GetOrdersRequest, GetOrdersResponse>> GetOrdersCallAsync(
        GetOrdersRequest request,
        CancellationToken ct = default);

    Task<Call<PostOrdersSubmitCancelByOrderIdRequest, PostOrdersSubmitCancelByOrderIdResponse>> PostOrdersSubmitCancelByOrderIdCallAsync(
        PostOrdersSubmitCancelByOrderIdRequest request,
        CancellationToken ct = default);

    Task<Call<PostOrdersBatchCancelRequest, PostOrdersBatchCancelResponse>> PostOrdersBatchCancelCallAsync(
        PostOrdersBatchCancelRequest request,
        CancellationToken ct = default);

    Task<Call<PostOrdersBatchCancelOpenOrdersRequest, PostOrdersBatchCancelOpenOrdersResponse>> PostOrdersBatchCancelOpenOrdersCallAsync(
        PostOrdersBatchCancelOpenOrdersRequest request,
        CancellationToken ct = default);

    Task<Call<GetOpenOrdersRequest, GetOpenOrdersResponse>> GetOpenOrdersCallAsync(
        GetOpenOrdersRequest request,
        CancellationToken ct = default);

    Task<Call<GetOrdersByOrderIdRequest, GetOrdersByOrderIdResponse>> GetOrdersByOrderIdCallAsync(
        GetOrdersByOrderIdRequest request,
        CancellationToken ct = default);

    Task<Call<GetOrdersMatchResultsByOrderIdRequest, GetOrdersMatchResultsByOrderIdResponse>> GetOrdersMatchResultsByOrderIdCallAsync(
        GetOrdersMatchResultsByOrderIdRequest request,
        CancellationToken ct = default);

    Task<Call<GetMatchResultsRequest, GetMatchResultsResponse>> GetMatchResultsCallAsync(
        GetMatchResultsRequest request,
        CancellationToken ct = default);

    Task<Call<PostWithdrawApiCreateRequest, PostWithdrawApiCreateResponse>> PostWithdrawApiCreateCallAsync(
        PostWithdrawApiCreateRequest request,
        CancellationToken ct = default);

    Task<Call<PostWithdrawVirtualByAddressIdCreateRequest, PostWithdrawVirtualByAddressIdCreateResponse>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        PostWithdrawVirtualByAddressIdCreateRequest request,
        CancellationToken ct = default);

    Task<Call<PostWithdrawVirtualByWithdrawIdPlaceRequest, PostWithdrawVirtualByWithdrawIdPlaceResponse>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        PostWithdrawVirtualByWithdrawIdPlaceRequest request,
        CancellationToken ct = default);

    Task<Call<PostWithdrawVirtualByWithdrawIdCancelRequest, PostWithdrawVirtualByWithdrawIdCancelResponse>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        PostWithdrawVirtualByWithdrawIdCancelRequest request,
        CancellationToken ct = default);

    Task<Call<PostRetailOrderPlaceRequest, PostRetailOrderPlaceResponse>> PostRetailOrderPlaceCallAsync(
        PostRetailOrderPlaceRequest request,
        CancellationToken ct = default);

    Task<Call<GetRetailOrderListRequest, GetRetailOrderListResponse>> GetRetailOrderListCallAsync(
        GetRetailOrderListRequest request,
        CancellationToken ct = default);

    Task<Call<GetRetailOrderDetailByOrderIdRequest, GetRetailOrderDetailByOrderIdResponse>> GetRetailOrderDetailByOrderIdCallAsync(
        GetRetailOrderDetailByOrderIdRequest request,
        CancellationToken ct = default);

    Task<Call<PostRetailOrderHistoryRequest, PostRetailOrderHistoryResponse>> PostRetailOrderHistoryCallAsync(
        PostRetailOrderHistoryRequest request,
        CancellationToken ct = default);

    Task<Call<PostRetailOrderDetailRequest, PostRetailOrderDetailResponse>> PostRetailOrderDetailCallAsync(
        PostRetailOrderDetailRequest request,
        CancellationToken ct = default);

    Task<Call<PostRetailOrderCreateRequest, PostRetailOrderCreateResponse>> PostRetailOrderCreateCallAsync(
        PostRetailOrderCreateRequest request,
        CancellationToken ct = default);

    Task<Call<PostRetailOrderCancelByOrderIdRequest, PostRetailOrderCancelByOrderIdResponse>> PostRetailOrderCancelByOrderIdCallAsync(
        PostRetailOrderCancelByOrderIdRequest request,
        CancellationToken ct = default);
}
