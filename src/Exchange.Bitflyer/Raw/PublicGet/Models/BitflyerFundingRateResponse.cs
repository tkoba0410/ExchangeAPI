using System.Text.Json.Serialization;

namespace ExchangeApi.Adapter.Bitflyer.Models;

public sealed record BitflyerFundingRateResponse(
    [property: JsonPropertyName("product_code")] string ProductCode,
    [property: JsonPropertyName("funding_rate")] decimal FundingRate);
