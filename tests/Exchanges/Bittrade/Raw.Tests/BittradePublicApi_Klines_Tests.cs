using ExchangeApi.Exchanges.Bittrade.Wire.Endpoints;
using Xunit;

namespace ExchangeApi.Exchanges.Bittrade.Tests;

public sealed class BittradePublicApi_Klines_Tests
{
    [Fact]
    public void GetKlinesAsync_UsesExpectedPath()
    {
        var request = BittradeEndpoints.GetKlines("BTC/JPY", "1day", size: 2);

        Assert.Equal("market/history/kline", request.Path);
        Assert.Equal("period=1day&symbol=btcjpy&size=2", request.Query);
    }
}
