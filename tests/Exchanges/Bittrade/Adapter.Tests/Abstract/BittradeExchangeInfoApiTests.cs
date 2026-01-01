using System.Collections.Generic;
using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bittrade.Adapter.Apis.ExchangeInfo;
using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Types;
using ExchangeApi.Exchanges.Bittrade.Normalize;
using ExchangeApi.Exchanges.Bittrade.Normalize.Apis;
using ExchangeApi.Exchanges.Bittrade.Normalize.Models;
using ExchangeApi.Spec.CallCommon;
using Xunit;

namespace ExchangeApi.Exchanges.Bittrade.Tests;

public class BittradeExchangeInfoApiTests
{
    [Fact]
    public async Task GetExchangeInfoAsync_MapsSymbols()
    {
        var api = new BittradeExchangeInfoApi(new StubNormalizedExchangeInfoApi());

        var info = await api.GetExchangeInfoAsync();

        Assert.Single(info.Markets);
        var m = info.Markets[0];
        Assert.Equal("BTC/JPY", m.Symbol);
        Assert.Equal("btcjpy", m.ProductCode);
        Assert.Equal(new Price(0.01m), m.PriceIncrement);
        Assert.Equal(new Size(0.0001m), m.SizeIncrement);
        Assert.Equal(new Size(0.0001m), m.MinSize);
        Assert.Equal(1000m, m.MinNotional);
        Assert.True(m.IsSupported);
    }

    private sealed class StubNormalizedExchangeInfoApi : IBittradeNormalizedExchangeInfoApi
    {
        private static readonly BittradeNormalizedRequest DefaultRequest =
            new BittradeNormalizedRequest("test", new Dictionary<string, string?>());

        public Task<IReadOnlyList<BittradeSymbolNormalized>> GetSymbolsAsync(System.Threading.CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<BittradeSymbolNormalized>>(new[]
            {
                new BittradeSymbolNormalized(
                    Symbol: "btcjpy",
                    BaseCurrency: "btc",
                    QuoteCurrency: "jpy",
                    PricePrecision: 2,
                    AmountPrecision: 4,
                    MinOrderAmount: 0.0001m,
                    MinOrderValue: 1000m,
                    State: "online")
            });

        public Task<BittradeNormalizedCall<IReadOnlyList<BittradeSymbolNormalized>, System.Text.Json.JsonElement>> GetSymbolsCallAsync(
            System.Threading.CancellationToken ct = default) =>
            Task.FromResult(new BittradeNormalizedCall<IReadOnlyList<BittradeSymbolNormalized>, System.Text.Json.JsonElement>(
                DefaultRequest,
                new Ok<IReadOnlyList<BittradeSymbolNormalized>, System.Text.Json.JsonElement>(new[]
                {
                    new BittradeSymbolNormalized(
                        Symbol: "btcjpy",
                        BaseCurrency: "btc",
                        QuoteCurrency: "jpy",
                        PricePrecision: 2,
                        AmountPrecision: 4,
                        MinOrderAmount: 0.0001m,
                        MinOrderValue: 1000m,
                        State: "online")
                }, 200),
                new CallMeta(System.DateTimeOffset.UtcNow, System.TimeSpan.Zero, null)));
    }
}
