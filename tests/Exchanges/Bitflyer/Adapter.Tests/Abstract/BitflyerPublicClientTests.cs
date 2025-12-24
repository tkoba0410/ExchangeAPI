using System;
using System.Threading.Tasks;
using ExchangeApi.Common.Types;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Facade;
using ExchangeApi.Exchanges.Bitflyer.Raw.PublicGet;
using ExchangeApi.Exchanges.Bitflyer.Tests.Fakes;
using ExchangeApi.Core.Contracts.Errors;
using RawProductCode = ExchangeApi.Exchanges.Bitflyer.Raw.Types.RawProductCode;
using Xunit;

namespace ExchangeApi.Exchanges.Bitflyer.Tests;

public sealed class BitflyerPublicClientTests
{
    [Fact]
    public async Task GetHealthAsync_ReturnsRawHealth()
    {
        var rawTicker = new Ticker { ProductCode = new RawProductCode("BTC_JPY") };
        var publicApi = new FakeBitflyerPublicApi(rawTicker);
        var client = new BitflyerPublicClient(publicApi);

        var result = await client.GetHealthAsync(new Symbol("BTC/JPY"));

        Assert.Equal("NORMAL", result.Status);
    }

    [Fact]
    public async Task GetBoardStateAsync_ReturnsRawBoardState()
    {
        var rawTicker = new Ticker { ProductCode = new RawProductCode("BTC_JPY") };
        var publicApi = new FakeBitflyerPublicApi(rawTicker);
        var client = new BitflyerPublicClient(publicApi);

        var result = await client.GetBoardStateAsync(new Symbol("BTC/JPY"));

        Assert.Equal("NORMAL", result.Health);
        Assert.Equal("RUNNING", result.State);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetTickerAsync_UnknownSymbol_Throws()
    {
        var rawTicker = new Ticker { ProductCode = new RawProductCode("BTC_JPY") };
        var publicApi = new FakeBitflyerPublicApi(rawTicker);
        var client = new BitflyerPublicClient(publicApi);

        await Assert.ThrowsAsync<SymbolNotSupportedException>(() =>
            client.GetTickerAsync(new Symbol("ETH/JPY")));
    }
}
