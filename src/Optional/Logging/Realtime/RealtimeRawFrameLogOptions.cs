namespace ExchangeApi.Optional.Logging.Realtime;

public sealed class RealtimeRawFrameLogOptions
{
    public bool IncludeBody { get; init; }

    public int MaxRawFrameBodyBytes { get; init; } = 65_536;
}
