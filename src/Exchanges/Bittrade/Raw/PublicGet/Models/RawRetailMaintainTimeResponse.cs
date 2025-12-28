using System;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record RawRetailMaintainTimeResponse(
    [property: JsonPropertyName("code")] int Code,
    [property: JsonPropertyName("data")] RawRetailMaintainTimeData? Data,
    [property: JsonPropertyName("message")] string? Message = null,
    [property: JsonPropertyName("success")] bool? Success = null);

public sealed record RawRetailMaintainTimeData(
    [property: JsonPropertyName("start_time")] string StartTime,
    [property: JsonPropertyName("end_time")] string EndTime,
    [property: JsonPropertyName("ts")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset Ts,
    [property: JsonPropertyName("state")] int State);
