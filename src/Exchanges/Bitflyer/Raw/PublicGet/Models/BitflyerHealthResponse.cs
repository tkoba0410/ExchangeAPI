using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Raw;

public sealed record BitflyerHealthResponse(
    [property: JsonPropertyName("status")] string? Status);
