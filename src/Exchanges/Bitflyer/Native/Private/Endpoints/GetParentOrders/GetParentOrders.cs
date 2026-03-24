namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrders;

public static class GetParentOrders
{
    public sealed class Item
    {
        public required long Id { get; init; }
        public required string ParentOrderId { get; init; }
        public required string ProductCode { get; init; }
        public required string Side { get; init; }
        public required string ParentOrderType { get; init; }
        public required decimal Price { get; init; }
        public required decimal AveragePrice { get; init; }
        public required decimal Size { get; init; }
        public required string ParentOrderState { get; init; }
        public required DateTimeOffset ExpireDate { get; init; }
        public required DateTimeOffset ParentOrderDate { get; init; }
        public required string ParentOrderAcceptanceId { get; init; }
        public required decimal OutstandingSize { get; init; }
        public required decimal CancelSize { get; init; }
        public required decimal ExecutedSize { get; init; }
        public required decimal TotalCommission { get; init; }
    }
}
