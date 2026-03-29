using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetWithdrawals;

public static class GetWithdrawals
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
        [JsonPropertyName("status")]
        public required BitflyerTransferStatus Status { get; init; }
        [JsonPropertyName("event_date")]
        public required DateTimeOffset EventDate { get; init; }
    }
}
