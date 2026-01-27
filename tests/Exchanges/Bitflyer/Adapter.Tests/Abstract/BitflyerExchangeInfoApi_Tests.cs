using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Public.Api;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public class BitflyerExchangeInfoApi_Tests
{
    [Fact]
    public async Task GetExchangeInfo_ReturnsFeatureFlags()
    {
        var api = new BitflyerExchangeInfoApi();

        var call = await api.GetExchangeInfoCallAsync();
        var ok = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Contracts.Common.Dtos.ExchangeInfo.ExchangeInfo>.Ok>(call.Result);
        var info = ok.Response;

        Assert.False(info.Features!.SupportsCandlestick);
        Assert.False(info.Features.SupportsOrderBookDelta);
        Assert.False(info.Features.SupportsRealtimeExecutions);
    }
}
