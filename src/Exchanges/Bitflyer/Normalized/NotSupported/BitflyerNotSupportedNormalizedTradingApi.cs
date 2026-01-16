using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Apis;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Dtos;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Requests;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.NotSupported;

internal sealed class BitflyerNotSupportedNormalizedTradingApi : IBitflyerNormalizedTradingApi
{
    private const string Layer = "Normalized";
    private const string Component = "Bitflyer.NotSupported";

    public Task<Call<PlaceOrderRequest, OrderResult>> PlaceOrderCallAsync(
        OrderRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall.Create<PlaceOrderRequest, OrderResult>(
            Layer,
            Component,
            new PlaceOrderRequest(request),
            "Trading.PlaceOrder"));

    public Task<Call<CancelOrderRequest, CancelResult>> CancelOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall.Create<CancelOrderRequest, CancelResult>(
            Layer,
            Component,
            new CancelOrderRequest(symbol, orderKey),
            "Trading.CancelOrder"));

    public Task<Call<GetOpenOrdersRequest, IReadOnlyList<OpenOrder>>> GetOpenOrdersCallAsync(
        Symbol symbol,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall.Create<GetOpenOrdersRequest, IReadOnlyList<OpenOrder>>(
            Layer,
            Component,
            new GetOpenOrdersRequest(symbol),
            "Trading.GetOpenOrders"));

    public Task<Call<GetOrderRequest, OrderStatus>> GetOrderCallAsync(
        Symbol symbol,
        OrderKey orderKey,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall.Create<GetOrderRequest, OrderStatus>(
            Layer,
            Component,
            new GetOrderRequest(symbol, orderKey),
            "Trading.GetOrder"));

    public Task<Call<SendParentOrderRequest, BitflyerParentOrderAcceptance>> SendParentOrderCallAsync(
        SendParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall.Create<SendParentOrderRequest, BitflyerParentOrderAcceptance>(
            Layer,
            Component,
            request,
            "Trading.SendParentOrder"));

    public Task<Call<CancelParentOrderRequest, BitflyerParentOrderCancelResult>> CancelParentOrderCallAsync(
        CancelParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall.Create<CancelParentOrderRequest, BitflyerParentOrderCancelResult>(
            Layer,
            Component,
            request,
            "Trading.CancelParentOrder"));

    public Task<Call<GetParentOrdersRequest, IReadOnlyList<BitflyerParentOrderNormalized>>> GetParentOrdersCallAsync(
        GetParentOrdersRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall.Create<GetParentOrdersRequest, IReadOnlyList<BitflyerParentOrderNormalized>>(
            Layer,
            Component,
            request,
            "Trading.GetParentOrders"));

    public Task<Call<GetParentOrderRequest, BitflyerParentOrderDetailNormalized>> GetParentOrderCallAsync(
        GetParentOrderRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(NotSupportedCall.Create<GetParentOrderRequest, BitflyerParentOrderDetailNormalized>(
            Layer,
            Component,
            request,
            "Trading.GetParentOrder"));
}
