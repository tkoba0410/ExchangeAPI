using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetExecutions;

public static class GetExecutions
{
    public sealed class Item
    {
        [JsonPropertyName("id")]
        public required long Id { get; init; }

        [JsonPropertyName("child_order_id")]
        public required string ChildOrderId { get; init; }

        [JsonPropertyName("side")]
        public required BitflyerOrderSide Side { get; init; }

        [JsonPropertyName("price")]
        public required decimal Price { get; init; }

        [JsonPropertyName("size")]
        public required decimal Size { get; init; }

        [JsonPropertyName("commission")]
        public required decimal Commission { get; init; }

        [JsonPropertyName("exec_date")]
        [JsonConverter(typeof(BitflyerUtcTimestampJsonConverter))]
        public required DateTimeOffset ExecDate { get; init; }

        [JsonPropertyName("child_order_acceptance_id")]
        public required string ChildOrderAcceptanceId { get; init; }
    }
}
