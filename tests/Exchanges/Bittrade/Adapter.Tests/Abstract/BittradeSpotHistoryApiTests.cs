using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Markets;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Types;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Helpers;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public sealed class BittradeSpotHistoryApiTests
{
    [Fact]
    public async Task GetOrdersAsync_LimitApplied()
    {
        var raw = new StubRawApi();
        var api = CreateApi(raw);

        var call = await api.GetOrdersCallAsync(new MarketLimitCursorRequest(new Symbol("BTC/JPY"), Limit: 1));
        var ok = Assert.IsType<CallResult<Page<OrderSnapshotItem>>.Ok>(call.Result);

        Assert.Single(ok.Response.Items);
        Assert.Equal(1, ok.Response.Meta.AppliedLimit);
        Assert.Equal(1, ok.Response.Meta.ReturnedCount);
    }

    [Fact]
    public async Task GetExecutionsAsync_LimitApplied()
    {
        var raw = new StubRawApi();
        var api = CreateApi(raw);

        var call = await api.GetExecutionsCallAsync(new MarketLimitCursorRequest(new Symbol("BTC/JPY"), Limit: 1));
        var resultType = call.Result.GetType();
        var responseProp = resultType.GetProperty("Response");
        if (responseProp is null)
        {
            var errorProp = resultType.GetProperty("Error");
            var error = errorProp?.GetValue(call.Result);
            Assert.Fail($"Expected Ok but got Err: {error}");
        }
        var response = Assert.IsType<Page<ExecutionItem>>(responseProp.GetValue(call.Result));

        Assert.Empty(response.Items);
        Assert.Equal(1, response.Meta.AppliedLimit);
        Assert.Equal(0, response.Meta.ReturnedCount);
    }

    private static BittradeSpotHistoryApi CreateApi(StubRawApi raw)
    {
        var markets = new StubMarketResolver("btcjpy");
        var normalized = new BittradeNormalizedPrivateApi(raw, markets, accountId: "account");
        return new BittradeSpotHistoryApi(normalized);
    }

    private sealed class StubRawApi : BittradeRawApiStub
    {
        public override Task<Call<RawPrivateRequests.GetOpenOrdersRequest, RawPrivateDtos.RawOpenOrdersResponse>> GetOpenOrdersCallAsync(
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
            var response = new RawPrivateDtos.RawOpenOrdersResponse("ok", data);
            return Task.FromResult(CreateOkCall(request, response));
        }

        public override Task<Call<RawPrivateRequests.GetMatchResultsRequest, RawPrivateDtos.RawMatchResultsResponse>> GetMatchResultsCallAsync(
            RawPrivateRequests.GetMatchResultsRequest request,
            CancellationToken cancellationToken = default)
        {
            var response = new RawPrivateDtos.RawMatchResultsResponse(
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
        private readonly BittradeMarketInfo _market;

        public StubMarketResolver(string productCode)
        {
            _market = new BittradeMarketInfo(new Symbol("BTC/JPY"), ProductCode.Parse(productCode));
        }

        public Task<Call<ResolveBittradeMarketRequest, BittradeMarketInfo>> ResolveCallAsync(
            Symbol symbol,
            CancellationToken ct = default)
        {
            var request = new ResolveBittradeMarketRequest(symbol);
            var meta = CallMeta.CreateInternal("Normalized", "StubMarketResolver");

            return Task.FromResult(new Call<ResolveBittradeMarketRequest, BittradeMarketInfo>(
                Id: CallId.New(),
                StartedAt: DateTimeOffset.UtcNow,
                Duration: TimeSpan.Zero,
                Request: request,
                Result: new CallResult<BittradeMarketInfo>.Ok(_market),
                Meta: meta));
        }
    }
}
