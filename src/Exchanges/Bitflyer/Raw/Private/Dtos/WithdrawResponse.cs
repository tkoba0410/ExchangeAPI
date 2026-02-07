using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private.Dtos;

public sealed class WithdrawResponse
{
    [JsonPropertyName("message_id")] public string MessageId { get; init; } = string.Empty;
}
