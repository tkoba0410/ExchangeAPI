using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCollateralHistory;

public static class GetCollateralHistory
{
    public sealed class Item
    {
        [JsonPropertyName("id")]
        public required long Id { get; init; }

        [JsonPropertyName("currency_code")]
        public required string CurrencyCode { get; init; }

        [JsonPropertyName("change")]
        public required decimal Change { get; init; }

        [JsonPropertyName("amount")]
        public required decimal Amount { get; init; }

        [JsonPropertyName("reason_code")]
        public required string ReasonCode { get; init; }

        [JsonPropertyName("date")]
        [JsonConverter(typeof(BitflyerUtcTimestampJsonConverter))]
        public required DateTimeOffset Date { get; init; }
    }
}
