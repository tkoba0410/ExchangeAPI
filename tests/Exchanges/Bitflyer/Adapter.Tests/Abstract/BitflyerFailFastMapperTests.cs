using System;
using ExchangeApi.Common.Enums;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;
using ExchangeApi.Exchanges.Bitflyer.Raw;
using Xunit;

namespace ExchangeApi.Exchanges.Bitflyer.Tests;

public sealed class BitflyerFailFastMapperTests
{
    [Fact]
    public void SideMapper_UnknownSide_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => BitflyerSideMapper.FromApi("UNKNOWN"));
    }

    [Fact]
    public void TradingMapper_UnknownChildOrderType_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            BitflyerTradingMapper.MapOrderTypeFromExchange((ChildOrderType)999));
    }

    [Fact]
    public void TradingMapper_UnknownTimeInForce_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            BitflyerTradingMapper.MapTimeInForce((ExchangeApi.Common.Enums.TimeInForce)999));
    }
}
