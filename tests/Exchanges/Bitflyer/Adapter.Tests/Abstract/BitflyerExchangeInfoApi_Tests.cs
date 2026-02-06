using System.Threading.Tasks;
using ExchangeApi.Exchanges.Bitflyer.ExchangeInfo.Adapter.Public.Api;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public class BitflyerExchangeInfoApi_Tests
{
    [Fact]
    public async Task GetExchangeInfo_ReturnsMappedMarkets()
    {
        var api = new BitflyerExchangeInfoApi();

        var call = await api.GetExchangeInfoAsync();
        var ok = Assert.IsType<ExchangeApi.Primitives.CallCommon.CallResult<ExchangeApi.Contracts.Common.Dtos.ExchangeInfoResponse>.Ok>(call.Result);
        var info = ok.Response;

        Assert.Contains(info.Markets, market => market.Symbol.Value == "BTC/JPY" && market.ProductCode.Value == "BTC_JPY");
    }
}
