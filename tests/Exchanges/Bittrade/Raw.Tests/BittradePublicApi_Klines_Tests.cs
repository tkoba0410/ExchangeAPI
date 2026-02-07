using ExchangeApi.Exchanges.Bittrade.Wire.Public.Endpoints;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bittrade.Raw.Tests;

public sealed class PublicApi_Klines_Tests
{
    [Fact]
    public void GetHistoryKline_UsesExpectedPath()
    {
        var request = PublicEndpoints.GetHistoryKline("btcjpy", "1day", size: "2");

        Assert.Equal("/market/history/kline", request.Path);
        Assert.Equal("period=1day&symbol=btcjpy&size=2", request.Query);
    }
}
