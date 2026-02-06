using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Raw.Private.Dtos;

public sealed class CreateWithdrawalResponse
{
    [JsonPropertyName("message_id")] public string MessageId { get; init; } = string.Empty;
}
