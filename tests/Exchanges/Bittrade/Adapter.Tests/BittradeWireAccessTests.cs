using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ExchangeApi.Common.Dtos;
using ExchangeApi.Common.Extensions;
using ExchangeApi.Common.Interfaces;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bittrade.Adapter.Factory;
using ExchangeApi.Exchanges.Bittrade.Adapter.Facade;
using ExchangeApi.Exchanges.Bittrade.Wire;

namespace ExchangeApi.Exchanges.Bittrade.Adapter.Tests;

public sealed class BittradeWireAccessTests
{
    [Fact]
    public void Wire_NotSupported_Throws()
    {
        var client = new BittradePublicClient(new FakeMarketDataApi(), new FakeExchangeInfoApi());

        var ex = Assert.Throws<ExchangeFeatureNotSupportedException>(() => client.Wire<IBittradeWireApi>());
        Assert.Contains(nameof(IBittradeWireApi), ex.Message);
        Assert.Contains("Bittrade", ex.Message);
    }

    [Fact]
    public void Wire_Supported_CanResolveBundle()
    {
        var client = BittradeClientFactory.CreatePublicClient();
        var wire = client.Wire<IBittradeWireApi>();
        Assert.NotNull(wire);
    }

    private sealed class FakeMarketDataApi : IMarketDataApi
    {
        public Task<Ticker> GetTickerAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<OrderBook> GetOrderBookAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<IReadOnlyList<ExecutionMarket>> GetMarketExecutionsAsync(Symbol symbol, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<IReadOnlyList<Candlestick>> GetCandlesticksAsync(Symbol symbol, TimeSpan timescale, DateTimeOffset? from = null, DateTimeOffset? to = null, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }

    private sealed class FakeExchangeInfoApi : IExchangeInfoApi
    {
        public Task<ExchangeInfo> GetExchangeInfoAsync(CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
