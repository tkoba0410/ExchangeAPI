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
    Task<Call<GetDetailMergedRequest, TickerNormalized>> GetDetailMergedCallAsync(
        ProductCode productCode,
        CancellationToken ct = default);

    Task<Call<GetDepthRequest, OrderBookNormalized>> GetDepthCallAsync(
        ProductCode productCode,
        DepthType? depthType = null,
        CancellationToken ct = default);

    Task<Call<GetTradeRequest, IReadOnlyList<ExecutionNormalized>>> GetTradeCallAsync(
        ProductCode productCode,
        CancellationToken ct = default);

    Task<Call<GetSymbolsRequest, IReadOnlyList<SymbolNormalized>>> GetSymbolsCallAsync(
        CancellationToken ct = default);

    Task<Call<GetCurrencysRequest, IReadOnlyList<CurrencyCode>>> GetCurrencysCallAsync(
        CancellationToken ct = default);

    Task<Call<GetTimestampRequest, DateTimeOffset>> GetTimestampCallAsync(
        CancellationToken ct = default);

    Task<Call<GetHistoryKlineRequest, IReadOnlyList<KlineNormalized>>> GetHistoryKlineCallAsync(
        ProductCode productCode,
        Period period,
        int? size = null,
        CancellationToken ct = default);

    Task<Call<GetTickersRequest, IReadOnlyList<TickerEntryNormalized>>> GetTickersCallAsync(
        CancellationToken ct = default);

    Task<Call<GetHistoryTradeRequest, IReadOnlyList<ExecutionNormalized>>> GetHistoryTradeCallAsync(
        ProductCode productCode,
        CancellationToken ct = default);

    Task<Call<GetAccountsRequest, IReadOnlyList<AccountNormalized>>> GetAccountsCallAsync(
        CancellationToken ct = default);

    Task<Call<GetAccountsBalanceByAccountIdRequest, IReadOnlyList<BalanceEntryNormalized>>> GetAccountsBalanceByAccountIdCallAsync(
        CancellationToken ct = default);

    Task<Call<GetDepositWithdrawRequest, IReadOnlyList<DepositWithdrawNormalized>>> GetDepositWithdrawCallAsync(
        GetDepositWithdrawRequest request,
        CancellationToken ct = default);

    Task<Call<GetWithdrawVirtualAddressesRequest, IReadOnlyList<WithdrawVirtualAddressNormalized>>> GetWithdrawVirtualAddressesCallAsync(
        CancellationToken ct = default);

    Task<Call<GetRetailAccountBalanceRequest, GetRetailAccountBalanceResponse>> GetRetailAccountBalanceCallAsync(
        CancellationToken ct = default);

    Task<Call<PostOrdersPlaceRequest, OrderResult>> PostOrdersPlaceCallAsync(
        PostOrdersPlaceRequest request,
        CancellationToken ct = default);

    Task<Call<GetOrdersRequest, IReadOnlyList<OrderSummaryNormalized>>> GetOrdersCallAsync(
        GetOrdersRequest request,
        CancellationToken ct = default);

    Task<Call<PostOrdersSubmitCancelByOrderIdRequest, CancelResult>> PostOrdersSubmitCancelByOrderIdCallAsync(
        PostOrdersSubmitCancelByOrderIdRequest request,
        CancellationToken ct = default);

    Task<Call<PostOrdersBatchCancelRequest, CancelResult>> PostOrdersBatchCancelCallAsync(
        PostOrdersBatchCancelRequest request,
        CancellationToken ct = default);

    Task<Call<PostOrdersBatchCancelOpenOrdersRequest, CancelResult>> PostOrdersBatchCancelOpenOrdersCallAsync(
        PostOrdersBatchCancelOpenOrdersRequest request,
        CancellationToken ct = default);

    Task<Call<GetOpenOrdersRequest, IReadOnlyList<OpenOrder>>> GetOpenOrdersCallAsync(
        GetOpenOrdersRequest request,
        CancellationToken ct = default);

    Task<Call<GetOrdersByOrderIdRequest, OrderStatus>> GetOrdersByOrderIdCallAsync(
        GetOrdersByOrderIdRequest request,
        CancellationToken ct = default);

    Task<Call<GetOrdersMatchResultsByOrderIdRequest, IReadOnlyList<ExecutionNormalized>>> GetOrdersMatchResultsByOrderIdCallAsync(
        GetOrdersMatchResultsByOrderIdRequest request,
        CancellationToken ct = default);

    Task<Call<GetMatchResultsRequest, IReadOnlyList<ExecutionNormalized>>> GetMatchResultsCallAsync(
        GetMatchResultsRequest request,
        CancellationToken ct = default);

    Task<Call<PostWithdrawApiCreateRequest, WithdrawResult>> PostWithdrawApiCreateCallAsync(
        PostWithdrawApiCreateRequest request,
        CancellationToken ct = default);

    Task<Call<PostWithdrawVirtualByAddressIdCreateRequest, WithdrawResult>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        PostWithdrawVirtualByAddressIdCreateRequest request,
        CancellationToken ct = default);

    Task<Call<PostWithdrawVirtualByWithdrawIdPlaceRequest, WithdrawResult>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        PostWithdrawVirtualByWithdrawIdPlaceRequest request,
        CancellationToken ct = default);

    Task<Call<PostWithdrawVirtualByWithdrawIdCancelRequest, WithdrawResult>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        PostWithdrawVirtualByWithdrawIdCancelRequest request,
        CancellationToken ct = default);

    Task<Call<PostRetailOrderPlaceRequest, RetailOrderResult>> PostRetailOrderPlaceCallAsync(
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

    Task<Call<PostRetailOrderCreateRequest, RetailOrderResult>> PostRetailOrderCreateCallAsync(
        PostRetailOrderCreateRequest request,
        CancellationToken ct = default);

    Task<Call<PostRetailOrderCancelByOrderIdRequest, RetailOrderResult>> PostRetailOrderCancelByOrderIdCallAsync(
        PostRetailOrderCancelByOrderIdRequest request,
        CancellationToken ct = default);
}
