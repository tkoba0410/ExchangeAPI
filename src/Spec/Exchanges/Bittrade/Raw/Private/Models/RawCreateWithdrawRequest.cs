using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record RawCreateWithdrawRequest(
    [property: JsonPropertyName("address")] string Address,
    [property: JsonPropertyName("amount")] string Amount,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("fee")] string? Fee = null,
    [property: JsonPropertyName("addr-tag")] string? AddressTag = null);

public sealed record RawCreateWithdrawResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] long? Data);

public sealed record RawCancelWithdrawResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] long? Data);
