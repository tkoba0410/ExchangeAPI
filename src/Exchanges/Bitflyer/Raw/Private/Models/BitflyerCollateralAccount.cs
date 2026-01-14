using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private.Models;

public sealed record CollateralAccount(
    [property: JsonPropertyName("currency_code")] string CurrencyCode,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("available")] decimal Available);
