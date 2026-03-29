using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetMarkets;

public static class GetMarkets
{
    public sealed class Item
    {
        [JsonPropertyName("product_code")]
        public required string ProductCode { get; init; }

        [JsonPropertyName("market_type")]
        public required BitflyerMarketType MarketType { get; init; }
    }
}
