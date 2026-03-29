using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.Withdraw;

public sealed class WithdrawResponse
{
    [JsonPropertyName("message_id")]
    public required string MessageId { get; init; }
}
