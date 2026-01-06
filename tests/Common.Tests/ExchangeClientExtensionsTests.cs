using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Contracts.Interfaces;
using ExchangeApi.Contracts.Requests;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Core.Extensions;
using ExchangeApi.Spec.CallCommon;

namespace ExchangeApi.Common.Tests;

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

        public Task<Call<PlaceStopOrderRequest, OrderResult>> PlaceStopOrderCallAsync(
            Symbol symbol,
            Side side,
            Size size,
            Price triggerPrice,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Call<CancelOrderRequest, CancelResult>> CancelOrderCallAsync(
            Symbol symbol,
            OrderKey orderKey,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Call<GetOrdersRequest, IReadOnlyList<OpenOrder>>> GetOrdersCallAsync(
            Symbol symbol,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Call<GetOrderRequest, OrderStatus>> GetOrderCallAsync(
            Symbol symbol,
            OrderKey orderKey,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Call<GetParentOrdersRequest, IReadOnlyList<ParentOrder>>> GetParentOrdersCallAsync(
            Symbol symbol,
            string? parentOrderId = null,
            string? parentOrderAcceptanceId = null,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Call<GetParentOrderRequest, ParentOrderDetail>> GetParentOrderCallAsync(
            Symbol symbol,
            string? parentOrderId = null,
            string? parentOrderAcceptanceId = null,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class DummyAccountApi : IAccountApi
    {
        public Task<Call<GetBalancesRequest, IReadOnlyList<Balance>>> GetBalancesCallAsync(
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<Call<GetAccountExecutionsRequest, IReadOnlyList<ExecutionAccount>>> GetAccountExecutionsCallAsync(
            Symbol symbol,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }

    private sealed class DummyExchangeInfoApi : IExchangeInfoApi
    {
        public Task<Call<GetExchangeInfoRequest, ExchangeInfo>> GetExchangeInfoCallAsync(
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
