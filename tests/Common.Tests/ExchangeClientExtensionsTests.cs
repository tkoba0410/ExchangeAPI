using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Call;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Extensions;

namespace ExchangeApi.Common.Tests;

public sealed class ExchangeClientExtensionsTests
{
    private sealed class DummyMarketApi : IMarketDataApi
    {
        public Task<Ticker> GetTickerAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<OrderBook> GetOrderBookAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<ExecutionMarket>> GetMarketExecutionsAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<Candlestick>> GetCandlesticksAsync(
            Symbol symbol,
            TimeSpan timescale,
            DateTimeOffset? from = null,
            DateTimeOffset? to = null,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ApiCall<GetTickerRequest, Ticker, ApiError>> GetTickerCallAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ApiCall<GetOrderBookRequest, OrderBook, ApiError>> GetOrderBookCallAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ApiCall<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>, ApiError>> GetMarketExecutionsCallAsync(
            Symbol symbol,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class DummyTradingApi : ITradingApi
    {
        public Task<OrderResult> PlaceLimitOrderAsync(
            Symbol symbol,
            Side side,
            Size size,
            Price price,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<OrderResult> PlaceMarketOrderAsync(
            Symbol symbol,
            Side side,
            Size size,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<OrderResult> PlaceStopOrderAsync(
            Symbol symbol,
            Side side,
            Size size,
            Price triggerPrice,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<CancelResult> CancelOrderAsync(
            Symbol symbol,
            OrderKey orderKey,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<OpenOrder>> GetOrdersAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<OrderStatus> GetOrderAsync(
            Symbol symbol,
            OrderKey orderKey,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ApiCall<PlaceLimitOrderRequest, OrderResult, ApiError>> PlaceLimitOrderCallAsync(
            Symbol symbol,
            Side side,
            Size size,
            Price price,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ApiCall<PlaceMarketOrderRequest, OrderResult, ApiError>> PlaceMarketOrderCallAsync(
            Symbol symbol,
            Side side,
            Size size,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ApiCall<PlaceStopOrderRequest, OrderResult, ApiError>> PlaceStopOrderCallAsync(
            Symbol symbol,
            Side side,
            Size size,
            Price triggerPrice,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ApiCall<CancelOrderRequest, CancelResult, ApiError>> CancelOrderCallAsync(
            Symbol symbol,
            OrderKey orderKey,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ApiCall<GetOrdersRequest, IReadOnlyList<OpenOrder>, ApiError>> GetOrdersCallAsync(
            Symbol symbol,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ApiCall<GetOrderRequest, OrderStatus, ApiError>> GetOrderCallAsync(
            Symbol symbol,
            OrderKey orderKey,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class DummyAccountApi : IAccountApi
    {
        public Task<IReadOnlyList<Balance>> GetBalancesAsync(CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<IReadOnlyList<ExecutionAccount>> GetAccountExecutionsAsync(
            Symbol symbol,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ApiCall<GetBalancesRequest, IReadOnlyList<Balance>, ApiError>> GetBalancesCallAsync(
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ApiCall<GetAccountExecutionsRequest, IReadOnlyList<ExecutionAccount>, ApiError>> GetAccountExecutionsCallAsync(
            Symbol symbol,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class DummyExchangeInfoApi : IExchangeInfoApi
    {
        public Task<ExchangeInfo> GetExchangeInfoAsync(CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<ApiCall<GetExchangeInfoRequest, ExchangeInfo, ApiError>> GetExchangeInfoCallAsync(
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class DummyClient : IExchangeClient
    {
        public IMarketDataApi Market { get; } = new DummyMarketApi();
        public ITradingApi Trading { get; } = new DummyTradingApi();
        public IAccountApi Account { get; } = new DummyAccountApi();
        public IExchangeInfoApi Info { get; } = new DummyExchangeInfoApi();
        public ExchangeCode ExchangeCode => ExchangeCode.Bitflyer;
    }

    [Fact]
    public void Raw_Throws_When_NotSupported()
    {
        var client = new DummyClient();
        Assert.Throws<ExchangeFeatureNotSupportedException>(() => client.Raw<object>());
    }
}
