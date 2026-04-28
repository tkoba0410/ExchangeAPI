namespace ExchangeApi.Optional.Testing.Realtime;

public sealed record RealtimeReplayFrame
{
    public required string Channel { get; init; }
    public required string RawText { get; init; }
    public required DateTimeOffset ReceivedAt { get; init; }

    public static RealtimeReplayFrame Create(
        string channel,
        string rawText,
        DateTimeOffset? receivedAt = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(channel);
        ArgumentException.ThrowIfNullOrWhiteSpace(rawText);

        return new RealtimeReplayFrame
        {
            Channel = channel,
            RawText = rawText,
            ReceivedAt = receivedAt ?? DateTimeOffset.UtcNow,
        };
    }

    public static async Task<RealtimeReplayFrame> FromFileAsync(
        string channel,
        string path,
        DateTimeOffset? receivedAt = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        var rawText = await File.ReadAllTextAsync(path, cancellationToken).ConfigureAwait(false);
        return Create(channel, rawText, receivedAt);
    }
}
