using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Protocol.Realtime;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Models;

public abstract record BitflyerRealtimeStreamEvent<T>
{
    public required string Channel { get; init; }

    [JsonConverter(typeof(BitflyerUtcTimestampJsonConverter))]
    public required DateTimeOffset OccurredAt { get; init; }
}

public sealed record BitflyerRealtimeData<T> : BitflyerRealtimeStreamEvent<T>
{
    public required T Value { get; init; }
}

public sealed record BitflyerRealtimeDiagnostic<T> : BitflyerRealtimeStreamEvent<T>
{
    public required RealtimeDiagnosticEvent Diagnostic { get; init; }
}

public sealed record BitflyerRealtimeReconnecting<T> : BitflyerRealtimeStreamEvent<T>
{
    public required int Attempt { get; init; }
    public required string Reason { get; init; }
}

public sealed record BitflyerRealtimeReconnected<T> : BitflyerRealtimeStreamEvent<T>
{
    public required int Attempt { get; init; }
}

public sealed record BitflyerRealtimeAuthenticationReplayed<T> : BitflyerRealtimeStreamEvent<T>
{
    public required int Attempt { get; init; }
}

public sealed record BitflyerRealtimeResubscribed<T> : BitflyerRealtimeStreamEvent<T>
{
    public required int Attempt { get; init; }
}

public sealed record BitflyerRealtimeContinuityLost<T> : BitflyerRealtimeStreamEvent<T>
{
    public required string Reason { get; init; }
    public int? ReconnectAttempt { get; init; }
}

public sealed record BitflyerRealtimeMessageRejected<T> : BitflyerRealtimeStreamEvent<T>
{
    public required BitflyerRealtimeErrorKind ErrorKind { get; init; }
    public required string Reason { get; init; }
}
