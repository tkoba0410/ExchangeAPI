using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Exchanges.Bittrade.Api.Adapter.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Markets;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Helpers;
using CommonSymbol = ExchangeApi.Primitives.DomainCommon.Types.Symbol;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public sealed class BittradeOrderKeyConnectivityTests
{
    [Fact]
    public async Task GetOrdersByOrderIdCallAsync_UsesOrderKeyValue_WithAcceptanceId()
    {
        var raw = new RecordingRawApi();
        var api = CreateApi(raw);

        var key = new OrderKey(OrderIdKind.AcceptanceId, "1001");
        var call = await api.GetOrdersByOrderIdCallAsync(new CommonSymbol("BTC/JPY"), key);
        var ok = Assert.IsType<CallResult<OrderStatus>.Ok>(call.Result);
        var status = ok.Response;

        Assert.Equal("1001", raw.LastOrderId);
        Assert.Equal(OrderIdKind.AcceptanceId, status.Key.Kind);
        Assert.Equal("1001", status.Key.Value);
    }

    [Fact]
    public async Task CancelOrderAsync_UsesOrderKeyValue_WithAcceptanceId()
    {
        var raw = new RecordingRawApi();
        var api = CreateApi(raw);

        var key = new OrderKey(OrderIdKind.AcceptanceId, "1002");
        var call = await api.CancelOrderCallAsync(new CommonSymbol("BTC/JPY"), key);
        var ok = Assert.IsType<CallResult<CancelResult>.Ok>(call.Result);
        var result = ok.Response;

        Assert.True(result.IsSuccess);
        Assert.Equal("1002", raw.LastCancelOrderId);
    }

    private static BittradeTradingApi CreateApi(RecordingRawApi raw)
    {
        var markets = new StubMarketResolver("BTC_JPY");
        var normalized = new BittradeNormalizedPrivateApi(raw, markets, accountId: "account");
        return new BittradeTradingApi(normalized);
    }

    private sealed class RecordingRawApi : BittradeRawApiStub
    {
        public string? LastOrderId { get; private set; }
        public string? LastCancelOrderId { get; private set; }

        public override Task<Call<RawPrivateRequests.GetOrderRequest, RawPrivateDtos.RawOrderDetailResponse>> GetOrdersByOrderIdCallAsync(
            RawPrivateRequests.GetOrderRequest request,
            CancellationToken cancellationToken = default)
        {
            LastOrderId = request.OrderId;
            var detail = new RawPrivateDtos.RawOrderDetail(
                Id: request.OrderId,
                Symbol: "btcjpy",
                AccountId: "account",
                Amount: "1",
                Price: "100",
                State: "filled",
                Type: "buy-limit",
                ClientOrderId: null,
                CreatedAt: DateTimeOffset.UtcNow,
                FinishedAt: null,
                FilledAmount: "1",
                FilledCashAmount: "100",
                Fees: "0");
            var response = new RawPrivateDtos.RawOrderDetailResponse("ok", detail);
            return Task.FromResult(CreateOkCall(request, response));
        }

        public override Task<Call<RawPrivateRequests.CancelOrderRequest, RawPrivateDtos.RawCancelOrderResponse>> PostOrdersSubmitCancelByOrderIdCallAsync(
            RawPrivateRequests.CancelOrderRequest request,
            CancellationToken cancellationToken = default)
        {
            LastCancelOrderId = request.OrderId;
            var response = new RawPrivateDtos.RawCancelOrderResponse("ok", request.OrderId);
            return Task.FromResult(CreateOkCall(request, response));
        }

        private static Call<TReq, TOk> CreateOkCall<TReq, TOk>(TReq request, TOk ok)
        {
            var meta = CallMeta.CreateInternal("Raw", "RecordingRawApi");
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
            _market = new BittradeMarketInfo(new Symbol("BTC/JPY"), productCode);
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
