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

        public virtual Task<Call<RawPublicRequests.GetMergedTickerRequest, RawPublicDtos.RawMergedResponse>> GetDetailMergedCallAsync(
            RawPublicRequests.GetMergedTickerRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPublicRequests.GetDepthRequest, RawPublicDtos.RawDepthResponse>> GetDepthCallAsync(
            RawPublicRequests.GetDepthRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPublicRequests.GetTradesRequest, RawPublicDtos.RawTradeResponse>> GetTradeCallAsync(
            RawPublicRequests.GetTradesRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPublicRequests.GetSymbolsRequest, RawPublicDtos.RawSymbolsResponse>> GetSymbolsCallAsync(
            RawPublicRequests.GetSymbolsRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPublicRequests.GetCurrenciesRequest, RawPublicDtos.RawCurrenciesResponse>> GetCurrencysCallAsync(
            RawPublicRequests.GetCurrenciesRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPublicRequests.GetTimestampRequest, RawPublicDtos.RawTimestampResponse>> GetTimestampCallAsync(
            RawPublicRequests.GetTimestampRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPublicRequests.GetKlinesRequest, RawPublicDtos.RawKlinesResponse>> GetHistoryKlineCallAsync(
            RawPublicRequests.GetKlinesRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPublicRequests.GetTickersRequest, RawPublicDtos.RawTickersResponse>> GetTickersCallAsync(
            RawPublicRequests.GetTickersRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPublicRequests.GetTradeHistoryRequest, RawPublicDtos.RawTradeHistoryResponse>> GetHistoryTradeCallAsync(
            RawPublicRequests.GetTradeHistoryRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.GetAccountsRequest, RawPrivateDtos.RawAccountsResponse>> GetAccountsCallAsync(
            RawPrivateRequests.GetAccountsRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.GetAccountBalanceRequest, RawPrivateDtos.RawBalancesResponse>> GetAccountsBalanceByAccountIdCallAsync(
            RawPrivateRequests.GetAccountBalanceRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.GetOpenOrdersRequest, RawPrivateDtos.RawOpenOrdersResponse>> GetOpenOrdersCallAsync(
            RawPrivateRequests.GetOpenOrdersRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.GetOrdersRequest, RawPrivateDtos.RawOrdersResponse>> GetOrdersCallAsync(
            RawPrivateRequests.GetOrdersRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.GetOrderRequest, RawPrivateDtos.RawOrderDetailResponse>> GetOrdersByOrderIdCallAsync(
            RawPrivateRequests.GetOrderRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.GetOrderMatchResultsRequest, RawPrivateDtos.RawOrderMatchResultsResponse>> GetOrdersMatchResultsByOrderIdCallAsync(
            RawPrivateRequests.GetOrderMatchResultsRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.GetMatchResultsRequest, RawPrivateDtos.RawMatchResultsResponse>> GetMatchResultsCallAsync(
            RawPrivateRequests.GetMatchResultsRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.GetDepositWithdrawsRequest, RawPrivateDtos.RawDepositWithdrawsResponse>> GetDepositWithdrawCallAsync(
            RawPrivateRequests.GetDepositWithdrawsRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.GetWithdrawVirtualAddressesRequest, RawPrivateDtos.RawWithdrawVirtualAddressesResponse>> GetWithdrawVirtualAddressesCallAsync(
            RawPrivateRequests.GetWithdrawVirtualAddressesRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.GetRetailOrdersRequest, RawPrivateDtos.RawRetailOrdersResponse>> GetRetailOrderListCallAsync(
            RawPrivateRequests.GetRetailOrdersRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.GetRetailOrderDetailByOrderIdRequest, RawPrivateDtos.RawRetailOrderDetailResponse>> GetRetailOrderDetailByOrderIdCallAsync(
            RawPrivateRequests.GetRetailOrderDetailByOrderIdRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.GetRetailAccountBalanceRequest, RawPrivateDtos.RawRetailAccountBalanceResponse>> GetRetailAccountBalanceCallAsync(
            RawPrivateRequests.GetRetailAccountBalanceRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.CreateOrderRequest, RawPrivateDtos.RawPlaceOrderResponse>> PostOrdersPlaceCallAsync(
            RawPrivateRequests.CreateOrderRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.CancelOrderRequest, RawPrivateDtos.RawCancelOrderResponse>> PostOrdersSubmitCancelByOrderIdCallAsync(
            RawPrivateRequests.CancelOrderRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.CancelOrdersRequest, RawPrivateDtos.RawCancelOrdersResponse>> PostOrdersBatchCancelCallAsync(
            RawPrivateRequests.CancelOrdersRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.CancelOpenOrdersRequest, RawPrivateDtos.RawCancelOpenOrdersResponse>> PostOrdersBatchCancelOpenOrdersCallAsync(
            RawPrivateRequests.CancelOpenOrdersRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.CreateWithdrawRequest, RawPrivateDtos.RawCreateWithdrawResponse>> PostWithdrawApiCreateCallAsync(
            RawPrivateRequests.CreateWithdrawRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.CreateWithdrawVirtualByAddressIdRequest, RawPrivateDtos.RawCreateWithdrawResponse>> PostWithdrawVirtualByAddressIdCreateCallAsync(
            RawPrivateRequests.CreateWithdrawVirtualByAddressIdRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.CancelWithdrawRequest, RawPrivateDtos.RawCancelWithdrawResponse>> PostWithdrawVirtualByWithdrawIdCancelCallAsync(
            RawPrivateRequests.CancelWithdrawRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.PlaceWithdrawVirtualRequest, RawPrivateDtos.RawCreateWithdrawResponse>> PostWithdrawVirtualByWithdrawIdPlaceCallAsync(
            RawPrivateRequests.PlaceWithdrawVirtualRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.CreateRetailOrderRequest, RawPrivateDtos.RawRetailOrderResponse>> PostRetailOrderPlaceCallAsync(
            RawPrivateRequests.CreateRetailOrderRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.CancelRetailOrderRequest, RawPrivateDtos.RawRetailOrderResponse>> PostRetailOrderCancelByOrderIdCallAsync(
            RawPrivateRequests.CancelRetailOrderRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.PostRetailOrderHistoryRequest, RawPrivateDtos.RawRetailOrdersResponse>> PostRetailOrderHistoryCallAsync(
            RawPrivateRequests.PostRetailOrderHistoryRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.PostRetailOrderDetailRequest, RawPrivateDtos.RawRetailOrderDetailResponse>> PostRetailOrderDetailCallAsync(
            RawPrivateRequests.PostRetailOrderDetailRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();

        public virtual Task<Call<RawPrivateRequests.CreateRetailOrderRequest, RawPrivateDtos.RawRetailOrderResponse>> PostRetailOrderCreateCallAsync(
            RawPrivateRequests.CreateRetailOrderRequest request,
            CancellationToken cancellationToken = default) =>
            throw CreateException();
    }

    private sealed class ThrowingRawApi : RawApiBase
    {
    }

    private sealed class RecordingRawTradingApi : RawApiBase
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
