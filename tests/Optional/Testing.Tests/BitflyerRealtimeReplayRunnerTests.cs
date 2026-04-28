using ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Models;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;
using ExchangeApi.Optional.Testing.Realtime;

namespace ExchangeApi.Optional.Testing.Tests;

public sealed class BitflyerRealtimeReplayRunnerTests
{
    [Fact]
    public async Task ReplayTickerAsync_DecodesFixtureFrame()
    {
        var frame = await LoadFrameAsync("ticker.json", BitflyerRealtimeChannels.Ticker(ProductCodes.BtcJpy));

        var result = await BitflyerRealtimeReplayRunner.ReplayTickerAsync(ProductCodes.BtcJpy, [frame]);

        Assert.True(result.IsSuccessful);
        var ticker = Assert.Single(result.Items);
        Assert.Equal(ProductCodes.BtcJpy, ticker.ProductCode);
        Assert.Equal(10000000m, ticker.Ltp);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.EventType == RealtimeDiagnosticEventTypes.RawFrameReceived);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.EventType == RealtimeDiagnosticEventTypes.MessageDecoded);
    }

    [Fact]
    public async Task ReplayExecutionsAsync_DecodesFixtureFrame()
    {
        var frame = await LoadFrameAsync("executions.json", BitflyerRealtimeChannels.Executions(ProductCodes.BtcJpy));

        var result = await BitflyerRealtimeReplayRunner.ReplayExecutionsAsync(ProductCodes.BtcJpy, [frame]);

        Assert.True(result.IsSuccessful);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal("BUY", result.Items[0].Side);
        Assert.Equal("SELL", result.Items[1].Side);
    }

    [Fact]
    public async Task ReplayBoardSnapshotAsync_DecodesFixtureFrame()
    {
        var frame = await LoadFrameAsync("board-snapshot.json", BitflyerRealtimeChannels.BoardSnapshot(ProductCodes.BtcJpy));

        var result = await BitflyerRealtimeReplayRunner.ReplayBoardSnapshotAsync(ProductCodes.BtcJpy, [frame]);

        Assert.True(result.IsSuccessful);
        var snapshot = Assert.Single(result.Items);
        Assert.Equal(10000000m, snapshot.MidPrice);
        Assert.Equal(2, snapshot.Bids.Count);
        Assert.Equal(2, snapshot.Asks.Count);
    }

    [Fact]
    public async Task ReplayBoardDeltaAsync_DecodesFixtureFrame()
    {
        var frame = await LoadFrameAsync("board-delta.json", BitflyerRealtimeChannels.Board(ProductCodes.BtcJpy));

        var result = await BitflyerRealtimeReplayRunner.ReplayBoardDeltaAsync(ProductCodes.BtcJpy, [frame]);

        Assert.True(result.IsSuccessful);
        var delta = Assert.Single(result.Items);
        Assert.Equal(10000000m, delta.MidPrice);
        Assert.Equal(0m, delta.Bids[0].Size);
        Assert.Equal(0m, delta.Asks[0].Size);
    }

    [Fact]
    public async Task ReplayTickerAsync_ReturnsRejectedDiagnosticForMalformedFixture()
    {
        var frame = await LoadFrameAsync("malformed-ticker-missing-fields.json", BitflyerRealtimeChannels.Ticker(ProductCodes.BtcJpy));

        var result = await BitflyerRealtimeReplayRunner.ReplayTickerAsync(ProductCodes.BtcJpy, [frame]);

        Assert.False(result.IsSuccessful);
        Assert.Empty(result.Items);
        Assert.Equal(BitflyerRealtimeErrorKind.MessageDecodeFailed.ToString(), result.ErrorKind);
        Assert.Contains(result.Diagnostics, diagnostic => diagnostic.EventType == RealtimeDiagnosticEventTypes.MessageRejected);
        Assert.DoesNotContain(ProductCodes.BtcJpy, result.RejectionReason, StringComparison.Ordinal);
    }

    private static Task<RealtimeReplayFrame> LoadFrameAsync(string fileName, string channel)
    {
        var path = Path.Combine(
            AppContext.BaseDirectory,
            "Fixtures",
            "Realtime",
            "Bitflyer",
            "RawFrames",
            fileName);

        return RealtimeReplayFrame.FromFileAsync(
            channel,
            path,
            DateTimeOffset.Parse("2026-04-27T12:34:56Z"));
    }
}
