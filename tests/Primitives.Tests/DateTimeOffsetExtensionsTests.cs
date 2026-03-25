using ExchangeApi.Primitives.Time;

namespace ExchangeApi.Primitives.Tests;

public sealed class DateTimeOffsetExtensionsTests
{
    [Fact]
    public void ToUtc_NormalizesOffsetToZero()
    {
        var value = new DateTimeOffset(2026, 3, 26, 15, 30, 45, 123, TimeSpan.FromHours(9));

        var actual = value.ToUtc();

        Assert.Equal(TimeSpan.Zero, actual.Offset);
        Assert.Equal(new DateTimeOffset(2026, 3, 26, 6, 30, 45, 123, TimeSpan.Zero), actual);
    }

    [Fact]
    public void ToUtcString_ReturnsRoundTripUtcString()
    {
        var value = new DateTimeOffset(2026, 3, 26, 15, 30, 45, 123, TimeSpan.FromHours(9));

        var actual = value.ToUtcString();

        Assert.Equal("2026-03-26T06:30:45.1230000+00:00", actual);
    }

    [Fact]
    public void ToJst_NormalizesOffsetToPlusNine()
    {
        var value = new DateTimeOffset(2026, 3, 26, 6, 30, 45, 123, TimeSpan.Zero);

        var actual = value.ToJst();

        Assert.Equal(TimeSpan.FromHours(9), actual.Offset);
        Assert.Equal(new DateTimeOffset(2026, 3, 26, 15, 30, 45, 123, TimeSpan.FromHours(9)), actual);
    }

    [Fact]
    public void ToJstString_ReturnsRoundTripJstString()
    {
        var value = new DateTimeOffset(2026, 3, 26, 6, 30, 45, 123, TimeSpan.Zero);

        var actual = value.ToJstString();

        Assert.Equal("2026-03-26T15:30:45.1230000+09:00", actual);
    }
}
