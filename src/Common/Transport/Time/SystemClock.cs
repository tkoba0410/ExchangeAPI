namespace Common.Transport.Time;

/// <summary>
/// 実際のシステム時刻を返す実装。
/// </summary>
public sealed class SystemClock : IExchangeClock
{
    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}
