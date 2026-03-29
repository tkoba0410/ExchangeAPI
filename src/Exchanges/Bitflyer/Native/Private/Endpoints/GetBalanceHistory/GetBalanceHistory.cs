using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalanceHistory;

public static class GetBalanceHistory
{
    public sealed class Item
    {
        [JsonPropertyName("id")]
        public required long Id { get; init; }
        [JsonPropertyName("trade_date")]
        public required DateTimeOffset TradeDate { get; init; }
        [JsonPropertyName("event_date")]
        public required DateTimeOffset EventDate { get; init; }
        [JsonPropertyName("product_code")]
        public string? ProductCode { get; init; }
        [JsonPropertyName("currency_code")]
        public required string CurrencyCode { get; init; }
        [JsonPropertyName("trade_type")]
        public required BitflyerTradeType TradeType { get; init; }
        [JsonPropertyName("price")]
        public required decimal Price { get; init; }
        [JsonPropertyName("amount")]
        public required decimal Amount { get; init; }
        [JsonPropertyName("quantity")]
        public required decimal Quantity { get; init; }
        [JsonPropertyName("commission")]
        public required decimal Commission { get; init; }
        [JsonPropertyName("balance")]
        public required decimal Balance { get; init; }
        [JsonPropertyName("order_id")]
        public string? OrderId { get; init; }
    }
}
