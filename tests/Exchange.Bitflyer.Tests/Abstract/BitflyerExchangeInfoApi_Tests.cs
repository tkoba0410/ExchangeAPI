using System.Threading.Tasks;
using ExchangeApi.Adapter.Bitflyer;
using Xunit;

namespace ExchangeApi.Adapter.Bitflyer.Tests;

public class BitflyerExchangeInfoApi_Tests
{
    [Fact]
    public async Task GetExchangeInfo_ReturnsFeatureFlags()
    {
        var api = new BitflyerExchangeInfoApi();

        var info = await api.GetExchangeInfoAsync();

        Assert.False(info.Features!.SupportsCandlestick);
        Assert.False(info.Features.SupportsOrderBookDelta);
        Assert.False(info.Features.SupportsRealtimeExecutions);
    }
}
