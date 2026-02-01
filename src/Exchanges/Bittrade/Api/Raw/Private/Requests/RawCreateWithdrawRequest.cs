using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Requests;

public sealed record RawCreateWithdrawRequest(
    [property: JsonPropertyName("address")] string Address,
    [property: JsonPropertyName("amount")] string Amount,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("fee")] string? Fee = null,
    [property: JsonPropertyName("addr-tag")] string? AddressTag = null);
