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

namespace ExchangeApi.Exchanges.Bittrade.Normalized.NotSupported;

internal sealed class BittradePreconditionMissingNormalizedTradingApi : IBittradeNormalizedTradingApi
{
    private const string Layer = "Normalized";
    private const string Component = "Bittrade.PreconditionMissing";

    public BittradePreconditionMissingNormalizedTradingApi(string accountId)
    {
        _ = accountId;
    }

    public Task<Call<PlaceOrderRequest, BittradeOrderResult>> PlaceOrderCallAsync(
        BittradeOrderRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<PlaceOrderRequest, BittradeOrderResult>(
            new PlaceOrderRequest(request)));

    public Task<Call<GetOrdersRequest, IReadOnlyList<BittradeOrderSummaryNormalized>>> GetOrdersCallAsync(
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<GetOrdersRequest, IReadOnlyList<BittradeOrderSummaryNormalized>>(
            new GetOrdersRequest()));

    public Task<Call<CancelOrderRequest, BittradeCancelResult>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<CancelOrderRequest, BittradeCancelResult>(
            new CancelOrderRequest(symbol, orderKey)));

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

    public Task<Call<GetAccountExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetMatchResultsCallAsync(
        Symbol symbol,
        int? limit = null,
        CancellationToken ct = default) =>
        Task.FromResult(CreatePreconditionMissing<GetAccountExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>(
            new GetAccountExecutionsRequest(symbol, limit)));

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
