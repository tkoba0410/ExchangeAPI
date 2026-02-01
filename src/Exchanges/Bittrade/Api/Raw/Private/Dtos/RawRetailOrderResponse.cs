using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Dtos;

public sealed record RawRetailOrderResponse(
    [property: JsonPropertyName("code")] int Code,
    [property: JsonPropertyName("data")] long? Data,
    [property: JsonPropertyName("message")] string? Message,
    [property: JsonPropertyName("success")] bool? Success);
