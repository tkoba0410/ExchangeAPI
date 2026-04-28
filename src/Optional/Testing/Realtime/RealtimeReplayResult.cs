using ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Models;

namespace ExchangeApi.Optional.Testing.Realtime;

public sealed record RealtimeReplayResult<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public required IReadOnlyList<RealtimeDiagnosticEvent> Diagnostics { get; init; }
    public string? RejectionReason { get; init; }
    public string? ErrorKind { get; init; }
    public bool IsSuccessful => RejectionReason is null;
}
