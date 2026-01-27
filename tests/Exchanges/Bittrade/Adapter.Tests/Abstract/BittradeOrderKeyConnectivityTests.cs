using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Exchanges.Bittrade.Adapter.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos.Trading;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;
using ExchangeApi.Primitives.CallCommon;
using CommonSymbol = ExchangeApi.Primitives.DomainCommon.Types.Symbol;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public sealed class BittradeOrderKeyConnectivityTests
{
    [Fact]
    public async Task GetOrdersByOrderIdCallAsync_UsesOrderKeyValue_WithAcceptanceId()
    {
        var trading = new RecordingNormalizedTradingApi
        {
            Order = CreateOrderStatus("1001")
        };
        var api = new BittradeTradingApi(trading);

        var key = new OrderKey(OrderIdKind.AcceptanceId, "1001");
        var call = await api.GetOrdersByOrderIdCallAsync(new CommonSymbol("BTC/JPY"), key);
        var ok = Assert.IsType<CallResult<OrderStatus>.Ok>(call.Result);
        var status = ok.Response;

        Assert.Equal("1001", trading.LastOrderKey?.Value);
        Assert.Equal(OrderIdKind.AcceptanceId, status.Key.Kind);
        Assert.Equal("1001", status.Key.Value);
    }

    [Fact]
    public async Task CancelOrderAsync_UsesOrderKeyValue_WithAcceptanceId()
    {
        var trading = new RecordingNormalizedTradingApi();
        var api = new BittradeTradingApi(trading);

        var key = new OrderKey(OrderIdKind.AcceptanceId, "1002");
        var call = await api.CancelOrderCallAsync(new CommonSymbol("BTC/JPY"), key);
        var ok = Assert.IsType<CallResult<CancelResult>.Ok>(call.Result);
        var result = ok.Response;

        Assert.True(result.IsSuccess);
        Assert.Equal("1002", trading.LastOrderKey?.Value);
    }

    private static BittradeOrderStatus CreateOrderStatus(string id) =>
        new(
            ProductCode: "BTC_JPY",
            Key: new OrderKey(OrderIdKind.AcceptanceId, id),
            Status: OrderState.Completed,
            ExecutedSize: new Size(0.01m),
            OutstandingSize: new Size(0m),
            Price: new Price(100m),
            AveragePrice: new Price(100m));

    private sealed class RecordingNormalizedTradingApi : IBittradeNormalizedTradingApi
    {
        public OrderKey? LastOrderKey { get; private set; }
        public Symbol? LastSymbol { get; private set; }
        public BittradeOrderStatus Order { get; init; } = CreateOrderStatus("default");

        public Task<Call<PlaceOrderRequest, BittradeOrderResult>> PlaceOrderCallAsync(
            BittradeOrderRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(new PlaceOrderRequest(request), new BittradeOrderResult(new OrderKey(OrderIdKind.AcceptanceId, "dummy"), AcceptanceId: "dummy")));

        public Task<Call<GetOrdersRequest, IReadOnlyList<BittradeOrderSummaryNormalized>>> GetOrdersCallAsync(
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(
                new GetOrdersRequest(),
                (IReadOnlyList<BittradeOrderSummaryNormalized>)Array.Empty<BittradeOrderSummaryNormalized>()));

        public Task<Call<CancelOrderRequest, BittradeCancelResult>> CancelOrderCallAsync(
            Symbol symbol,
            OrderKey orderKey,
            CancellationToken ct = default)
        {
            LastSymbol = symbol;
            LastOrderKey = orderKey;
            return Task.FromResult(MakeOkCall(new CancelOrderRequest(symbol, orderKey), new BittradeCancelResult(true)));
        }

        public Task<Call<GetOpenOrdersRequest, IReadOnlyList<BittradeOpenOrder>>> GetOpenOrdersCallAsync(
            Symbol symbol,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(new GetOpenOrdersRequest(symbol), (IReadOnlyList<BittradeOpenOrder>)Array.Empty<BittradeOpenOrder>()));

        public Task<Call<GetOrderRequest, BittradeOrderStatus>> GetOrdersByOrderIdCallAsync(
            Symbol symbol,
            OrderKey orderKey,
            CancellationToken ct = default)
        {
            LastSymbol = symbol;
            LastOrderKey = orderKey;
            return Task.FromResult(MakeOkCall(new GetOrderRequest(symbol, orderKey), Order with { Key = orderKey }));
        }

        public Task<Call<GetAccountExecutionsRequest, IReadOnlyList<BittradeExecutionNormalized>>> GetMatchResultsCallAsync(
            Symbol symbol,
            int? limit = null,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(
                new GetAccountExecutionsRequest(symbol, limit),
                (IReadOnlyList<BittradeExecutionNormalized>)Array.Empty<BittradeExecutionNormalized>()));

        public Task<Call<GetRetailOrderListRequest, IReadOnlyList<BittradeRetailOrderEntryNormalized>>> GetRetailOrderListCallAsync(
            GetRetailOrderListRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(
                request,
                (IReadOnlyList<BittradeRetailOrderEntryNormalized>)Array.Empty<BittradeRetailOrderEntryNormalized>()));

        public Task<Call<GetRetailOrderDetailByOrderIdRequest, BittradeRetailOrderEntryNormalized?>> GetRetailOrderDetailByOrderIdCallAsync(
            GetRetailOrderDetailByOrderIdRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall<GetRetailOrderDetailByOrderIdRequest, BittradeRetailOrderEntryNormalized?>(request, null));

        public Task<Call<PostRetailOrderHistoryRequest, IReadOnlyList<BittradeRetailOrderEntryNormalized>>> PostRetailOrderHistoryCallAsync(
            PostRetailOrderHistoryRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(
                request,
                (IReadOnlyList<BittradeRetailOrderEntryNormalized>)Array.Empty<BittradeRetailOrderEntryNormalized>()));

        public Task<Call<PostRetailOrderDetailRequest, BittradeRetailOrderEntryNormalized?>> PostRetailOrderDetailCallAsync(
            PostRetailOrderDetailRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall<PostRetailOrderDetailRequest, BittradeRetailOrderEntryNormalized?>(request, null));

        public Task<Call<PostRetailOrderCreateRequest, BittradeRetailOrderResult>> PostRetailOrderCreateCallAsync(
            PostRetailOrderCreateRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(request, new BittradeRetailOrderResult(0, null, true, null)));

        public Task<Call<PostRetailOrderCancelByOrderIdRequest, BittradeRetailOrderResult>> PostRetailOrderCancelByOrderIdCallAsync(
            PostRetailOrderCancelByOrderIdRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(request, new BittradeRetailOrderResult(0, null, true, null)));

        public Task<Call<PostWithdrawVirtualByAddressIdCreateRequest, BittradeWithdrawResult>> PostWithdrawVirtualByAddressIdCreateCallAsync(
            PostWithdrawVirtualByAddressIdCreateRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(request, new BittradeWithdrawResult("ok", null)));

        public Task<Call<PostWithdrawVirtualByWithdrawIdPlaceRequest, BittradeWithdrawResult>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
            PostWithdrawVirtualByWithdrawIdPlaceRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(request, new BittradeWithdrawResult("ok", null)));

        public Task<Call<PostWithdrawVirtualByWithdrawIdCancelRequest, BittradeWithdrawResult>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
            PostWithdrawVirtualByWithdrawIdCancelRequest request,
            CancellationToken ct = default) =>
            Task.FromResult(MakeOkCall(request, new BittradeWithdrawResult("ok", null)));

        private static Call<TReq, TResponse> MakeOkCall<TReq, TResponse>(TReq request, TResponse response)
        {
            var meta = CallMeta.CreateInternal("Normalized", "RecordingNormalizedTradingApi");
            return new Call<TReq, TResponse>(
                Id: CallId.New(),
                StartedAt: DateTimeOffset.UtcNow,
                Duration: TimeSpan.Zero,
                Request: request,
                Result: new CallResult<TResponse>.Ok(response),
                Meta: meta);
        }
    }
}
