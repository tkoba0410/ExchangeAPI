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
