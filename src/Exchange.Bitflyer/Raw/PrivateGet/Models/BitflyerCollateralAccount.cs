using System.Text.Json.Serialization;

namespace ExchangeApi.Adapter.Bitflyer.Models;

public sealed record BitflyerCollateralAccount(
    [property: JsonPropertyName("currency_code")] string CurrencyCode,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("available")] decimal Available);
