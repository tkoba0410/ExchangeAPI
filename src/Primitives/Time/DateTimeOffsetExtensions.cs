using System.Globalization;

namespace ExchangeApi.Primitives.Time;

public static class DateTimeOffsetExtensions
{
    private static readonly TimeSpan JstOffset = TimeSpan.FromHours(9);

    public static DateTimeOffset ToUtc(this DateTimeOffset value)
        => value.ToOffset(TimeSpan.Zero);

    public static string ToUtcString(this DateTimeOffset value)
        => value.ToUtc().ToString("O", CultureInfo.InvariantCulture);

    public static DateTimeOffset ToJst(this DateTimeOffset value)
        => value.ToOffset(JstOffset);

    public static string ToJstString(this DateTimeOffset value)
        => value.ToJst().ToString("O", CultureInfo.InvariantCulture);
}
