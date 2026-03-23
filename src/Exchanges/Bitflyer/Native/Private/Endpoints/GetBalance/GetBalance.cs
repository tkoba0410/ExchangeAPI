using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalance;

public static class GetBalance
{
    public sealed class Item
    {
        [JsonPropertyName("currency_code")]
        public required string CurrencyCode { get; init; }

        [JsonPropertyName("amount")]
        public required decimal Amount { get; init; }

        [JsonPropertyName("available")]
        public required decimal Available { get; init; }
    }
}
