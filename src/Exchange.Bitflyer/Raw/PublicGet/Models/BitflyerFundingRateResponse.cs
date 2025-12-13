using System.Text.Json.Serialization;

namespace Exchange.Bitflyer.Raw.PublicGet.Models;

public sealed record BitflyerFundingRateResponse(
    [property: JsonPropertyName("product_code")] string ProductCode,
    [property: JsonPropertyName("funding_rate")] decimal FundingRate);
