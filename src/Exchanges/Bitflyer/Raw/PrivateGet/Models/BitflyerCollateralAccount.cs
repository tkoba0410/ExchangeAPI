using System.Text.Json.Serialization;
namespace Exchange.Bitflyer.Raw;

public sealed record BitflyerCollateralAccount(
    [property: JsonPropertyName("currency_code")] string CurrencyCode,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("available")] decimal Available);
