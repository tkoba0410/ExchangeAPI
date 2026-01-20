using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalized.Call;
using ExchangeApi.Exchanges.Bittrade.Normalized.Dtos.Trading;
using ExchangeApi.Exchanges.Bittrade.Normalized.Markets;
using ExchangeApi.Exchanges.Bittrade.Normalized.Requests;
using ExchangeApi.Exchanges.Bittrade.Raw.Private;
using RawRequests = ExchangeApi.Exchanges.Bittrade.Raw.Requests;
using ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;
using ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;
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
        var request = new BittradeOrderRequest(new Symbol("BTC/JPY"), Side.Buy, OrderType.Market, new Size(1m));

        var call = await api.PlaceOrderCallAsync(request, CancellationToken.None);

        var err = Assert.IsType<CallResult<BittradeOrderResult>.Err>(call.Result);
        Assert.NotNull(err.Error);
    }

    [Fact]
    public async Task TryGetApiSymbol_normalizes_product_code_and_invokes_raw()
    {
        var raw = new RecordingRawTradingApi();
        var api = CreateApi("BTC_JPY", raw);
        var request = new BittradeOrderRequest(new Symbol("BTC/JPY"), Side.Buy, OrderType.Market, new Size(1m));

        await api.PlaceOrderCallAsync(request, CancellationToken.None);

        Assert.True(raw.WasCalled);
    }

    private static BittradeNormalizedTradingApi CreateApi(string productCode, IBittradeRawTradingApi? raw = null)
    {
        raw ??= new ThrowingRawTradingApi();
        var markets = new StubMarketResolver(productCode);
        return new BittradeNormalizedTradingApi(raw, markets, accountId: "account");
    }

    private sealed class ThrowingRawTradingApi : IBittradeRawTradingApi
    {
        private static Exception CreateException() => new InvalidOperationException("Raw API should not be called.");

        public Task<Call<RawRequests.CreateOrderRequest, RawPlaceOrderResponse>> CreateOrderAsync(RawRequests.CreateOrderRequest request, CancellationToken cancellationToken = default) =>
            throw CreateException();

        public Task<Call<RawRequests.CancelOrderRequest, RawCancelOrderResponse>> CancelOrderAsync(RawRequests.CancelOrderRequest request, CancellationToken cancellationToken = default) =>
            throw CreateException();

        public Task<Call<RawRequests.CancelOrdersRequest, RawCancelOrdersResponse>> CancelOrdersAsync(RawRequests.CancelOrdersRequest request, CancellationToken cancellationToken = default) =>
            throw CreateException();

        public Task<Call<RawRequests.CancelOpenOrdersRequest, RawCancelOpenOrdersResponse>> CancelOpenOrdersAsync(RawRequests.CancelOpenOrdersRequest request, CancellationToken cancellationToken = default) =>
            throw CreateException();

        public Task<Call<RawRequests.CreateWithdrawRequest, RawCreateWithdrawResponse>> CreateWithdrawAsync(RawRequests.CreateWithdrawRequest request, CancellationToken cancellationToken = default) =>
            throw CreateException();

        public Task<Call<RawRequests.CancelWithdrawRequest, RawCancelWithdrawResponse>> CancelWithdrawAsync(RawRequests.CancelWithdrawRequest request, CancellationToken cancellationToken = default) =>
            throw CreateException();

        public Task<Call<RawRequests.CreateRetailOrderRequest, RawRetailOrderResponse>> CreateRetailOrderAsync(RawRequests.CreateRetailOrderRequest request, CancellationToken cancellationToken = default) =>
            throw CreateException();

        public Task<Call<RawRequests.GetOpenOrdersRequest, RawOpenOrdersResponse>> GetOpenOrdersAsync(RawRequests.GetOpenOrdersRequest request, CancellationToken cancellationToken = default) =>
            throw CreateException();

        public Task<Call<RawRequests.GetOrderRequest, RawOrderDetailResponse>> GetOrderAsync(RawRequests.GetOrderRequest request, CancellationToken cancellationToken = default) =>
            throw CreateException();

        public Task<Call<RawRequests.GetMatchResultsRequest, RawMatchResultsResponse>> GetMatchResultsAsync(RawRequests.GetMatchResultsRequest request, CancellationToken cancellationToken = default) =>
            throw CreateException();
    }

    private sealed class RecordingRawTradingApi : IBittradeRawTradingApi
    {
        private static Exception CreateException() => new InvalidOperationException("Unexpected raw API call.");

        public bool WasCalled { get; private set; }

        public Task<Call<RawRequests.CreateOrderRequest, RawPlaceOrderResponse>> CreateOrderAsync(
            RawRequests.CreateOrderRequest request,
            CancellationToken cancellationToken = default)
        {
            WasCalled = true;
            var meta = CallMeta.CreateInternal("Tests", "RecordingRawTradingApi");
            return Task.FromResult(new Call<RawRequests.CreateOrderRequest, RawPlaceOrderResponse>(
                Id: CallId.New(),
                StartedAt: DateTimeOffset.UtcNow,
                Duration: TimeSpan.Zero,
                Request: request,
                Result: new CallResult<RawPlaceOrderResponse>.Err(
                    new CallError(CallErrorKind.Unknown, "raw-error")),
                Meta: meta));
        }

        public Task<Call<RawRequests.CancelOrderRequest, RawCancelOrderResponse>> CancelOrderAsync(RawRequests.CancelOrderRequest request, CancellationToken cancellationToken = default) =>
            throw CreateException();

        public Task<Call<RawRequests.CancelOrdersRequest, RawCancelOrdersResponse>> CancelOrdersAsync(RawRequests.CancelOrdersRequest request, CancellationToken cancellationToken = default) =>
            throw CreateException();

        public Task<Call<RawRequests.CancelOpenOrdersRequest, RawCancelOpenOrdersResponse>> CancelOpenOrdersAsync(RawRequests.CancelOpenOrdersRequest request, CancellationToken cancellationToken = default) =>
            throw CreateException();

        public Task<Call<RawRequests.CreateWithdrawRequest, RawCreateWithdrawResponse>> CreateWithdrawAsync(RawRequests.CreateWithdrawRequest request, CancellationToken cancellationToken = default) =>
            throw CreateException();

        public Task<Call<RawRequests.CancelWithdrawRequest, RawCancelWithdrawResponse>> CancelWithdrawAsync(RawRequests.CancelWithdrawRequest request, CancellationToken cancellationToken = default) =>
            throw CreateException();

        public Task<Call<RawRequests.CreateRetailOrderRequest, RawRetailOrderResponse>> CreateRetailOrderAsync(RawRequests.CreateRetailOrderRequest request, CancellationToken cancellationToken = default) =>
            throw CreateException();

        public Task<Call<RawRequests.GetOpenOrdersRequest, RawOpenOrdersResponse>> GetOpenOrdersAsync(RawRequests.GetOpenOrdersRequest request, CancellationToken cancellationToken = default) =>
            throw CreateException();

        public Task<Call<RawRequests.GetOrderRequest, RawOrderDetailResponse>> GetOrderAsync(RawRequests.GetOrderRequest request, CancellationToken cancellationToken = default) =>
            throw CreateException();

        public Task<Call<RawRequests.GetMatchResultsRequest, RawMatchResultsResponse>> GetMatchResultsAsync(RawRequests.GetMatchResultsRequest request, CancellationToken cancellationToken = default) =>
            throw CreateException();
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
