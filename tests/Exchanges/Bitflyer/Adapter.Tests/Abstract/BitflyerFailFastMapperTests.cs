using System;
using ExchangeApi.Common.Enums;
using ExchangeApi.Exchanges.Bitflyer.Normalize.Mappers;
using Xunit;

namespace ExchangeApi.Exchanges.Bitflyer.Tests;

public sealed class BitflyerFailFastMapperTests
{
    [Fact]
    public void SideMapper_UnknownSide_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => BitflyerSideMapper.ToExchangeSide("UNKNOWN"));
    }

    [Fact]
    public void TradingMapper_UnknownChildOrderType_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            BitflyerTradingMapper.ParseChildOrderType("UNKNOWN"));
    }

    [Fact]
    public void TradingMapper_UnknownTimeInForce_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            BitflyerTradingMapper.MapTimeInForce((ExchangeApi.Common.Enums.TimeInForce)999));
    }

    [Fact]
    public void ParentOrderMapper_UnknownOrderMethod_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            BitflyerParentOrderMapper.ParseOrderMethod("UNKNOWN"));
    }

    [Fact]
    public void ParentOrderMapper_UnknownParentOrderState_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            BitflyerParentOrderMapper.ParseParentOrderState("UNKNOWN"));
    }
}
