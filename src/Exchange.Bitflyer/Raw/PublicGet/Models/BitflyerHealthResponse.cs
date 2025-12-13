using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw.PublicGet.Models;

public sealed record BitflyerHealthResponse(
    [property: JsonPropertyName("status")] string? Status);
