using System.Threading.Tasks;
using Exchange.Bitflyer.Abstract;
using Xunit;

namespace Exchange.Bitflyer.Tests;

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
