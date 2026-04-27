using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Tests.Exchanges.Bitflyer.Protocol.Tests.Realtime;

public sealed class BitflyerRealtimeChannelsTests
{
    [Fact]
    public void ChannelBuilders_ReturnExpectedChannelNames()
    {
        Assert.Equal("lightning_ticker_BTC_JPY", BitflyerRealtimeChannels.Ticker(ProductCodes.BtcJpy));
        Assert.Equal("lightning_executions_BTC_JPY", BitflyerRealtimeChannels.Executions(ProductCodes.BtcJpy));
        Assert.Equal("lightning_board_snapshot_BTC_JPY", BitflyerRealtimeChannels.BoardSnapshot(ProductCodes.BtcJpy));
        Assert.Equal("lightning_board_BTC_JPY", BitflyerRealtimeChannels.Board(ProductCodes.BtcJpy));
        Assert.Equal("child_order_events", BitflyerRealtimeChannels.ChildOrderEvents());
        Assert.Equal("parent_order_events", BitflyerRealtimeChannels.ParentOrderEvents());
    }

    [Fact]
    public void ChannelBuilders_RejectBlankProductCode()
    {
        Assert.Throws<ArgumentException>(() => BitflyerRealtimeChannels.Ticker(""));
    }
}
