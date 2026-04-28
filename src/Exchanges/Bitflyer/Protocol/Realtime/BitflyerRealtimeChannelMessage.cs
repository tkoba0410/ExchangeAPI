using System.Text.Json;

namespace ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;

public sealed class BitflyerRealtimeChannelMessage
{
    public required string Channel { get; init; }
    public required JsonElement Message { get; init; }
    public required DateTimeOffset ReceivedAt { get; init; }
    public string? RawText { get; init; }
    public int? RawTextLength { get; init; }
}
