using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalized.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ExchangeApi.Primitives.ValueCommon.ClosedSet;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Extensions;

public static class NormalizedApiExtensions
{
    public static Task<Call<GetDepositWithdrawRequest, GetDepositWithdrawResponse>> GetDepositWithdrawCallAsync(
        this IBittradeNormalizedApi api,
        Closed<ExchangeDepositWithdrawType> type,
        CurrencyCode? currency = null,
        RequestFrom? from = null,
        RequestSize? size = null,
        FreeText? direct = null,
        CancellationToken cancellationToken = default) =>
        api.GetDepositWithdrawCallAsync(
            new GetDepositWithdrawRequest(type, currency, from, size, direct),
            cancellationToken);

    public static Task<Call<PostOrdersPlaceRequest, PostOrdersPlaceResponse>> PostOrdersPlaceCallAsync(
        this IBittradeNormalizedApi api,
        OrderRequest request,
        CancellationToken cancellationToken = default) =>
        api.PostOrdersPlaceCallAsync(new PostOrdersPlaceRequest(request), cancellationToken);

    public static Task<Call<GetOrdersRequest, GetOrdersResponse>> GetOrdersCallAsync(
        this IBittradeNormalizedApi api,
        CancellationToken cancellationToken = default) =>
        api.GetOrdersCallAsync(new GetOrdersRequest(), cancellationToken);

    public static Task<Call<PostOrdersSubmitCancelByOrderIdRequest, PostOrdersSubmitCancelByOrderIdResponse>> PostOrdersSubmitCancelByOrderIdCallAsync(
        this IBittradeNormalizedApi api,
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        api.PostOrdersSubmitCancelByOrderIdCallAsync(
            new PostOrdersSubmitCancelByOrderIdRequest(symbol, orderKey),
            cancellationToken);

    public static Task<Call<PostOrdersBatchCancelRequest, PostOrdersBatchCancelResponse>> PostOrdersBatchCancelCallAsync(
        this IBittradeNormalizedApi api,
        IReadOnlyList<OrderId> orderIds,
        CancellationToken cancellationToken = default) =>
        api.PostOrdersBatchCancelCallAsync(new PostOrdersBatchCancelRequest(orderIds), cancellationToken);

    public static Task<Call<PostOrdersBatchCancelOpenOrdersRequest, PostOrdersBatchCancelOpenOrdersResponse>> PostOrdersBatchCancelOpenOrdersCallAsync(
        this IBittradeNormalizedApi api,
        Symbol? symbol = null,
        Side? side = null,
        Size? size = null,
        decimal? price = null,
        DateTimeOffset? createdAt = null,
        CancellationToken cancellationToken = default) =>
        api.PostOrdersBatchCancelOpenOrdersCallAsync(
            new PostOrdersBatchCancelOpenOrdersRequest(symbol, side, size, price, createdAt),
            cancellationToken);

    public static Task<Call<GetOpenOrdersRequest, GetOpenOrdersResponse>> GetOpenOrdersCallAsync(
        this IBittradeNormalizedApi api,
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        api.GetOpenOrdersCallAsync(new GetOpenOrdersRequest(symbol), cancellationToken);

    public static Task<Call<GetOrdersByOrderIdRequest, GetOrdersByOrderIdResponse>> GetOrdersByOrderIdCallAsync(
        this IBittradeNormalizedApi api,
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        api.GetOrdersByOrderIdCallAsync(new GetOrdersByOrderIdRequest(symbol, orderKey), cancellationToken);

    public static Task<Call<GetOrdersMatchResultsByOrderIdRequest, GetOrdersMatchResultsByOrderIdResponse>> GetOrdersMatchResultsByOrderIdCallAsync(
        this IBittradeNormalizedApi api,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        api.GetOrdersMatchResultsByOrderIdCallAsync(
            new GetOrdersMatchResultsByOrderIdRequest(orderKey),
            cancellationToken);

    public static Task<Call<GetMatchResultsRequest, GetMatchResultsResponse>> GetMatchResultsCallAsync(
        this IBittradeNormalizedApi api,
        Symbol symbol,
        RequestSize? limit = null,
        CancellationToken cancellationToken = default) =>
        api.GetMatchResultsCallAsync(new GetMatchResultsRequest(symbol, limit), cancellationToken);

    public static Task<Call<PostWithdrawApiCreateRequest, PostWithdrawApiCreateResponse>> PostWithdrawApiCreateCallAsync(
        this IBittradeNormalizedApi api,
        FreeText address,
        WithdrawAmount amount,
        FreeText currency,
        WithdrawFee? fee = null,
        FreeText? addressTag = null,
        CancellationToken cancellationToken = default) =>
        api.PostWithdrawApiCreateCallAsync(
            new PostWithdrawApiCreateRequest(address, amount, currency, fee, addressTag),
            cancellationToken);

    public static Task<Call<PostWithdrawVirtualByAddressIdCreateRequest, PostWithdrawVirtualByAddressIdCreateResponse>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        this IBittradeNormalizedApi api,
        FreeText addressId,
        CancellationToken cancellationToken = default) =>
        api.PostWithdrawVirtualByAddressIdCreateCallAsync(
            new PostWithdrawVirtualByAddressIdCreateRequest(addressId),
            cancellationToken);

    public static Task<Call<PostWithdrawVirtualByWithdrawIdPlaceRequest, PostWithdrawVirtualByWithdrawIdPlaceResponse>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        this IBittradeNormalizedApi api,
        FreeText withdrawId,
        CancellationToken cancellationToken = default) =>
        api.PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
            new PostWithdrawVirtualByWithdrawIdPlaceRequest(withdrawId),
            cancellationToken);

    public static Task<Call<PostWithdrawVirtualByWithdrawIdCancelRequest, PostWithdrawVirtualByWithdrawIdCancelResponse>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        this IBittradeNormalizedApi api,
        FreeText withdrawId,
        CancellationToken cancellationToken = default) =>
        api.PostWithdrawVirtualByWithdrawIdCancelCallAsync(
            new PostWithdrawVirtualByWithdrawIdCancelRequest(withdrawId),
            cancellationToken);

    public static Task<Call<PostRetailOrderPlaceRequest, PostRetailOrderPlaceResponse>> PostRetailOrderPlaceCallAsync(
        this IBittradeNormalizedApi api,
        RetailOrderRequest request,
        CancellationToken cancellationToken = default) =>
        api.PostRetailOrderPlaceCallAsync(new PostRetailOrderPlaceRequest(request), cancellationToken);

    public static Task<Call<GetRetailOrderListRequest, GetRetailOrderListResponse>> GetRetailOrderListCallAsync(
        this IBittradeNormalizedApi api,
        RetailOrderDirection direct,
        RetailOrderStatus? status = null,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        CancellationToken cancellationToken = default) =>
        api.GetRetailOrderListCallAsync(
            new GetRetailOrderListRequest(direct, status, startTime, endTime),
            cancellationToken);

    public static Task<Call<GetRetailOrderDetailByOrderIdRequest, GetRetailOrderDetailByOrderIdResponse>> GetRetailOrderDetailByOrderIdCallAsync(
        this IBittradeNormalizedApi api,
        OrderId orderId,
        CancellationToken cancellationToken = default) =>
        api.GetRetailOrderDetailByOrderIdCallAsync(new GetRetailOrderDetailByOrderIdRequest(orderId), cancellationToken);

    public static Task<Call<PostRetailOrderHistoryRequest, PostRetailOrderHistoryResponse>> PostRetailOrderHistoryCallAsync(
        this IBittradeNormalizedApi api,
        Symbol? symbol = null,
        RetailOrderDirection? direct = null,
        RetailOrderStatus? status = null,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        RequestSize? size = null,
        CancellationToken cancellationToken = default) =>
        api.PostRetailOrderHistoryCallAsync(
            new PostRetailOrderHistoryRequest(symbol, direct, status, startTime, endTime, size),
            cancellationToken);

    public static Task<Call<PostRetailOrderDetailRequest, PostRetailOrderDetailResponse>> PostRetailOrderDetailCallAsync(
        this IBittradeNormalizedApi api,
        OrderId orderId,
        CancellationToken cancellationToken = default) =>
        api.PostRetailOrderDetailCallAsync(new PostRetailOrderDetailRequest(orderId), cancellationToken);

    public static Task<Call<PostRetailOrderCreateRequest, PostRetailOrderCreateResponse>> PostRetailOrderCreateCallAsync(
        this IBittradeNormalizedApi api,
        RetailOrderRequest request,
        CancellationToken cancellationToken = default) =>
        api.PostRetailOrderCreateCallAsync(new PostRetailOrderCreateRequest(request), cancellationToken);

    public static Task<Call<PostRetailOrderCancelByOrderIdRequest, PostRetailOrderCancelByOrderIdResponse>> PostRetailOrderCancelByOrderIdCallAsync(
        this IBittradeNormalizedApi api,
        OrderId orderId,
        CancellationToken cancellationToken = default) =>
        api.PostRetailOrderCancelByOrderIdCallAsync(new PostRetailOrderCancelByOrderIdRequest(orderId), cancellationToken);
}
