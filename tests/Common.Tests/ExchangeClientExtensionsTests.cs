using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Contracts.Common.DomainCommon.Enums;
using ExchangeApi.Contracts.Common.DomainCommon.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Dtos.Account;
using ExchangeApi.Contracts.Dtos.Common;
using ExchangeApi.Contracts.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Dtos.Market;
using ExchangeApi.Contracts.Dtos.Trading;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Contracts.Errors;
using ExchangeApi.Contracts.Extensions;
using ExchangeApi.Contracts.Common.CallCommon;

namespace ExchangeApi.Tests.Common.Tests;

public sealed class ExchangeClientExtensionsTests
{
    private sealed class DummyMarketApi : IMarketDataApi
    {
        public Task<Call<GetTickerRequest, Ticker>> GetTickerCallAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Call<GetOrderBookRequest, OrderBook>> GetOrderBookCallAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Call<GetMarketExecutionsRequest, IReadOnlyList<ExecutionMarket>>> GetMarketExecutionsCallAsync(
            Symbol symbol,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class DummyTradingApi : ITradingApi
    {
        public Task<Call<PlaceLimitOrderRequest, OrderResult>> PlaceLimitOrderCallAsync(
            Symbol symbol,
            Side side,
            Size size,
            Price price,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Call<PlaceMarketOrderRequest, OrderResult>> PlaceMarketOrderCallAsync(
            Symbol symbol,
            Side side,
            Size size,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Call<CancelOrderRequest, CancelResult>> CancelOrderCallAsync(
            Symbol symbol,
            OrderKey orderKey,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Call<GetOrderRequest, OrderStatus>> GetOrderCallAsync(
            Symbol symbol,
            OrderKey orderKey,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Call<GetOpenOrdersRequest, IReadOnlyList<OrderSnapshotItem>>> GetOpenOrdersCallAsync(
            Symbol symbol,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class DummyAccountApi : IAccountApi
    {
        public Task<Call<GetBalancesRequest, IReadOnlyList<Balance>>> GetBalancesCallAsync(
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class DummyHistoryApi : ISpotHistoryApi
    {
        public Task<Call<MarketLimitCursorRequest, Page<OrderSnapshotItem>>> GetOrdersCallAsync(
            MarketLimitCursorRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Call<MarketLimitCursorRequest, Page<ExecutionItem>>> GetExecutionsCallAsync(
            MarketLimitCursorRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class DummyClient : IExchangeClient
    {
        public IMarketDataApi Market { get; } = new DummyMarketApi();
        public ITradingApi Trading { get; } = new DummyTradingApi();
        public IAccountApi Account { get; } = new DummyAccountApi();
        public ISpotHistoryApi History { get; } = new DummyHistoryApi();
        public ExchangeCode ExchangeCode => ExchangeCode.Bitflyer;
    }

    [Fact]
    public void Raw_Throws_When_NotSupported()
    {
        var client = new DummyClient();
        Assert.Throws<ExchangeFeatureNotSupportedException>(() => client.Raw<object>());
    }
}
