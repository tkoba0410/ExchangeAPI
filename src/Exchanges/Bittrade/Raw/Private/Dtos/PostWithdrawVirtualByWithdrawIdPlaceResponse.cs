using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Dtos;

public sealed record PostWithdrawVirtualByWithdrawIdPlaceResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] long? Data);
