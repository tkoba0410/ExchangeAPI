using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetMarkets;

public static class GetMarkets
{
    public sealed class Item
    {
        [JsonPropertyName("product_code")]
        public required string ProductCode { get; init; }

        [JsonPropertyName("market_type")]
        public required string MarketType { get; init; }
    }
}
