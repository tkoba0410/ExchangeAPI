using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Realtime.Models;

public sealed record BitflyerRealtimeParentOrderEventMessage : IProductRealtimeMessage
{
    public required string Channel { get; init; }

    [JsonConverter(typeof(BitflyerUtcTimestampJsonConverter))]
    public required DateTimeOffset ReceivedAt { get; init; }

    [JsonPropertyName("product_code")]
    public required string ProductCode { get; init; }

    [JsonPropertyName("parent_order_id")]
    public string? ParentOrderId { get; init; }

    [JsonPropertyName("parent_order_acceptance_id")]
    public string? ParentOrderAcceptanceId { get; init; }

    [JsonPropertyName("event_date")]
    [JsonConverter(typeof(BitflyerUtcTimestampJsonConverter))]
    public required DateTimeOffset EventDate { get; init; }

    [JsonPropertyName("event_type")]
    public required string EventType { get; init; }

    [JsonPropertyName("parent_order_type")]
    public string? ParentOrderType { get; init; }

    [JsonPropertyName("reason")]
    public string? Reason { get; init; }

    [JsonPropertyName("child_order_type")]
    public string? ChildOrderType { get; init; }

    [JsonPropertyName("parameter_index")]
    public long? ParameterIndex { get; init; }

    [JsonPropertyName("child_order_acceptance_id")]
    public string? ChildOrderAcceptanceId { get; init; }

    [JsonPropertyName("side")]
    public string? Side { get; init; }

    [JsonPropertyName("price")]
    public decimal? Price { get; init; }

    [JsonPropertyName("size")]
    public decimal? Size { get; init; }

    [JsonPropertyName("expire_date")]
    [JsonConverter(typeof(BitflyerNullableUtcTimestampJsonConverter))]
    public DateTimeOffset? ExpireDate { get; init; }
}
