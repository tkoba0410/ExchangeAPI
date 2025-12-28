using ExchangeApi.Contracts.Dtos;
using ExchangeApi.Common.Enums;
using ExchangeApi.Common.Types;
using ExchangeApi.Core.Contracts.Errors;
using ExchangeApi.Exchanges.Bittrade.Adapter.Mappers;
using ExchangeApi.Exchanges.Bittrade.Wire.Private.Models;
using Xunit;

namespace ExchangeApi.Exchanges.Bittrade.Tests;

public sealed class BittradeFailFastMapperTests
{
    [Fact]
    public void ToOpenOrder_UnknownSide_Throws()
    {
        var wire = CreateOpenOrder(side: "hold", type: "buy-limit");

        Assert.Throws<ExchangeApiException>(() =>
            BittradeTradingMapper.ToOpenOrder(new Symbol("BTC/JPY"), wire));
    }

    [Fact]
    public void ToOpenOrder_UnknownType_Throws()
    {
        var wire = CreateOpenOrder(side: "buy", type: "mystery");

        Assert.Throws<ExchangeApiException>(() =>
            BittradeTradingMapper.ToOpenOrder(new Symbol("BTC/JPY"), wire));
    }

    [Fact]
    public void ToOrderStatus_UnknownState_Throws()
    {
        var wire = new BittradeWireOrder(
            RawOrderId: "1",
            RawSymbol: "btcjpy",
            Side: "buy",
            Type: "buy-limit",
            State: "mystery",
            Price: 100m,
            Size: 1m,
            FilledSize: 0m,
            OutstandingSize: 1m,
            CreatedAt: DateTimeOffset.UtcNow);

        Assert.Throws<ExchangeApiException>(() =>
            BittradeTradingMapper.ToOrderStatus("BTC_JPY", wire, new OrderKey(OrderIdKind.ExchangeOrderId, "1")));
    }

    private static BittradeWireOpenOrder CreateOpenOrder(string side, string type) =>
        new(
            RawOrderId: "1",
            RawSymbol: "btcjpy",
            Side: side,
            Type: type,
            State: "submitted",
            Price: 100m,
            Size: 1m,
            FilledSize: 0m,
            CreatedAt: DateTimeOffset.UtcNow);
}
