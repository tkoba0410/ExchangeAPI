namespace ExchangeApi.Primitives.Protocol;

public sealed class ProtocolResponse
{
    public required int StatusCode { get; init; }
    public IReadOnlyDictionary<string, string[]>? Headers { get; init; }
    public string? BodyText { get; init; }
}
