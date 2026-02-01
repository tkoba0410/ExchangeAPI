using System;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Private.Dtos;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Fakes;
using ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Helpers;
using ExchangeApi.Primitives.CallCommon;
using Xunit;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Adapter.Tests.Abstract;

public sealed class BitflyerTradingCommissionNormalizedTests
{
    [Fact]
    public async Task NormalizeTradingCommission_ParsesRate()
    {
        var privateApi = new FakeBitflyerPrivateApi(
            response: Array.Empty<RawPrivateDtos.BalanceResponse>(),
            tradingCommissionJson: "{\"commission_rate\":0.15}");
        var api = BitflyerTestHelpers.CreateNormalizedApi(
            new RawPublicDtos.Ticker { ProductCode = "BTC_JPY" },
            BitflyerTestHelpers.CreateResolver(),
            privateApi: privateApi);

        var call = await api.GetTradingCommissionCallAsync(new Symbol("BTC/JPY"));
        var ok = Assert.IsType<CallResult<BitflyerTradingCommissionNormalized>.Ok>(call.Result);

        Assert.Equal("BTC_JPY", ok.Response.ProductCode);
        Assert.Equal(0.15m, ok.Response.CommissionRate);
    }
}
