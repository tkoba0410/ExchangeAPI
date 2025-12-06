using System.Threading.Tasks;
using ExchangeApi.Adapter.Bitflyer;
using ExchangeApi.Contracts.Errors;
using Xunit;

namespace ExchangeApi.Adapter.Bitflyer.Tests;

public class BitflyerRealtimeClient_Tests
{
    [Fact]
    public void SubscribeTicker_NotSupported()
    {
        var client = new BitflyerRealtimeClient();
        Assert.Throws<ExchangeApiException>(() => client.SubscribeTickerAsync("BTC/JPY"));
    }

    [Fact]
    public void SubscribeOrderBook_NotSupported()
    {
        var client = new BitflyerRealtimeClient();
        Assert.Throws<ExchangeApiException>(() => client.SubscribeOrderBookAsync("BTC/JPY"));
    }

    [Fact]
    public void SubscribeExecutions_NotSupported()
    {
        var client = new BitflyerRealtimeClient();
        Assert.Throws<ExchangeApiException>(() => client.SubscribeExecutionsAsync("BTC/JPY"));
    }
}
