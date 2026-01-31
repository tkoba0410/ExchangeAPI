using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.Adapter.Public.Api;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Api;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public class BitflyerExchangeInfoApi_Tests
{
    [Fact]
    public async Task GetExchangeInfo_ReturnsMappedMarkets()
    {
        var rawTicker = new RawPublicDtos.Ticker { ProductCode = "BTC_JPY" };
        var rawApi = new FakeBitflyerPublicApi(rawTicker);
        var normalized = new BitflyerNormalizedPublicApi(rawApi);
        var api = new BitflyerExchangeInfoApi(normalized);

        var call = await api.GetExchangeInfoCallAsync();
        var ok = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Contracts.Common.Dtos.ExchangeInfo.ExchangeInfo>.Ok>(call.Result);
        var info = ok.Response;

        Assert.Contains(info.Markets, market => market.Symbol == "BTC/JPY" && market.ProductCode == "BTC_JPY");
    }
}
