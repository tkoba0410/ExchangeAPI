using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Primitives.DomainCommon.Enums;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Contracts.Common.Dtos.Account;
using ExchangeApi.Contracts.Common.Dtos.Common;
using ExchangeApi.Contracts.Common.Dtos.ExchangeInfo;
using ExchangeApi.Contracts.Common.Dtos.Market;
using ExchangeApi.Contracts.Common.Dtos.Trading;
using ExchangeApi.Contracts.Facade.Interfaces;
using ExchangeApi.Contracts.Facade.Requests;
using ExchangeApi.Contracts.Common.Errors;
using ExchangeApi.Contracts.Facade.Extensions;
using ExchangeApi.Primitives.CallCommon;

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
