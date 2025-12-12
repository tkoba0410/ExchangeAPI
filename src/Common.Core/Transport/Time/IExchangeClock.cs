namespace ExchangeApi.Transport.Time;

/// <summary>
/// 時刻取得を抽象化するインターフェース。
/// 主に API 署名用のタイムスタンプ生成に使用する。
/// </summary>
public interface IExchangeClock
{
    DateTimeOffset UtcNow { get; }
}
