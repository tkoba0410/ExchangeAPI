using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos.Trading;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Private.NotSupported;

internal sealed class BittradePreconditionMissingNormalizedTradingApi : IBittradeNormalizedTradingApi
{
    private const string Layer = "Normalized";
    private const string Component = "Bittrade.PreconditionMissing";

    public BittradePreconditionMissingNormalizedTradingApi(string accountId)
    {
        _ = accountId;
    }

    public Task<Call<PostOrdersPlaceRequest, BittradeOrderResult>> PostOrdersPlaceCallAsync(
        PostOrdersPlaceRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<PostOrdersPlaceRequest, BittradeOrderResult>(
            request));

    public Task<Call<GetOrdersRequest, IReadOnlyList<BittradeOrderSummaryNormalized>>> GetOrdersCallAsync(
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<GetOrdersRequest, IReadOnlyList<BittradeOrderSummaryNormalized>>(
            new GetOrdersRequest()));

    public Task<Call<PostOrdersSubmitCancelByOrderIdRequest, BittradeCancelResult>> PostOrdersSubmitCancelByOrderIdCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<PostOrdersSubmitCancelByOrderIdRequest, BittradeCancelResult>(
            new PostOrdersSubmitCancelByOrderIdRequest(symbol, orderKey)));

    public Task<Call<PostOrdersBatchCancelRequest, BittradeCancelResult>> PostOrdersBatchCancelCallAsync(
        PostOrdersBatchCancelRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<PostOrdersBatchCancelRequest, BittradeCancelResult>(
            request));

    public Task<Call<PostOrdersBatchCancelOpenOrdersRequest, BittradeCancelResult>> PostOrdersBatchCancelOpenOrdersCallAsync(
        PostOrdersBatchCancelOpenOrdersRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<PostOrdersBatchCancelOpenOrdersRequest, BittradeCancelResult>(
            request));

    public Task<Call<GetOpenOrdersRequest, IReadOnlyList<BittradeOpenOrder>>> GetOpenOrdersCallAsync(
        Symbol symbol,
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<GetOpenOrdersRequest, IReadOnlyList<BittradeOpenOrder>>(
            new GetOpenOrdersRequest(symbol)));

    public Task<Call<GetOrderRequest, BittradeOrderStatus>> GetOrdersByOrderIdCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<GetOrderRequest, BittradeOrderStatus>(
            new GetOrderRequest(symbol, orderKey)));

    public Task<Call<GetOrdersMatchResultsByOrderIdRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetOrdersMatchResultsByOrderIdCallAsync(
        GetOrdersMatchResultsByOrderIdRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<GetOrdersMatchResultsByOrderIdRequest, IReadOnlyList<BittradeExecutionNormalized>>(
            request));

    public Task<Call<GetAccountExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetMatchResultsCallAsync(
        Symbol symbol,
        int? limit = null,
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<GetAccountExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>(
            new GetAccountExecutionsRequest(symbol, limit)));

    public Task<Call<PostWithdrawApiCreateRequest, BittradeWithdrawResult>> PostWithdrawApiCreateCallAsync(
        PostWithdrawApiCreateRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<PostWithdrawApiCreateRequest, BittradeWithdrawResult>(
            request));

    public Task<Call<PostRetailOrderPlaceRequest, BittradeRetailOrderResult>> PostRetailOrderPlaceCallAsync(
        PostRetailOrderPlaceRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<PostRetailOrderPlaceRequest, BittradeRetailOrderResult>(
            request));

    public Task<Call<GetRetailOrderListRequest, IReadOnlyList<BittradeRetailOrderEntryNormalized>>> GetRetailOrderListCallAsync(
        GetRetailOrderListRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<GetRetailOrderListRequest, IReadOnlyList<BittradeRetailOrderEntryNormalized>>(
            request));

    public Task<Call<GetRetailOrderDetailByOrderIdRequest, BittradeRetailOrderEntryNormalized?>> GetRetailOrderDetailByOrderIdCallAsync(
        GetRetailOrderDetailByOrderIdRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<GetRetailOrderDetailByOrderIdRequest, BittradeRetailOrderEntryNormalized?>(
            request));

    public Task<Call<PostRetailOrderHistoryRequest, IReadOnlyList<BittradeRetailOrderEntryNormalized>>> PostRetailOrderHistoryCallAsync(
        PostRetailOrderHistoryRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<PostRetailOrderHistoryRequest, IReadOnlyList<BittradeRetailOrderEntryNormalized>>(
            request));

    public Task<Call<PostRetailOrderDetailRequest, BittradeRetailOrderEntryNormalized?>> PostRetailOrderDetailCallAsync(
        PostRetailOrderDetailRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<PostRetailOrderDetailRequest, BittradeRetailOrderEntryNormalized?>(
            request));

    public Task<Call<PostRetailOrderCreateRequest, BittradeRetailOrderResult>> PostRetailOrderCreateCallAsync(
        PostRetailOrderCreateRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<PostRetailOrderCreateRequest, BittradeRetailOrderResult>(
            request));

    public Task<Call<PostRetailOrderCancelByOrderIdRequest, BittradeRetailOrderResult>> PostRetailOrderCancelByOrderIdCallAsync(
        PostRetailOrderCancelByOrderIdRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<PostRetailOrderCancelByOrderIdRequest, BittradeRetailOrderResult>(
            request));

    public Task<Call<PostWithdrawVirtualByAddressIdCreateRequest, BittradeWithdrawResult>> PostWithdrawVirtualByAddressIdCreateCallAsync(
        PostWithdrawVirtualByAddressIdCreateRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<PostWithdrawVirtualByAddressIdCreateRequest, BittradeWithdrawResult>(
            request));

    public Task<Call<PostWithdrawVirtualByWithdrawIdPlaceRequest, BittradeWithdrawResult>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
        PostWithdrawVirtualByWithdrawIdPlaceRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<PostWithdrawVirtualByWithdrawIdPlaceRequest, BittradeWithdrawResult>(
            request));

    public Task<Call<PostWithdrawVirtualByWithdrawIdCancelRequest, BittradeWithdrawResult>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
        PostWithdrawVirtualByWithdrawIdCancelRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<PostWithdrawVirtualByWithdrawIdCancelRequest, BittradeWithdrawResult>(
            request));

    private Call<TReq, TOk> CreatePreconditionMissing<TReq, TOk>(TReq request)
    {
        var error = new CallError(CallErrorKind.Semantic, "PreconditionMissing:accountId");
        var meta = CallMeta.CreateInternal(Layer, Component);

        return new Call<TReq, TOk>(
            Id: CallId.New(),
            StartedAt: DateTimeOffset.UtcNow,
            Duration: TimeSpan.Zero,
            Request: request,
            Result: new CallResult<TOk>.Err(error),
            Meta: meta);
    }
}
