using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetAddresses;

public static class GetAddresses
{
    public sealed class Item
    {
        [JsonPropertyName("type")]
        public required string Type { get; init; }
        [JsonPropertyName("currency_code")]
        public required string CurrencyCode { get; init; }
        [JsonPropertyName("address")]
        public required string Address { get; init; }
    }
}
