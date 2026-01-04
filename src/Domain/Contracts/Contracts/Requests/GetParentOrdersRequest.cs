using ExchangeApi.Common.Types;

namespace ExchangeApi.Contracts.Requests;

public sealed record GetParentOrdersRequest(
    Symbol Symbol,
    string? ParentOrderId = null,
    string? ParentOrderAcceptanceId = null);
