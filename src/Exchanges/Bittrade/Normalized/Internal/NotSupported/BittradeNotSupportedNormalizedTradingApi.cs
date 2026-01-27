using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos.Trading;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Internal.NotSupported;

internal sealed class BittradeNotSupportedNormalizedTradingApi : IBittradeNormalizedTradingApi
{
    private const string Layer = "Normalized";
    private const string Component = "Bittrade.NotSupported";

    public Task<Call<PostOrdersPlaceRequest, BittradeOrderResult>> PostOrdersPlaceCallAsync(
        PostOrdersPlaceRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<PostOrdersPlaceRequest, BittradeOrderResult>(
            Layer,
            Component,
            request,
            "Trading.PlaceOrder"));

    public Task<Call<GetOrdersRequest, IReadOnlyList<BittradeOrderSummaryNormalized>>> GetOrdersCallAsync(
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<GetOrdersRequest, IReadOnlyList<BittradeOrderSummaryNormalized>>(
            Layer,
            Component,
            new GetOrdersRequest(),
            "Trading.GetOrders"));

    public Task<Call<PostOrdersSubmitCancelByOrderIdRequest, BittradeCancelResult>> PostOrdersSubmitCancelByOrderIdCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<PostOrdersSubmitCancelByOrderIdRequest, BittradeCancelResult>(
            Layer,
            Component,
            new PostOrdersSubmitCancelByOrderIdRequest(symbol, orderKey),
            "Trading.CancelOrder"));

    public Task<Call<PostOrdersBatchCancelRequest, BittradeCancelResult>> PostOrdersBatchCancelCallAsync(
        PostOrdersBatchCancelRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<PostOrdersBatchCancelRequest, BittradeCancelResult>(
            Layer,
            Component,
            request,
            "Trading.CancelOrders"));

    public Task<Call<PostOrdersBatchCancelOpenOrdersRequest, BittradeCancelResult>> PostOrdersBatchCancelOpenOrdersCallAsync(
        PostOrdersBatchCancelOpenOrdersRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<PostOrdersBatchCancelOpenOrdersRequest, BittradeCancelResult>(
            Layer,
            Component,
            request,
            "Trading.CancelOpenOrders"));

    public Task<Call<GetOpenOrdersRequest, IReadOnlyList<BittradeOpenOrder>>> GetOpenOrdersCallAsync(
        Symbol symbol,
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<GetOpenOrdersRequest, IReadOnlyList<BittradeOpenOrder>>(
            Layer,
            Component,
            new GetOpenOrdersRequest(symbol),
            "Trading.GetOpenOrders"));

    public Task<Call<GetOrderRequest, BittradeOrderStatus>> GetOrdersByOrderIdCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<GetOrderRequest, BittradeOrderStatus>(
            Layer,
            Component,
            new GetOrderRequest(symbol, orderKey),
            "Trading.GetOrder"));

    public Task<Call<GetOrdersMatchResultsByOrderIdRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetOrdersMatchResultsByOrderIdCallAsync(
        GetOrdersMatchResultsByOrderIdRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<GetOrdersMatchResultsByOrderIdRequest, IReadOnlyList<BittradeExecutionNormalized>>(
            Layer,
            Component,
            request,
            "Trading.GetOrderExecutions"));

    public Task<Call<GetAccountExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetMatchResultsCallAsync(
        Symbol symbol,
        int? limit = null,
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<GetAccountExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>(
            Layer,
            Component,
            new GetAccountExecutionsRequest(symbol, limit),
            "Trading.GetExecutions"));

    public Task<Call<PostWithdrawApiCreateRequest, BittradeWithdrawResult>> PostWithdrawApiCreateCallAsync(
        PostWithdrawApiCreateRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<PostWithdrawApiCreateRequest, BittradeWithdrawResult>(
            Layer,
            Component,
            request,
            "Trading.PostWithdrawApiCreate"));

    public Task<Call<PostRetailOrderPlaceRequest, BittradeRetailOrderResult>> PostRetailOrderPlaceCallAsync(
        PostRetailOrderPlaceRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<PostRetailOrderPlaceRequest, BittradeRetailOrderResult>(
            Layer,
            Component,
            request,
            "Trading.PostRetailOrderPlace"));

    public Task<Call<GetRetailOrderListRequest, IReadOnlyList<BittradeRetailOrderEntryNormalized>>> GetRetailOrderListCallAsync(
        GetRetailOrderListRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<GetRetailOrderListRequest, IReadOnlyList<BittradeRetailOrderEntryNormalized>>(
            Layer,
            Component,
            request,
            "Trading.GetRetailOrderList"));

    public Task<Call<GetRetailOrderDetailByOrderIdRequest, BittradeRetailOrderEntryNormalized?>> GetRetailOrderDetailByOrderIdCallAsync(
        GetRetailOrderDetailByOrderIdRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<GetRetailOrderDetailByOrderIdRequest, BittradeRetailOrderEntryNormalized?>(
            Layer,
            Component,
            request,
            "Trading.GetRetailOrderDetail"));

    public Task<Call<PostRetailOrderHistoryRequest, IReadOnlyList<BittradeRetailOrderEntryNormalized>>> PostRetailOrderHistoryCallAsync(
        PostRetailOrderHistoryRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<PostRetailOrderHistoryRequest, IReadOnlyList<BittradeRetailOrderEntryNormalized>>(
            Layer,
            Component,
            request,
            "Trading.PostRetailOrderHistory"));

    public Task<Call<PostRetailOrderDetailRequest, BittradeRetailOrderEntryNormalized?>> PostRetailOrderDetailCallAsync(
        PostRetailOrderDetailRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<PostRetailOrderDetailRequest, BittradeRetailOrderEntryNormalized?>(
            Layer,
            Component,
            request,
            "Trading.PostRetailOrderDetail"));

    public Task<Call<PostRetailOrderCreateRequest, BittradeRetailOrderResult>> PostRetailOrderCreateCallAsync(
        PostRetailOrderCreateRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<PostRetailOrderCreateRequest, BittradeRetailOrderResult>(
            Layer,
            Component,
            request,
            "Trading.PostRetailOrderCreate"));

    public Task<Call<PostRetailOrderCancelByOrderIdRequest, BittradeRetailOrderResult>> PostRetailOrderCancelByOrderIdCallAsync(
        PostRetailOrderCancelByOrderIdRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<PostRetailOrderCancelByOrderIdRequest, BittradeRetailOrderResult>(
            Layer,
            Component,
            request,
            "Trading.PostRetailOrderCancel"));

    public Task<Call<PostWithdrawVirtualByAddressIdCreateRequest, BittradeWithdrawResult>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        PostWithdrawVirtualByAddressIdCreateRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<PostWithdrawVirtualByAddressIdCreateRequest, BittradeWithdrawResult>(
            Layer,
            Component,
            request,
            "Trading.PostWithdrawVirtualByAddressIdCreate"));

    public Task<Call<PostWithdrawVirtualByWithdrawIdPlaceRequest, BittradeWithdrawResult>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        PostWithdrawVirtualByWithdrawIdPlaceRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<PostWithdrawVirtualByWithdrawIdPlaceRequest, BittradeWithdrawResult>(
            Layer,
            Component,
            request,
            "Trading.PostWithdrawVirtualByWithdrawIdPlace"));

    public Task<Call<PostWithdrawVirtualByWithdrawIdCancelRequest, BittradeWithdrawResult>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        PostWithdrawVirtualByWithdrawIdCancelRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<PostWithdrawVirtualByWithdrawIdCancelRequest, BittradeWithdrawResult>(
            Layer,
            Component,
            request,
            "Trading.PostWithdrawVirtualByWithdrawIdCancel"));
}
