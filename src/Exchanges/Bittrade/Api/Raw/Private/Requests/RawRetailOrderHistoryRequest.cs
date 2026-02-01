using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Requests;

public sealed record RawRetailOrderHistoryRequest(
    [property: JsonPropertyName("symbol")] string? Symbol = null,
    [property: JsonPropertyName("direct")] int? Direct = null,
    [property: JsonPropertyName("status")] int? Status = null,
    [property: JsonPropertyName("start_time")] long? StartTime = null,
    [property: JsonPropertyName("end_time")] long? EndTime = null,
    [property: JsonPropertyName("size")] int? Size = null);
