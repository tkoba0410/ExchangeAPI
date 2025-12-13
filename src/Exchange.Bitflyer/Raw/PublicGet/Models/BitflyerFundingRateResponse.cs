using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw;

public sealed record BitflyerFundingRateResponse(
[property: JsonPropertyName("product_code")] ProductCode ProductCode,
    [property: JsonPropertyName("funding_rate")] decimal FundingRate);
