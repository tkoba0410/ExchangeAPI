using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Markets;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;
using ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Helpers;
using ExchangeApi.Primitives.CallCommon;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Adapter.Tests.Abstract;

public sealed class NormalizedTradingApiSymbolTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("btc-jpy")]
    public async Task TryGetApiSymbol_invalid_product_code_returns_error(string productCode)
    {
        var api = CreateApi(productCode);
        var request = new PostOrdersPlaceRequest(
            new OrderRequest(new Symbol("BTC/JPY"), Side.Buy, OrderType.Market, new Size(1m)));

        var call = await api.PostOrdersPlaceCallAsync(request, CancellationToken.None);

        var err = Assert.IsType<CallResult<PostOrdersPlaceResponse>.Err>(call.Result);
        Assert.NotNull(err.Error);
    }

    [Fact]
    public async Task TryGetApiSymbol_normalizes_product_code_and_invokes_raw()
    {
        var raw = new RecordingRawTradingApi();
        var api = CreateApi("BTC_JPY", raw);
        var request = new PostOrdersPlaceRequest(
            new OrderRequest(new Symbol("BTC/JPY"), Side.Buy, OrderType.Market, new Size(1m)));

        await api.PostOrdersPlaceCallAsync(request, CancellationToken.None);

        Assert.True(raw.WasCalled);
    }

    private static NormalizedPrivateApi CreateApi(string productCode, RawApiStub? raw = null)
    {
        raw ??= new ThrowingRawApi();
        var markets = new StubMarketResolver(productCode);
        return new NormalizedPrivateApi(raw, markets, accountId: new FreeText("account"));
    }

    private sealed class ThrowingRawApi : RawApiStub
    {
    }

    private sealed class RecordingRawTradingApi : RawApiStub
    {
        public bool WasCalled { get; private set; }

        public override Task<Call<RawPrivateRequests.PostOrdersPlaceRequest, RawPrivateDtos.PostOrdersPlaceResponse>> PostOrdersPlaceCallAsync(
            RawPrivateRequests.PostOrdersPlaceRequest request,
            CancellationToken cancellationToken = default)
        {
            WasCalled = true;
            var meta = CallMeta.CreateInternal("Tests", "RecordingRawTradingApi");
            return Task.FromResult(new Call<RawPrivateRequests.PostOrdersPlaceRequest, RawPrivateDtos.PostOrdersPlaceResponse>(
                Id: CallId.New(),
                StartedAt: DateTimeOffset.UtcNow,
                Duration: TimeSpan.Zero,
                Request: request,
                Result: new CallResult<RawPrivateDtos.PostOrdersPlaceResponse>.Err(
                    new CallError(CallErrorKind.Unknown, "raw-error")),
                Meta: meta));
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
            CancellationToken cancellationToken = default)
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
