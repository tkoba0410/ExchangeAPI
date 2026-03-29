using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrders;

public static class GetParentOrders
{
    public sealed class Item
    {
        [JsonPropertyName("id")]
        public required long Id { get; init; }
        [JsonPropertyName("parent_order_id")]
        public required string ParentOrderId { get; init; }
        [JsonPropertyName("product_code")]
        public required string ProductCode { get; init; }
        [JsonPropertyName("side")]
        public required string Side { get; init; }
        [JsonPropertyName("parent_order_type")]
        public required string ParentOrderType { get; init; }
        [JsonPropertyName("price")]
        public required decimal Price { get; init; }
        [JsonPropertyName("average_price")]
        public required decimal AveragePrice { get; init; }
        [JsonPropertyName("size")]
        public required decimal Size { get; init; }
        [JsonPropertyName("parent_order_state")]
        public required string ParentOrderState { get; init; }
        [JsonPropertyName("expire_date")]
        public required DateTimeOffset ExpireDate { get; init; }
        [JsonPropertyName("parent_order_date")]
        public required DateTimeOffset ParentOrderDate { get; init; }
        [JsonPropertyName("parent_order_acceptance_id")]
        public required string ParentOrderAcceptanceId { get; init; }
        [JsonPropertyName("outstanding_size")]
        public required decimal OutstandingSize { get; init; }
        [JsonPropertyName("cancel_size")]
        public required decimal CancelSize { get; init; }
        [JsonPropertyName("executed_size")]
        public required decimal ExecutedSize { get; init; }
        [JsonPropertyName("total_commission")]
        public required decimal TotalCommission { get; init; }
    }
}
