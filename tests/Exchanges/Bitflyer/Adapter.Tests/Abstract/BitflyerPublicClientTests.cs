using System;
using System.Threading.Tasks;
using ExchangeApi.Common.Types;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Facade;
using ExchangeApi.Exchanges.Bitflyer.Wire;
using ExchangeApi.Exchanges.Bitflyer.Tests.Fakes;
using RawProductCode = ExchangeApi.Exchanges.Bitflyer.Wire.ProductCode;
using Xunit;

namespace ExchangeApi.Exchanges.Bitflyer.Tests;

public sealed class BitflyerPublicClientTests
{
    [Fact]
    public async Task GetHealthAsync_ReturnsRawHealth()
    {
        var rawTicker = new Ticker { ProductCode = RawProductCode.BtcJpy };
        var publicApi = new FakeBitflyerPublicApi(rawTicker);
        var client = new BitflyerPublicClient(publicApi);

        var result = await client.GetHealthAsync(new Symbol("BTC/JPY"));

        Assert.Equal("NORMAL", result.Status);
    }

    [Fact]
    public async Task GetBoardStateAsync_ReturnsRawBoardState()
    {
        var rawTicker = new Ticker { ProductCode = RawProductCode.BtcJpy };
        var publicApi = new FakeBitflyerPublicApi(rawTicker);
        var client = new BitflyerPublicClient(publicApi);

        var result = await client.GetBoardStateAsync(new Symbol("BTC/JPY"));

        Assert.Equal("NORMAL", result.Health);
        Assert.Equal("RUNNING", result.State);
        Assert.Null(result.Data);
    }
}
