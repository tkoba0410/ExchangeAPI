using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bittrade.Adapter.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Markets;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Helpers;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public sealed class SpotHistoryApiTests
{
    [Fact]
    public async Task GetOrdersAsync_LimitApplied()
    {
        var raw = new StubRawApi();
        var api = CreateApi(raw);

        var call = await api.GetOrdersAsync(new OrdersRequest(new Symbol("BTC/JPY"), Limit: 1));
        var ok = Assert.IsType<CallResult<OrdersResponse>.Ok>(call.Result);

        Assert.Single(ok.Response.Items);
        Assert.Equal(1, ok.Response.AppliedLimit);
        Assert.Equal(1, ok.Response.ReturnedCount);
    }

    [Fact]
    public async Task GetExecutionsAsync_LimitApplied()
    {
        var raw = new StubRawApi();
        var api = CreateApi(raw);

        var call = await api.GetExecutionsPrivateAsync(new ExecutionsPrivateRequest(new Symbol("BTC/JPY"), Limit: 1));
        var ok = Assert.IsType<CallResult<ExecutionsPrivateResponse>.Ok>(call.Result);
        var response = ok.Response;

        Assert.Empty(response.Items);
        Assert.Equal(1, response.AppliedLimit);
        Assert.Equal(0, response.ReturnedCount);
    }

    private static SpotHistoryApi CreateApi(StubRawApi raw)
    {
        var markets = new StubMarketResolver("btcjpy");
        var normalized = new NormalizedPrivateApi(raw, markets, accountId: new FreeText("account"));
        return new SpotHistoryApi(normalized);
    }

    private sealed class StubRawApi : RawApiStub
    {
        public override Task<Call<RawPrivateRequests.GetOpenOrdersRequest, RawPrivateDtos.GetOpenOrdersResponse>> GetOpenOrdersCallAsync(
            RawPrivateRequests.GetOpenOrdersRequest request,
            CancellationToken cancellationToken = default)
        {
            var data = new[]
            {
                new RawPrivateDtos.RawOrderSummary(
                    Id: "id-1",
                    Symbol: "btcjpy",
                    AccountId: "account",
                    Amount: "0.1",
                    Price: "100",
                    State: "submitted",
                    Type: "buy-limit",
                    ClientOrderId: null,
                    CreatedAt: DateTimeOffset.UtcNow,
                    FilledAmount: "0"),
                new RawPrivateDtos.RawOrderSummary(
                    Id: "id-2",
                    Symbol: "btcjpy",
                    AccountId: "account",
                    Amount: "0.2",
                    Price: "101",
                    State: "submitted",
                    Type: "sell-limit",
                    ClientOrderId: null,
                    CreatedAt: DateTimeOffset.UtcNow,
                    FilledAmount: "0"),
            };
            var response = new RawPrivateDtos.GetOpenOrdersResponse("ok", data);
            return Task.FromResult(CreateOkCall(request, response));
        }

        public override Task<Call<RawPrivateRequests.GetMatchResultsRequest, RawPrivateDtos.GetMatchResultsResponse>> GetMatchResultsCallAsync(
            RawPrivateRequests.GetMatchResultsRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = new RawPrivateDtos.GetMatchResultsResponse(
                "ok",
                Array.Empty<RawPrivateDtos.RawMatchResultEntry>());
            return Task.FromResult(CreateOkCall(request, response));
        }

        private static Call<TReq, TOk> CreateOkCall<TReq, TOk>(TReq request, TOk ok)
        {
            var meta = CallMeta.CreateInternal("Raw", "StubRawApi");
            return new Call<TReq, TOk>(
                Id: CallId.New(),
                StartedAt: DateTimeOffset.UtcNow,
                Duration: TimeSpan.Zero,
                Request: request,
                Result: new CallResult<TOk>.Ok(ok),
                Meta: meta);
        }
    }

    private sealed class StubMarketResolver : IBittradeMarketResolver
    {
        private readonly MarketInfo _market;

        public StubMarketResolver(string productCode)
        {
            _market = new MarketInfo(new Symbol("BTC/JPY"), ProductCode.Parse(productCode));
        }

        public Task<Call<ResolveBittradeMarketRequest, MarketInfo>> ResolveCallAsync(
            Symbol symbol,
            CancellationToken ct = default)
        {
            var request = new ResolveBittradeMarketRequest(symbol);
            var meta = CallMeta.CreateInternal("Normalized", "StubMarketResolver");

            return Task.FromResult(new Call<ResolveBittradeMarketRequest, MarketInfo>(
                Id: CallId.New(),
                StartedAt: DateTimeOffset.UtcNow,
                Duration: TimeSpan.Zero,
                Request: request,
                Result: new CallResult<MarketInfo>.Ok(_market),
                Meta: meta));
        }
    }
}
