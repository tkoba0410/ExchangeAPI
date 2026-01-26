using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;

public sealed record RawRetailOrderDetailResponse(
    [property: JsonPropertyName("code")] int Code,
    [property: JsonPropertyName("data")] RawRetailOrderEntry? Data,
    [property: JsonPropertyName("message")] string? Message,
    [property: JsonPropertyName("success")] bool? Success);
