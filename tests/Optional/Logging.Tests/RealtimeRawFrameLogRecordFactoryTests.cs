using ExchangeApi.Optional.Logging.Realtime;

namespace ExchangeApi.Optional.Logging.Tests;

public sealed class RealtimeRawFrameLogRecordFactoryTests
{
    [Fact]
    public void Create_SkipsBodyByDefault()
    {
        var factory = new RealtimeRawFrameLogRecordFactory();

        var record = factory.Create(
            "bitFlyer",
            "lightning_ticker_BTC_JPY",
            DateTimeOffset.Parse("2026-04-28T00:00:00Z"),
            """{"message":{"ltp":100}}""");

        Assert.True(record.BodySkipped);
        Assert.Equal("BodyLoggingDisabled", record.SkipReason);
        Assert.Null(record.Body);
    }

    [Fact]
    public void Create_RedactsBodyWhenEnabled()
    {
        var factory = new RealtimeRawFrameLogRecordFactory(new RealtimeRawFrameLogOptions
        {
            IncludeBody = true,
        });

        var record = factory.Create(
            "bitFlyer",
            "child_order_events",
            DateTimeOffset.Parse("2026-04-28T00:00:00Z"),
            """{"api_key":"key-123","message":{"event_type":"ORDER"}}""");

        Assert.False(record.BodySkipped);
        Assert.NotNull(record.Body);
        Assert.DoesNotContain("key-123", record.Body, StringComparison.Ordinal);
        Assert.Contains("[REDACTED]", record.Body, StringComparison.Ordinal);
    }

    [Fact]
    public void Create_SkipsOversizedBodyWithoutTruncation()
    {
        var factory = new RealtimeRawFrameLogRecordFactory(new RealtimeRawFrameLogOptions
        {
            IncludeBody = true,
            MaxRawFrameBodyBytes = 8,
        });

        var record = factory.Create(
            "bitFlyer",
            "lightning_board_BTC_JPY",
            DateTimeOffset.Parse("2026-04-28T00:00:00Z"),
            """{"message":{"large":true}}""");

        Assert.True(record.BodySkipped);
        Assert.Equal("PayloadTooLarge", record.SkipReason);
        Assert.Null(record.Body);
    }

    [Fact]
    public void Create_SkipsWhenRedactionCannotParseJson()
    {
        var factory = new RealtimeRawFrameLogRecordFactory(new RealtimeRawFrameLogOptions
        {
            IncludeBody = true,
        });

        var record = factory.Create(
            "bitFlyer",
            "lightning_ticker_BTC_JPY",
            DateTimeOffset.Parse("2026-04-28T00:00:00Z"),
            """{"message":""");

        Assert.True(record.BodySkipped);
        Assert.Equal("RedactionFailed", record.SkipReason);
        Assert.Null(record.Body);
    }
}
