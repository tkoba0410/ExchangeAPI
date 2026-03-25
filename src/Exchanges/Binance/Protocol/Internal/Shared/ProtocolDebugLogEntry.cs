namespace ExchangeApi.Exchanges.Binance.Protocol.Internal.Shared;

public sealed class ProtocolDebugLogEntry
{
    public required string EndpointId { get; init; }
    public required string Method { get; init; }
    public required string Path { get; init; }
    public IReadOnlyDictionary<string, string>? Query { get; init; }
    public string? BodyText { get; init; }
    public int? StatusCode { get; init; }
    public string? ResponseBodyText { get; init; }
    public required DateTimeOffset TimestampUtc { get; init; }
    public required DateTimeOffset TimestampJst { get; init; }
    public string? ErrorMessage { get; init; }
}
