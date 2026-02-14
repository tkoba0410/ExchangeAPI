using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Dtos;

public sealed record PostWithdrawVirtualByAddressIdCreateResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] long? Data);
