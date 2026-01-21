using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Application.Interfaces;
using ExchangeApi.Application.Trading;
using ExchangeApi.Composition.Adapters.Application;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Api.Facade;
using ExchangeApi.Exchanges.Bitflyer.Raw.Private.Api;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal;
using ExchangeApi.Exchanges.Bitflyer.Raw.Internal.Encoding;
using ExchangeApi.Exchanges.Bitflyer.Raw.Public.Api;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Application.UseCases;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public sealed class BitflyerExchangeClient_PollOrderStatus_Tests
{
    [Fact]
    public async Task WaitForOrderAsync_CompletesWhenStateTransitions()
    {
        var acceptanceId = "ACCEPT-1";
        var active = new RawPrivateModels.RawGetChildOrdersResponse
        {
            ProductCode = "BTC_JPY",
            ChildOrderAcceptanceId = acceptanceId,
            ChildOrderStatusState = "ACTIVE",
            ExecutedSize = 0m,
            OutstandingSize = 0.01m,
            Price = 3000000m,
            AveragePrice = 0m,
            Side = "BUY",
            ChildOrderType = "LIMIT",
            Size = 0.01m,
        };
        var completed = new RawPrivateModels.RawGetChildOrdersResponse
        {
            ProductCode = active.ProductCode,
            ChildOrderAcceptanceId = acceptanceId,
            ChildOrderStatusState = "COMPLETED",
            ExecutedSize = 0.01m,
            OutstandingSize = 0m,
            Price = active.Price,
            AveragePrice = 3000000m,
            Side = active.Side,
            ChildOrderType = active.ChildOrderType,
            Size = active.Size,
        };

        var publicApi = new FakeBitflyerPublicApi(new RawPublicModels.Ticker());
        var sequenceApi = new SequenceChildOrderApi(new[] { active }, new[] { completed });
        var tradingApi = sequenceApi;
        var accountApi = new FakeBitflyerPrivateApi(Array.Empty<RawPrivateModels.BalanceResponse>());
        var client = CreateClient(publicApi, accountApi, tradingApi);

        IOrderQueryApi orderQueryApi = new TradingApiOrderQueryAdapter(client);
        var statusCall = await OrderPolling.WaitForOrderAsync(
            api: orderQueryApi,
            symbol: new Symbol("BTC/JPY"),
            orderKey: new OrderKey(OrderIdKind.AcceptanceId, acceptanceId),
            options: new PollingOptions(TimeSpan.FromMilliseconds(1), 5));

        var status = Assert.IsType<CallResult<OrderStatusSnapshot>.Ok>(statusCall.Result).Response;
        Assert.Equal(OrderState.Completed, status.Status);
        Assert.Equal(0m, status.OutstandingSize.Value);
        Assert.Equal(0.01m, status.ExecutedSize.Value);
        Assert.Equal(3000000m, status.AveragePrice!.Value.Value);
    }

    private static BitflyerExchangeClient CreateClient(
        IBitflyerRawMarketDataApi marketData,
        IBitflyerPrivateApi accountApi,
        IBitflyerRawTradingApi tradingApi)
    {
        var markets = BitflyerTestHelpers.CreateResolver();
        var normalizedMarket = BitflyerTestHelpers.CreateMarketData(marketData);
        var normalizedAccount = BitflyerTestHelpers.CreateAccountApi(accountApi, markets);
        var normalizedTrading = BitflyerTestHelpers.CreateTradingApi(tradingApi, markets);

        return new BitflyerExchangeClient(normalizedMarket, normalizedAccount, normalizedTrading);
    }

    private sealed class SequenceChildOrderApi : IBitflyerRawTradingApi
    {
        private readonly Queue<IReadOnlyList<RawPrivateModels.RawGetChildOrdersResponse>> _queue;

        public SequenceChildOrderApi(params IReadOnlyList<RawPrivateModels.RawGetChildOrdersResponse>[] snapshots)
        {
            _queue = new Queue<IReadOnlyList<RawPrivateModels.RawGetChildOrdersResponse>>(snapshots);
        }

        public Task<Call<string, RawPrivateModels.RawSendChildOrderResponse>> SendChildOrderCallAsync(
            string bodyJson,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OkCall(bodyJson, new RawPrivateModels.RawSendChildOrderResponse()));

        public Task<Call<string, RawPrivateModels.RawSendParentOrderResponse>> SendParentOrderCallAsync(
            string bodyJson,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OkCall(bodyJson, new RawPrivateModels.RawSendParentOrderResponse()));

        public Task<Call<RawPrivateModels.CancelChildOrderRequest, RawPrivateModels.RawCancelChildOrderResponse>> CancelChildOrderCallAsync(
            RawPrivateModels.CancelChildOrderRequest request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OkCall(request, new RawPrivateModels.RawCancelChildOrderResponse()));

        public Task<Call<RawPrivateModels.CancelParentOrderRequest, RawPrivateModels.RawCancelParentOrderResponse>> CancelParentOrderCallAsync(
            RawPrivateModels.CancelParentOrderRequest request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OkCall(request, new RawPrivateModels.RawCancelParentOrderResponse()));

        public Task<Call<RawPrivateModels.GetChildOrdersRequest, IReadOnlyList<RawPrivateModels.RawGetChildOrdersResponse>>> GetChildOrdersCallAsync(
            RawPrivateModels.GetChildOrdersRequest request,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyList<RawPrivateModels.RawGetChildOrdersResponse> snapshot = _queue.Count == 0
                ? Array.Empty<RawPrivateModels.RawGetChildOrdersResponse>()
                : _queue.Dequeue();

            return Task.FromResult(OkCall(request, snapshot));
        }

        public Task<Call<RawPrivateModels.GetParentOrdersRequest, IReadOnlyList<RawPrivateModels.RawGetParentOrdersResponse>>> GetParentOrdersCallAsync(
            RawPrivateModels.GetParentOrdersRequest request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OkCall(request, (IReadOnlyList<RawPrivateModels.RawGetParentOrdersResponse>)Array.Empty<RawPrivateModels.RawGetParentOrdersResponse>()));

        public Task<Call<RawPrivateModels.GetParentOrderRequest, RawPrivateModels.RawGetParentOrderResponse>> GetParentOrderCallAsync(
            RawPrivateModels.GetParentOrderRequest request,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(OkCall(request, new RawPrivateModels.RawGetParentOrderResponse()));

        private static Call<TReq, TResponse> OkCall<TReq, TResponse>(TReq request, TResponse response)
        {
            var meta = CallMeta.CreateInternal("Raw", "SequenceChildOrderApi");
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
