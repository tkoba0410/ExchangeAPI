using System.Text.Json.Serialization;

namespace ExchangeApi.Adapter.Bitflyer.Models;

public sealed record BitflyerHealthResponse(
    [property: JsonPropertyName("status")] string? Status);
