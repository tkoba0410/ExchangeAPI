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
        long? from = null,
        int? size = null,
        FreeText? direct = null,
        CancellationToken ct = default) =>
        api.GetDepositWithdrawCallAsync(
            new GetDepositWithdrawRequest(type, currency, from, size, direct),
            ct);

    public static Task<Call<PostOrdersPlaceRequest, PostOrdersPlaceResponse>> PostOrdersPlaceCallAsync(
        this IBittradeNormalizedApi api,
        OrderRequest request,
        CancellationToken ct = default) =>
        api.PostOrdersPlaceCallAsync(new PostOrdersPlaceRequest(request), ct);

    public static Task<Call<GetOrdersRequest, GetOrdersResponse>> GetOrdersCallAsync(
        this IBittradeNormalizedApi api,
        CancellationToken ct = default) =>
        api.GetOrdersCallAsync(new GetOrdersRequest(), ct);

    public static Task<Call<PostOrdersSubmitCancelByOrderIdRequest, PostOrdersSubmitCancelByOrderIdResponse>> PostOrdersSubmitCancelByOrderIdCallAsync(
        this IBittradeNormalizedApi api,
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default) =>
        api.PostOrdersSubmitCancelByOrderIdCallAsync(
            new PostOrdersSubmitCancelByOrderIdRequest(symbol, orderKey),
            ct);

    public static Task<Call<PostOrdersBatchCancelRequest, PostOrdersBatchCancelResponse>> PostOrdersBatchCancelCallAsync(
        this IBittradeNormalizedApi api,
        IReadOnlyList<OrderId> orderIds,
        CancellationToken ct = default) =>
        api.PostOrdersBatchCancelCallAsync(new PostOrdersBatchCancelRequest(orderIds), ct);

    public static Task<Call<PostOrdersBatchCancelOpenOrdersRequest, PostOrdersBatchCancelOpenOrdersResponse>> PostOrdersBatchCancelOpenOrdersCallAsync(
        this IBittradeNormalizedApi api,
        Symbol? symbol = null,
        Side? side = null,
        decimal? size = null,
        decimal? price = null,
        DateTimeOffset? createdAt = null,
        CancellationToken ct = default) =>
        api.PostOrdersBatchCancelOpenOrdersCallAsync(
            new PostOrdersBatchCancelOpenOrdersRequest(symbol, side, size, price, createdAt),
            ct);

    public static Task<Call<GetOpenOrdersRequest, GetOpenOrdersResponse>> GetOpenOrdersCallAsync(
        this IBittradeNormalizedApi api,
        Symbol symbol,
        CancellationToken ct = default) =>
        api.GetOpenOrdersCallAsync(new GetOpenOrdersRequest(symbol), ct);

    public static Task<Call<GetOrdersByOrderIdRequest, GetOrdersByOrderIdResponse>> GetOrdersByOrderIdCallAsync(
        this IBittradeNormalizedApi api,
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default) =>
        api.GetOrdersByOrderIdCallAsync(new GetOrdersByOrderIdRequest(symbol, orderKey), ct);

    public static Task<Call<GetOrdersMatchResultsByOrderIdRequest, GetOrdersMatchResultsByOrderIdResponse>> GetOrdersMatchResultsByOrderIdCallAsync(
        this IBittradeNormalizedApi api,
        OrderKey orderKey,
        CancellationToken ct = default) =>
        api.GetOrdersMatchResultsByOrderIdCallAsync(
            new GetOrdersMatchResultsByOrderIdRequest(orderKey),
            ct);

    public static Task<Call<GetMatchResultsRequest, GetMatchResultsResponse>> GetMatchResultsCallAsync(
        this IBittradeNormalizedApi api,
        Symbol symbol,
        int? limit = null,
        CancellationToken ct = default) =>
        api.GetMatchResultsCallAsync(new GetMatchResultsRequest(symbol, limit), ct);

    public static Task<Call<PostWithdrawApiCreateRequest, PostWithdrawApiCreateResponse>> PostWithdrawApiCreateCallAsync(
        this IBittradeNormalizedApi api,
        FreeText address,
        decimal amount,
        FreeText currency,
        decimal? fee = null,
        FreeText? addressTag = null,
        CancellationToken ct = default) =>
        api.PostWithdrawApiCreateCallAsync(
            new PostWithdrawApiCreateRequest(address, amount, currency, fee, addressTag),
            ct);

    public static Task<Call<PostWithdrawVirtualByAddressIdCreateRequest, PostWithdrawVirtualByAddressIdCreateResponse>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        this IBittradeNormalizedApi api,
        FreeText addressId,
        CancellationToken ct = default) =>
        api.PostWithdrawVirtualByAddressIdCreateCallAsync(
            new PostWithdrawVirtualByAddressIdCreateRequest(addressId),
            ct);

    public static Task<Call<PostWithdrawVirtualByWithdrawIdPlaceRequest, PostWithdrawVirtualByWithdrawIdPlaceResponse>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        this IBittradeNormalizedApi api,
        FreeText withdrawId,
        CancellationToken ct = default) =>
        api.PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
            new PostWithdrawVirtualByWithdrawIdPlaceRequest(withdrawId),
            ct);

    public static Task<Call<PostWithdrawVirtualByWithdrawIdCancelRequest, PostWithdrawVirtualByWithdrawIdCancelResponse>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        this IBittradeNormalizedApi api,
        FreeText withdrawId,
        CancellationToken ct = default) =>
        api.PostWithdrawVirtualByWithdrawIdCancelCallAsync(
            new PostWithdrawVirtualByWithdrawIdCancelRequest(withdrawId),
            ct);

    public static Task<Call<PostRetailOrderPlaceRequest, PostRetailOrderPlaceResponse>> PostRetailOrderPlaceCallAsync(
        this IBittradeNormalizedApi api,
        RetailOrderRequest request,
        CancellationToken ct = default) =>
        api.PostRetailOrderPlaceCallAsync(new PostRetailOrderPlaceRequest(request), ct);

    public static Task<Call<GetRetailOrderListRequest, GetRetailOrderListResponse>> GetRetailOrderListCallAsync(
        this IBittradeNormalizedApi api,
        int direct,
        int? status = null,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        CancellationToken ct = default) =>
        api.GetRetailOrderListCallAsync(
            new GetRetailOrderListRequest(direct, status, startTime, endTime),
            ct);

    public static Task<Call<GetRetailOrderDetailByOrderIdRequest, GetRetailOrderDetailByOrderIdResponse>> GetRetailOrderDetailByOrderIdCallAsync(
        this IBittradeNormalizedApi api,
        OrderId orderId,
        CancellationToken ct = default) =>
        api.GetRetailOrderDetailByOrderIdCallAsync(new GetRetailOrderDetailByOrderIdRequest(orderId), ct);

    public static Task<Call<PostRetailOrderHistoryRequest, PostRetailOrderHistoryResponse>> PostRetailOrderHistoryCallAsync(
        this IBittradeNormalizedApi api,
        Symbol? symbol = null,
        int? direct = null,
        int? status = null,
        DateTimeOffset? startTime = null,
        DateTimeOffset? endTime = null,
        int? size = null,
        CancellationToken ct = default) =>
        api.PostRetailOrderHistoryCallAsync(
            new PostRetailOrderHistoryRequest(symbol, direct, status, startTime, endTime, size),
            ct);

    public static Task<Call<PostRetailOrderDetailRequest, PostRetailOrderDetailResponse>> PostRetailOrderDetailCallAsync(
        this IBittradeNormalizedApi api,
        OrderId orderId,
        CancellationToken ct = default) =>
        api.PostRetailOrderDetailCallAsync(new PostRetailOrderDetailRequest(orderId), ct);

    public static Task<Call<PostRetailOrderCreateRequest, PostRetailOrderCreateResponse>> PostRetailOrderCreateCallAsync(
        this IBittradeNormalizedApi api,
        RetailOrderRequest request,
        CancellationToken ct = default) =>
        api.PostRetailOrderCreateCallAsync(new PostRetailOrderCreateRequest(request), ct);

    public static Task<Call<PostRetailOrderCancelByOrderIdRequest, PostRetailOrderCancelByOrderIdResponse>> PostRetailOrderCancelByOrderIdCallAsync(
        this IBittradeNormalizedApi api,
        OrderId orderId,
        CancellationToken ct = default) =>
        api.PostRetailOrderCancelByOrderIdCallAsync(new PostRetailOrderCancelByOrderIdRequest(orderId), ct);
}
