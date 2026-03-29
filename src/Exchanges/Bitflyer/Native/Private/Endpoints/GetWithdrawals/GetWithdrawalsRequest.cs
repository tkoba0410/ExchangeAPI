using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetWithdrawals;

public sealed class GetWithdrawalsRequest
{
    [JsonPropertyName("count")]
    public int? Count { get; init; }
    [JsonPropertyName("before")]
    public long? Before { get; init; }
    [JsonPropertyName("after")]
    public long? After { get; init; }
    [JsonPropertyName("message_id")]
    public string? MessageId { get; init; }
}
