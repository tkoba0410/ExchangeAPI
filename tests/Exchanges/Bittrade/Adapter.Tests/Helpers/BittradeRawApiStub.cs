using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Api.Raw.Api;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Helpers;

internal abstract class BittradeRawApiStub : IBittradeRawApi
{
    protected static Exception CreateException() => new InvalidOperationException("Raw API should not be called.");

    public virtual Task<Call<RawPublicRequests.GetDetailMergedRequest, RawPublicDtos.GetDetailMergedResponse>> GetDetailMergedCallAsync(
        RawPublicRequests.GetDetailMergedRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPublicRequests.GetDepthRequest, RawPublicDtos.GetDepthResponse>> GetDepthCallAsync(
        RawPublicRequests.GetDepthRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPublicRequests.GetTradeRequest, RawPublicDtos.GetTradeResponse>> GetTradeCallAsync(
        RawPublicRequests.GetTradeRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPublicRequests.GetSymbolsRequest, RawPublicDtos.GetSymbolsResponse>> GetSymbolsCallAsync(
        RawPublicRequests.GetSymbolsRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPublicRequests.GetCurrencysRequest, RawPublicDtos.GetCurrencysResponse>> GetCurrencysCallAsync(
        RawPublicRequests.GetCurrencysRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPublicRequests.GetTimestampRequest, RawPublicDtos.GetTimestampResponse>> GetTimestampCallAsync(
        RawPublicRequests.GetTimestampRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPublicRequests.GetHistoryKlineRequest, RawPublicDtos.GetHistoryKlineResponse>> GetHistoryKlineCallAsync(
        RawPublicRequests.GetHistoryKlineRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPublicRequests.GetTickersRequest, RawPublicDtos.GetTickersResponse>> GetTickersCallAsync(
        RawPublicRequests.GetTickersRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPublicRequests.GetHistoryTradeRequest, RawPublicDtos.GetHistoryTradeResponse>> GetHistoryTradeCallAsync(
        RawPublicRequests.GetHistoryTradeRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.GetAccountsRequest, RawPrivateDtos.GetAccountsResponse>> GetAccountsCallAsync(
        RawPrivateRequests.GetAccountsRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.GetAccountsBalanceByAccountIdRequest, RawPrivateDtos.GetAccountsBalanceByAccountIdResponse>> GetAccountsBalanceByAccountIdCallAsync(
        RawPrivateRequests.GetAccountsBalanceByAccountIdRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.GetOpenOrdersRequest, RawPrivateDtos.GetOpenOrdersResponse>> GetOpenOrdersCallAsync(
        RawPrivateRequests.GetOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.GetOrdersRequest, RawPrivateDtos.GetOrdersResponse>> GetOrdersCallAsync(
        RawPrivateRequests.GetOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.GetOrdersByOrderIdRequest, RawPrivateDtos.GetOrdersByOrderIdResponse>> GetOrdersByOrderIdCallAsync(
        RawPrivateRequests.GetOrdersByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.GetOrdersMatchResultsByOrderIdRequest, RawPrivateDtos.GetOrdersMatchResultsByOrderIdResponse>> GetOrdersMatchResultsByOrderIdCallAsync(
        RawPrivateRequests.GetOrdersMatchResultsByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.GetMatchResultsRequest, RawPrivateDtos.GetMatchResultsResponse>> GetMatchResultsCallAsync(
        RawPrivateRequests.GetMatchResultsRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.GetDepositWithdrawRequest, RawPrivateDtos.GetDepositWithdrawResponse>> GetDepositWithdrawCallAsync(
        RawPrivateRequests.GetDepositWithdrawRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.GetWithdrawVirtualAddressesRequest, RawPrivateDtos.GetWithdrawVirtualAddressesResponse>> GetWithdrawVirtualAddressesCallAsync(
        RawPrivateRequests.GetWithdrawVirtualAddressesRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.GetRetailOrderListRequest, RawPrivateDtos.GetRetailOrderListResponse>> GetRetailOrderListCallAsync(
        RawPrivateRequests.GetRetailOrderListRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.GetRetailOrderDetailByOrderIdRequest, RawPrivateDtos.GetRetailOrderDetailByOrderIdResponse>> GetRetailOrderDetailByOrderIdCallAsync(
        RawPrivateRequests.GetRetailOrderDetailByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.GetRetailAccountBalanceRequest, RawPrivateDtos.GetRetailAccountBalanceResponse>> GetRetailAccountBalanceCallAsync(
        RawPrivateRequests.GetRetailAccountBalanceRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.PostOrdersPlaceRequest, RawPrivateDtos.PostOrdersPlaceResponse>> PostOrdersPlaceCallAsync(
        RawPrivateRequests.PostOrdersPlaceRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.PostOrdersSubmitCancelByOrderIdRequest, RawPrivateDtos.PostOrdersSubmitCancelByOrderIdResponse>> PostOrdersSubmitCancelByOrderIdCallAsync(
        RawPrivateRequests.PostOrdersSubmitCancelByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.PostOrdersBatchCancelRequest, RawPrivateDtos.PostOrdersBatchCancelResponse>> PostOrdersBatchCancelCallAsync(
        RawPrivateRequests.PostOrdersBatchCancelRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.PostOrdersBatchCancelOpenOrdersRequest, RawPrivateDtos.PostOrdersBatchCancelOpenOrdersResponse>> PostOrdersBatchCancelOpenOrdersCallAsync(
        RawPrivateRequests.PostOrdersBatchCancelOpenOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.PostWithdrawApiCreateRequest, RawPrivateDtos.PostWithdrawApiCreateResponse>> PostWithdrawApiCreateCallAsync(
        RawPrivateRequests.PostWithdrawApiCreateRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.PostWithdrawVirtualByAddressIdCreateRequest, RawPrivateDtos.PostWithdrawVirtualByAddressIdCreateResponse>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        RawPrivateRequests.PostWithdrawVirtualByAddressIdCreateRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.PostWithdrawVirtualByWithdrawIdCancelRequest, RawPrivateDtos.PostWithdrawVirtualByWithdrawIdCancelResponse>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        RawPrivateRequests.PostWithdrawVirtualByWithdrawIdCancelRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.PostWithdrawVirtualByWithdrawIdPlaceRequest, RawPrivateDtos.PostWithdrawVirtualByWithdrawIdPlaceResponse>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        RawPrivateRequests.PostWithdrawVirtualByWithdrawIdPlaceRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.PostRetailOrderPlaceRequest, RawPrivateDtos.PostRetailOrderPlaceResponse>> PostRetailOrderPlaceCallAsync(
        RawPrivateRequests.PostRetailOrderPlaceRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.PostRetailOrderCancelByOrderIdRequest, RawPrivateDtos.PostRetailOrderCancelByOrderIdResponse>> PostRetailOrderCancelByOrderIdCallAsync(
        RawPrivateRequests.PostRetailOrderCancelByOrderIdRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.PostRetailOrderHistoryRequest, RawPrivateDtos.PostRetailOrderHistoryResponse>> PostRetailOrderHistoryCallAsync(
        RawPrivateRequests.PostRetailOrderHistoryRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.PostRetailOrderDetailRequest, RawPrivateDtos.PostRetailOrderDetailResponse>> PostRetailOrderDetailCallAsync(
        RawPrivateRequests.PostRetailOrderDetailRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();

    public virtual Task<Call<RawPrivateRequests.PostRetailOrderCreateRequest, RawPrivateDtos.PostRetailOrderCreateResponse>> PostRetailOrderCreateCallAsync(
        RawPrivateRequests.PostRetailOrderCreateRequest request,
        CancellationToken cancellationToken = default) =>
        throw CreateException();
}
