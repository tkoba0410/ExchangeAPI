using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bitflyer.Vocabulary;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetChildOrders;

public static class GetChildOrders
{
    public sealed class Item
    {
        [JsonPropertyName("id")]
        public required long Id { get; init; }

        [JsonPropertyName("child_order_id")]
        public required string ChildOrderId { get; init; }

        [JsonPropertyName("product_code")]
        public required string ProductCode { get; init; }

        [JsonPropertyName("side")]
        public required BitflyerOrderSide Side { get; init; }

        [JsonPropertyName("child_order_type")]
        public required BitflyerChildOrderType ChildOrderType { get; init; }

        [JsonPropertyName("price")]
        public required decimal Price { get; init; }

        [JsonPropertyName("average_price")]
        public required decimal AveragePrice { get; init; }

        [JsonPropertyName("size")]
        public required decimal Size { get; init; }

        [JsonPropertyName("child_order_state")]
        public required BitflyerOrderState ChildOrderState { get; init; }

        [JsonPropertyName("expire_date")]
        public required DateTimeOffset ExpireDate { get; init; }

        [JsonPropertyName("child_order_date")]
        public required DateTimeOffset ChildOrderDate { get; init; }

        [JsonPropertyName("child_order_acceptance_id")]
        public required string ChildOrderAcceptanceId { get; init; }

        [JsonPropertyName("outstanding_size")]
        public required decimal OutstandingSize { get; init; }

        [JsonPropertyName("cancel_size")]
        public required decimal CancelSize { get; init; }

        [JsonPropertyName("executed_size")]
        public required decimal ExecutedSize { get; init; }

        [JsonPropertyName("total_commission")]
        public required decimal TotalCommission { get; init; }

        [JsonPropertyName("time_in_force")]
        public required BitflyerTimeInForce TimeInForce { get; init; }
    }
}
