using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Types;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Api;

public interface IBittradeNormalizedApi
{
    Task<Call<GetTickerRequest, BittradeTickerNormalized>> GetDetailMergedCallAsync(
        ProductCode productCode,
        CancellationToken ct = default);

    Task<Call<GetOrderBookRequest, BittradeOrderBookNormalized>> GetDepthCallAsync(
        ProductCode productCode,
        BittradeDepthType? depthType = null,
        CancellationToken ct = default);

    Task<Call<GetExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetTradeCallAsync(
        ProductCode productCode,
        CancellationToken ct = default);

    Task<Call<GetSymbolsRequest, IReadOnlyList<BittradeSymbolNormalized>>> GetSymbolsCallAsync(
        CancellationToken ct = default);

    Task<Call<GetCurrencysRequest, IReadOnlyList<CurrencyCode>>> GetCurrencysCallAsync(
        CancellationToken ct = default);

    Task<Call<GetTimestampRequest, DateTimeOffset>> GetTimestampCallAsync(
        CancellationToken ct = default);

    Task<Call<GetHistoryKlineRequest, IReadOnlyList<BittradeKlineNormalized>>> GetHistoryKlineCallAsync(
        ProductCode productCode,
        Period period,
        int? size = null,
        CancellationToken ct = default);

    Task<Call<GetTickersRequest, IReadOnlyList<BittradeTickerEntryNormalized>>> GetTickersCallAsync(
        CancellationToken ct = default);

    Task<Call<GetHistoryTradeRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetHistoryTradeCallAsync(
        ProductCode productCode,
        CancellationToken ct = default);

    Task<Call<GetAccountsRequest, IReadOnlyList<BittradeAccountNormalized>>> GetAccountsCallAsync(
        CancellationToken ct = default);

    Task<Call<GetBalancesRequest, IReadOnlyList<BittradeBalanceEntryNormalized>>> GetAccountsBalanceByAccountIdCallAsync(
        CancellationToken ct = default);

    Task<Call<GetDepositWithdrawRequest, IReadOnlyList<BittradeDepositWithdrawNormalized>>> GetDepositWithdrawCallAsync(
        GetDepositWithdrawRequest request,
        CancellationToken ct = default);

    Task<Call<GetWithdrawVirtualAddressesRequest, IReadOnlyList<BittradeWithdrawVirtualAddressNormalized>>> GetWithdrawVirtualAddressesCallAsync(
        CancellationToken ct = default);

    Task<Call<GetRetailAccountBalanceRequest, IReadOnlyList<BittradeRetailBalanceEntryNormalized>>> GetRetailAccountBalanceCallAsync(
        CancellationToken ct = default);

    Task<Call<PostOrdersPlaceRequest, BittradeOrderResult>> PostOrdersPlaceCallAsync(
        PostOrdersPlaceRequest request,
        CancellationToken ct = default);

    Task<Call<GetOrdersRequest, IReadOnlyList<BittradeOrderSummaryNormalized>>> GetOrdersCallAsync(
        GetOrdersRequest request,
        CancellationToken ct = default);

    Task<Call<PostOrdersSubmitCancelByOrderIdRequest, BittradeCancelResult>> PostOrdersSubmitCancelByOrderIdCallAsync(
        PostOrdersSubmitCancelByOrderIdRequest request,
        CancellationToken ct = default);

    Task<Call<PostOrdersBatchCancelRequest, BittradeCancelResult>> PostOrdersBatchCancelCallAsync(
        PostOrdersBatchCancelRequest request,
        CancellationToken ct = default);

    Task<Call<PostOrdersBatchCancelOpenOrdersRequest, BittradeCancelResult>> PostOrdersBatchCancelOpenOrdersCallAsync(
        PostOrdersBatchCancelOpenOrdersRequest request,
        CancellationToken ct = default);

    Task<Call<GetOpenOrdersRequest, IReadOnlyList<BittradeOpenOrder>>> GetOpenOrdersCallAsync(
        GetOpenOrdersRequest request,
        CancellationToken ct = default);

    Task<Call<GetOrderRequest, BittradeOrderStatus>> GetOrdersByOrderIdCallAsync(
        GetOrderRequest request,
        CancellationToken ct = default);

    Task<Call<GetOrdersMatchResultsByOrderIdRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetOrdersMatchResultsByOrderIdCallAsync(
        GetOrdersMatchResultsByOrderIdRequest request,
        CancellationToken ct = default);

    Task<Call<GetAccountExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetMatchResultsCallAsync(
        GetAccountExecutionsRequest request,
        CancellationToken ct = default);

    Task<Call<PostWithdrawApiCreateRequest, BittradeWithdrawResult>> PostWithdrawApiCreateCallAsync(
        PostWithdrawApiCreateRequest request,
        CancellationToken ct = default);

    Task<Call<PostWithdrawVirtualByAddressIdCreateRequest, BittradeWithdrawResult>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        PostWithdrawVirtualByAddressIdCreateRequest request,
        CancellationToken ct = default);

    Task<Call<PostWithdrawVirtualByWithdrawIdPlaceRequest, BittradeWithdrawResult>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        PostWithdrawVirtualByWithdrawIdPlaceRequest request,
        CancellationToken ct = default);

    Task<Call<PostWithdrawVirtualByWithdrawIdCancelRequest, BittradeWithdrawResult>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        PostWithdrawVirtualByWithdrawIdCancelRequest request,
        CancellationToken ct = default);

    Task<Call<PostRetailOrderPlaceRequest, BittradeRetailOrderResult>> PostRetailOrderPlaceCallAsync(
        PostRetailOrderPlaceRequest request,
        CancellationToken ct = default);

    Task<Call<GetRetailOrderListRequest, IReadOnlyList<BittradeRetailOrderEntryNormalized>>> GetRetailOrderListCallAsync(
        GetRetailOrderListRequest request,
        CancellationToken ct = default);

    Task<Call<GetRetailOrderDetailByOrderIdRequest, BittradeRetailOrderEntryNormalized?>> GetRetailOrderDetailByOrderIdCallAsync(
        GetRetailOrderDetailByOrderIdRequest request,
        CancellationToken ct = default);

    Task<Call<PostRetailOrderHistoryRequest, IReadOnlyList<BittradeRetailOrderEntryNormalized>>> PostRetailOrderHistoryCallAsync(
        PostRetailOrderHistoryRequest request,
        CancellationToken ct = default);

    Task<Call<PostRetailOrderDetailRequest, BittradeRetailOrderEntryNormalized?>> PostRetailOrderDetailCallAsync(
        PostRetailOrderDetailRequest request,
        CancellationToken ct = default);

    Task<Call<PostRetailOrderCreateRequest, BittradeRetailOrderResult>> PostRetailOrderCreateCallAsync(
        PostRetailOrderCreateRequest request,
        CancellationToken ct = default);

    Task<Call<PostRetailOrderCancelByOrderIdRequest, BittradeRetailOrderResult>> PostRetailOrderCancelByOrderIdCallAsync(
        PostRetailOrderCancelByOrderIdRequest request,
        CancellationToken ct = default);
}
