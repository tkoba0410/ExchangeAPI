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

internal sealed class BittradeNotSupportedNormalizedTradingApi : IBittradeNormalizedTradingApi
{
    private const string Layer = "Normalized";
    private const string Component = "Bittrade.NotSupported";

    public Task<Call<PlaceOrderRequest, BittradeOrderResult>> PlaceOrderCallAsync(
        BittradeOrderRequest request,
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<PlaceOrderRequest, BittradeOrderResult>(
            Layer,
            Component,
            new PlaceOrderRequest(request),
            "Trading.PlaceOrder"));

    public Task<Call<CancelOrderRequest, BittradeCancelResult>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<CancelOrderRequest, BittradeCancelResult>(
            Layer,
            Component,
            new CancelOrderRequest(symbol, orderKey),
            "Trading.CancelOrder"));

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

    public Task<Call<GetAccountExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetMatchResultsCallAsync(
        Symbol symbol,
        int? limit = null,
        CancellationToken ct = default) =>
        Task.FromResult(NotSupportedCall.Create<GetAccountExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>(
            Layer,
            Component,
            new GetAccountExecutionsRequest(symbol, limit),
            "Trading.GetExecutions"));
}
