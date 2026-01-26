using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos.Trading;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;
using ExchangeApi.Primitives.CallCommon;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;

internal interface IBittradeNormalizedTradingApi
{
    Task<Call<PlaceOrderRequest, BittradeOrderResult>> PlaceOrderCallAsync(
        BittradeOrderRequest request,
        CancellationToken ct = default);

    Task<Call<GetOrdersRequest, IReadOnlyList<BittradeOrderSummaryNormalized>>> GetOrdersCallAsync(
        CancellationToken ct = default);

    Task<Call<CancelOrderRequest, BittradeCancelResult>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default);

    Task<Call<GetOpenOrdersRequest, IReadOnlyList<BittradeOpenOrder>>> GetOpenOrdersCallAsync(
        Symbol symbol,
        CancellationToken ct = default);

    Task<Call<GetOrderRequest, BittradeOrderStatus>> GetOrdersByOrderIdCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default);

    Task<Call<GetAccountExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetMatchResultsCallAsync(
        Symbol symbol,
        int? limit = null,
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

    Task<Call<PostWithdrawVirtualByAddressIdCreateRequest, BittradeWithdrawResult>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        PostWithdrawVirtualByAddressIdCreateRequest request,
        CancellationToken ct = default);

    Task<Call<PostWithdrawVirtualByWithdrawIdPlaceRequest, BittradeWithdrawResult>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        PostWithdrawVirtualByWithdrawIdPlaceRequest request,
        CancellationToken ct = default);

    Task<Call<PostWithdrawVirtualByWithdrawIdCancelRequest, BittradeWithdrawResult>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        PostWithdrawVirtualByWithdrawIdCancelRequest request,
        CancellationToken ct = default);
}
