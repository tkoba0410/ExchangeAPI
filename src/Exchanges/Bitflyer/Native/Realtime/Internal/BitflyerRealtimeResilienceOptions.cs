namespace ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Internal;

internal sealed class BitflyerRealtimeResilienceOptions
{
    internal static BitflyerRealtimeResilienceOptions Disabled { get; } = new()
    {
        MaxAttempts = 0,
        InitialDelay = TimeSpan.FromSeconds(1),
        MaxDelay = TimeSpan.FromSeconds(10),
    };

    internal static BitflyerRealtimeResilienceOptions Default { get; } = new()
    {
        MaxAttempts = 3,
        InitialDelay = TimeSpan.FromSeconds(1),
        MaxDelay = TimeSpan.FromSeconds(10),
    };

    internal int MaxAttempts { get; init; }
    internal TimeSpan InitialDelay { get; init; }
    internal TimeSpan MaxDelay { get; init; }
    internal TimeSpan? IdleTimeout { get; init; }

    internal TimeSpan GetDelay(int attempt)
    {
        if (attempt <= 1)
        {
            return InitialDelay;
        }

        var multiplier = Math.Pow(2, attempt - 1);
        var ticks = InitialDelay.Ticks * multiplier;
        if (ticks >= MaxDelay.Ticks)
        {
            return MaxDelay;
        }

        return TimeSpan.FromTicks((long)ticks);
    }
}
