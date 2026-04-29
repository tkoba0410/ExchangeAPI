using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Models;

public sealed record RealtimeDiagnosticEvent
{
    public required string EventType { get; init; }

    [JsonConverter(typeof(BitflyerUtcTimestampJsonConverter))]
    public required DateTimeOffset ObservedAt { get; init; }

    public string? Venue { get; init; }
    public string? Channel { get; init; }
    public string? ProductCode { get; init; }
    public string? ConnectionId { get; init; }
    public string? SubscriptionId { get; init; }
    public string? Severity { get; init; }
    public string? Reason { get; init; }
    public string? ErrorKind { get; init; }
    public IReadOnlyDictionary<string, string>? Attributes { get; init; }
}

public static class RealtimeDiagnosticEventTypes
{
    public const string Connecting = "Connecting";
    public const string Connected = "Connected";
    public const string SubscribeRequested = "SubscribeRequested";
    public const string Subscribed = "Subscribed";
    public const string RawFrameReceived = "RawFrameReceived";
    public const string RawFrameLogged = "RawFrameLogged";
    public const string RawFrameLoggingSkipped = "RawFrameLoggingSkipped";
    public const string MessageDecoded = "MessageDecoded";
    public const string MessageRejected = "MessageRejected";
    public const string NonTargetMessageIgnored = "NonTargetMessageIgnored";
    public const string ContinuityLost = "ContinuityLost";
    public const string Reconnecting = "Reconnecting";
    public const string Reconnected = "Reconnected";
    public const string Resubscribed = "Resubscribed";
    public const string Closed = "Closed";
    public const string Failed = "Failed";
}

public static class RealtimeDiagnosticSeverities
{
    public const string Trace = "Trace";
    public const string Info = "Info";
    public const string Warning = "Warning";
    public const string Error = "Error";
}
