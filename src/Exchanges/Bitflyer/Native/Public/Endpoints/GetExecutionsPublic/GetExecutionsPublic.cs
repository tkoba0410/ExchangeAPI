using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetExecutionsPublic;

public static class GetExecutionsPublic
{
    public sealed class Item
    {
        [JsonPropertyName("id")]
        public required long Id { get; init; }

        [JsonPropertyName("side")]
        public required BitflyerOrderSide Side { get; init; }

        [JsonPropertyName("price")]
        public required decimal Price { get; init; }

        [JsonPropertyName("size")]
        public required decimal Size { get; init; }

        [JsonPropertyName("exec_date")]
        public required DateTimeOffset ExecDate { get; init; }

        [JsonPropertyName("buy_child_order_acceptance_id")]
        public required string BuyChildOrderAcceptanceId { get; init; }

        [JsonPropertyName("sell_child_order_acceptance_id")]
        public required string SellChildOrderAcceptanceId { get; init; }
    }
}
