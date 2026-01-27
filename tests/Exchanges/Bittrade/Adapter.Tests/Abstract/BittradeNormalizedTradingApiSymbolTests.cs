using System;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Api;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Dtos.Trading;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Markets;
using ExchangeApi.Exchanges.Bittrade.Normalized.Private.Requests;
using ExchangeApi.Exchanges.Bittrade.Raw.Api;
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

    private static BittradeNormalizedTradingApi CreateApi(string productCode, IBittradeRawApi? raw = null)
    {
        raw ??= new ThrowingRawApi();
        var markets = new StubMarketResolver(productCode);
        return new BittradeNormalizedTradingApi(raw, markets, accountId: "account");
    }

    private abstract class RawApiBase : IBittradeRawApi
    {
        protected static Exception CreateException() => new InvalidOperationException("Raw API should not be called.");

        public virtual Task<Call<RawPublicModels.GetMergedTickerRequest, RawPublicModels.RawMergedResponse>> GetDetailMergedCallAsync(
            RawPublicModels.GetMergedTickerRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPublicModels.GetDepthRequest, RawPublicModels.RawDepthResponse>> GetDepthCallAsync(
            RawPublicModels.GetDepthRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPublicModels.GetTradesRequest, RawPublicModels.RawTradeResponse>> GetTradeCallAsync(
            RawPublicModels.GetTradesRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPublicModels.GetSymbolsRequest, RawPublicModels.RawSymbolsResponse>> GetSymbolsCallAsync(
            RawPublicModels.GetSymbolsRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPublicModels.GetCurrenciesRequest, RawPublicModels.RawCurrenciesResponse>> GetCurrencysCallAsync(
            RawPublicModels.GetCurrenciesRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPublicModels.GetTimestampRequest, RawPublicModels.RawTimestampResponse>> GetTimestampCallAsync(
            RawPublicModels.GetTimestampRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPublicModels.GetKlinesRequest, RawPublicModels.RawKlinesResponse>> GetHistoryKlineCallAsync(
            RawPublicModels.GetKlinesRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPublicModels.GetTickersRequest, RawPublicModels.RawTickersResponse>> GetTickersCallAsync(
            RawPublicModels.GetTickersRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPublicModels.GetTradeHistoryRequest, RawPublicModels.RawTradeHistoryResponse>> GetHistoryTradeCallAsync(
            RawPublicModels.GetTradeHistoryRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.GetAccountsRequest, RawPrivateModels.RawAccountsResponse>> GetAccountsCallAsync(
            RawPrivateModels.GetAccountsRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.GetAccountBalanceRequest, RawPrivateModels.RawBalancesResponse>> GetAccountsBalanceByAccountIdCallAsync(
            RawPrivateModels.GetAccountBalanceRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.GetOpenOrdersRequest, RawPrivateModels.RawOpenOrdersResponse>> GetOpenOrdersCallAsync(
            RawPrivateModels.GetOpenOrdersRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.GetOrdersRequest, RawPrivateModels.RawOrdersResponse>> GetOrdersCallAsync(
            RawPrivateModels.GetOrdersRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.GetOrderRequest, RawPrivateModels.RawOrderDetailResponse>> GetOrdersByOrderIdCallAsync(
            RawPrivateModels.GetOrderRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.GetOrderMatchResultsRequest, RawPrivateModels.RawOrderMatchResultsResponse>> GetOrdersMatchResultsByOrderIdCallAsync(
            RawPrivateModels.GetOrderMatchResultsRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.GetMatchResultsRequest, RawPrivateModels.RawMatchResultsResponse>> GetMatchResultsCallAsync(
            RawPrivateModels.GetMatchResultsRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.GetDepositWithdrawsRequest, RawPrivateModels.RawDepositWithdrawsResponse>> GetDepositWithdrawCallAsync(
            RawPrivateModels.GetDepositWithdrawsRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.GetWithdrawVirtualAddressesRequest, RawPrivateModels.RawWithdrawVirtualAddressesResponse>> GetWithdrawVirtualAddressesCallAsync(
            RawPrivateModels.GetWithdrawVirtualAddressesRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.GetRetailOrdersRequest, RawPrivateModels.RawRetailOrdersResponse>> GetRetailOrderListCallAsync(
            RawPrivateModels.GetRetailOrdersRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.GetRetailOrderDetailByOrderIdRequest, RawPrivateModels.RawRetailOrderDetailResponse>> GetRetailOrderDetailByOrderIdCallAsync(
            RawPrivateModels.GetRetailOrderDetailByOrderIdRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.GetRetailAccountBalanceRequest, RawPrivateModels.RawRetailAccountBalanceResponse>> GetRetailAccountBalanceCallAsync(
            RawPrivateModels.GetRetailAccountBalanceRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.CreateOrderRequest, RawPrivateModels.RawPlaceOrderResponse>> PostOrdersPlaceCallAsync(
            RawPrivateModels.CreateOrderRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.CancelOrderRequest, RawPrivateModels.RawCancelOrderResponse>> PostOrdersSubmitCancelByOrderIdCallAsync(
            RawPrivateModels.CancelOrderRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.CancelOrdersRequest, RawPrivateModels.RawCancelOrdersResponse>> PostOrdersBatchCancelCallAsync(
            RawPrivateModels.CancelOrdersRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.CancelOpenOrdersRequest, RawPrivateModels.RawCancelOpenOrdersResponse>> PostOrdersBatchCancelOpenOrdersCallAsync(
            RawPrivateModels.CancelOpenOrdersRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.CreateWithdrawRequest, RawPrivateModels.RawCreateWithdrawResponse>> PostWithdrawApiCreateCallAsync(
            RawPrivateModels.CreateWithdrawRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.CreateWithdrawVirtualByAddressIdRequest, RawPrivateModels.RawCreateWithdrawResponse>> PostWithdrawVirtualByAddressIdCreateCallAsync(
            RawPrivateModels.CreateWithdrawVirtualByAddressIdRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.CancelWithdrawRequest, RawPrivateModels.RawCancelWithdrawResponse>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
            RawPrivateModels.CancelWithdrawRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.PlaceWithdrawVirtualRequest, RawPrivateModels.RawCreateWithdrawResponse>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
            RawPrivateModels.PlaceWithdrawVirtualRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.CreateRetailOrderRequest, RawPrivateModels.RawRetailOrderResponse>> PostRetailOrderPlaceCallAsync(
            RawPrivateModels.CreateRetailOrderRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.CancelRetailOrderRequest, RawPrivateModels.RawRetailOrderResponse>> PostRetailOrderCancelByOrderIdCallAsync(
            RawPrivateModels.CancelRetailOrderRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.PostRetailOrderHistoryRequest, RawPrivateModels.RawRetailOrdersResponse>> PostRetailOrderHistoryCallAsync(
            RawPrivateModels.PostRetailOrderHistoryRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.PostRetailOrderDetailRequest, RawPrivateModels.RawRetailOrderDetailResponse>> PostRetailOrderDetailCallAsync(
            RawPrivateModels.PostRetailOrderDetailRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateModels.CreateRetailOrderRequest, RawPrivateModels.RawRetailOrderResponse>> PostRetailOrderCreateCallAsync(
            RawPrivateModels.CreateRetailOrderRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();
    }

    private sealed class ThrowingRawApi : RawApiBase
    {
    }

    private sealed class RecordingRawTradingApi : RawApiBase
    {
        public bool WasCalled { get; private set; }

        public override Task<Call<RawPrivateModels.CreateOrderRequest, RawPrivateModels.RawPlaceOrderResponse>> PostOrdersPlaceCallAsync(
            RawPrivateModels.CreateOrderRequest request,
            CancellationToken cancellationToken = default)
        {
            WasCalled = true;
            var meta = CallMeta.CreateInternal("Tests", "RecordingRawTradingApi");
            return Task.FromResult(new Call<RawPrivateModels.CreateOrderRequest, RawPrivateModels.RawPlaceOrderResponse>(
                Id: CallId.New(),
                StartedAt: DateTimeOffset.UtcNow,
                Duration: TimeSpan.Zero,
                Request: request,
                Result: new CallResult<RawPrivateModels.RawPlaceOrderResponse>.Err(
                    new CallError(CallErrorKind.Unknown, "raw-error")),
                Meta: meta));
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
