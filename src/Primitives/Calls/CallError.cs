namespace ExchangeApi.Primitives.Calls;

public sealed class CallError
{
    public required string Kind { get; init; }
    public required string Message { get; init; }
    public int? HttpStatusCode { get; init; }
    public string? VenueErrorCode { get; init; }
    public string? VenueErrorMessage { get; init; }
}

public static class CallErrorKinds
{
    public const string Transport = "Transport";
    public const string Http = "Http";
    public const string Codec = "Codec";
    public const string Semantic = "Semantic";
    public const string Mapping = "Mapping";
}
