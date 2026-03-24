using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.CancelAllChildOrders;

public sealed class CancelAllChildOrdersRequest
{
    [JsonPropertyName("product_code")]
    public required string ProductCode { get; init; }
}
