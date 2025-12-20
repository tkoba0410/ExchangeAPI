using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record RetailMaintainTimeResponse(
    [property: JsonPropertyName("code")] int Code,
    [property: JsonPropertyName("data")] RetailMaintainTimeData? Data,
    [property: JsonPropertyName("message")] string? Message = null,
    [property: JsonPropertyName("success")] bool? Success = null);

public sealed record RetailMaintainTimeData(
    [property: JsonPropertyName("start_time")] string StartTime,
    [property: JsonPropertyName("end_time")] string EndTime,
    [property: JsonPropertyName("ts")] long Ts,
    [property: JsonPropertyName("state")] int State);
