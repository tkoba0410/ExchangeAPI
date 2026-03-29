using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetCoinOuts;

public static class GetCoinOuts
{
    public sealed class Item
    {
        [JsonPropertyName("id")]
        public required long Id { get; init; }
        [JsonPropertyName("order_id")]
        public required string OrderId { get; init; }
        [JsonPropertyName("currency_code")]
        public required string CurrencyCode { get; init; }
        [JsonPropertyName("amount")]
        public required decimal Amount { get; init; }
        [JsonPropertyName("address")]
        public required string Address { get; init; }
        [JsonPropertyName("tx_hash")]
        public required string TxHash { get; init; }
        [JsonPropertyName("fee")]
        public required decimal Fee { get; init; }
        [JsonPropertyName("additional_fee")]
        public required decimal AdditionalFee { get; init; }
        [JsonPropertyName("status")]
        public required string Status { get; init; }
        [JsonPropertyName("event_date")]
        public required DateTimeOffset EventDate { get; init; }
    }
}
