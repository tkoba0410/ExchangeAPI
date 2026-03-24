using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetTradingCommission;

public sealed class GetTradingCommissionRequest
{
    [JsonPropertyName("product_code")]
    public string ProductCode { get; init; } = string.Empty;
}
