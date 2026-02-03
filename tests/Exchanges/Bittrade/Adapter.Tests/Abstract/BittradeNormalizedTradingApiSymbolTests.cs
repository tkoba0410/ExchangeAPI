using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Markets;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Private.Requests;
using ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Helpers;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public sealed class BittradeNormalizedTradingApiSymbolTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("btc-jpy")]
    public async Task TryGetApiSymbol_invalid_product_code_returns_error(string productCode)
    {
        var api = CreateApi(productCode);
        var request = new PostOrdersPlaceRequest(
            new BittradeOrderRequest(new Symbol("BTC/JPY"), Side.Buy, OrderType.Market, new Size(1m)));

        var call = await api.PostOrdersPlaceCallAsync(request, CancellationToken.None);

        var err = Assert.IsType<CallResult<BittradeOrderResult>.Err>(call.Result);
        Assert.NotNull(err.Error);
    }

    [Fact]
    public async Task TryGetApiSymbol_normalizes_product_code_and_invokes_raw()
    {
        var raw = new RecordingRawTradingApi();
        var api = CreateApi("BTC_JPY", raw);
        var request = new PostOrdersPlaceRequest(
            new BittradeOrderRequest(new Symbol("BTC/JPY"), Side.Buy, OrderType.Market, new Size(1m)));

        await api.PostOrdersPlaceCallAsync(request, CancellationToken.None);

        Assert.True(raw.WasCalled);
    }

    private static BittradeNormalizedPrivateApi CreateApi(string productCode, BittradeRawApiStub? raw = null)
    {
        raw ??= new ThrowingRawApi();
        var markets = new StubMarketResolver(productCode);
        return new BittradeNormalizedPrivateApi(raw, markets, accountId: new FreeText("account"));
    }

    private sealed class ThrowingRawApi : BittradeRawApiStub
    {
    }

    private sealed class RecordingRawTradingApi : BittradeRawApiStub
    {
        public bool WasCalled { get; private set; }

        public override Task<Call<RawPrivateRequests.CreateOrderRequest, RawPrivateDtos.RawPlaceOrderResponse>> PostOrdersPlaceCallAsync(
            RawPrivateRequests.CreateOrderRequest request,
            CancellationToken cancellationToken = default)
        {
            WasCalled = true;
            var meta = CallMeta.CreateInternal("Tests", "RecordingRawTradingApi");
            return Task.FromResult(new Call<RawPrivateRequests.CreateOrderRequest, RawPrivateDtos.RawPlaceOrderResponse>(
                Id: CallId.New(),
                StartedAt: DateTimeOffset.UtcNow,
                Duration: TimeSpan.Zero,
                Request: request,
                Result: new CallResult<RawPrivateDtos.RawPlaceOrderResponse>.Err(
                    new CallError(CallErrorKind.Unknown, "raw-error")),
                Meta: meta));
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
