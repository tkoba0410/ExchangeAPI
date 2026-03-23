namespace ExchangeApi.Primitives.Protocol;

public sealed class ProtocolRequest
{
    public required string EndpointId { get; init; }
    public required string Method { get; init; }
    public required string Path { get; init; }
    public IReadOnlyDictionary<string, string>? Query { get; init; }
    public string? BodyText { get; init; }
}
