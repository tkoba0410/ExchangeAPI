using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetTicker;

/// <summary>
/// Requests the current bitFlyer ticker for a product code.
/// </summary>
public sealed class GetTickerRequest
{
    /// <summary>
    /// bitFlyer product code such as <c>BTC_JPY</c>.
    /// </summary>
    [JsonPropertyName("product_code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? ProductCode { get; init; }
}
