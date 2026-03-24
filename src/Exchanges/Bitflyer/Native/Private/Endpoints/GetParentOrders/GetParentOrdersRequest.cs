namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrders;

public sealed class GetParentOrdersRequest
{
    public string? ProductCode { get; init; }
    public int? Count { get; init; }
    public long? Before { get; init; }
    public long? After { get; init; }
    public string? ParentOrderState { get; init; }
}
