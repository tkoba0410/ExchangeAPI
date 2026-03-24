namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrder;

public sealed class GetParentOrderResponse
{
    public required long Id { get; init; }
    public required string ParentOrderId { get; init; }
    public required string OrderMethod { get; init; }
    public required DateTimeOffset ExpireDate { get; init; }
    public required string TimeInForce { get; init; }
    public required IReadOnlyList<GetParentOrderParameter> Parameters { get; init; }
    public required string ParentOrderAcceptanceId { get; init; }
}

public sealed class GetParentOrderParameter
{
    public required string ProductCode { get; init; }
    public required string ConditionType { get; init; }
    public required string Side { get; init; }
    public required decimal Price { get; init; }
    public required decimal Size { get; init; }
    public required decimal TriggerPrice { get; init; }
    public required long Offset { get; init; }
}
