using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Exchanges.Bittrade.Normalized.Call;
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
        var request = OrderRequest.Market(new Symbol("BTC/JPY"), Side.Buy, new Size(1m));

        var call = await api.PlaceOrderCallAsync(request, CancellationToken.None);

        var err = Assert.IsType<CallResult<OrderResult>.Err>(call.Result);
        Assert.NotNull(err.Error);
    }

    [Fact]
    public async Task TryGetApiSymbol_normalizes_product_code_and_invokes_raw()
    {
        var raw = new RecordingRawTradingApi();
        var api = CreateApi("BTC_JPY", raw);
        var request = OrderRequest.Market(new Symbol("BTC/JPY"), Side.Buy, new Size(1m));

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
            var meta = new CallMeta(
                Layer: "Tests",
                Component: "RecordingRawTradingApi",
                Tags: null,
                Children: null);
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

    private sealed class StubMarketResolver : IExchangeMarketResolver
    {
        private readonly ExchangeMarketInfo _market;

        public StubMarketResolver(string productCode)
        {
            _market = new ExchangeMarketInfo(
                Symbol: "BTC/JPY",
                ProductCode: productCode,
                Type: "Spot");
        }

        public Task<Call<ResolveExchangeMarketRequest, ExchangeMarketInfo>> ResolveCallAsync(Symbol symbol, CancellationToken ct = default)
        {
            var request = new ResolveExchangeMarketRequest(symbol);
            var meta = new CallMeta(
                Layer: "Tests",
                Component: "StubMarketResolver",
                Tags: null,
                Children: null);

            return Task.FromResult(new Call<ResolveExchangeMarketRequest, ExchangeMarketInfo>(
                Id: CallId.New(),
                StartedAt: DateTimeOffset.UtcNow,
                Duration: TimeSpan.Zero,
                Request: request,
                Result: new CallResult<ExchangeMarketInfo>.Ok(_market),
                Meta: meta));
        }
    }
}
