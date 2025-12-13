using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw;

public sealed record BitflyerHealthResponse(
    [property: JsonPropertyName("status")] string? Status);
