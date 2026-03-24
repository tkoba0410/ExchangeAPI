namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetParentOrder;

public sealed class GetParentOrderRequest
{
    public string? ParentOrderId { get; init; }
    public string? ParentOrderAcceptanceId { get; init; }
}
