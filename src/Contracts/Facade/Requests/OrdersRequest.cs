using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Facade.Requests;

public sealed record OrdersRequest(
    Symbol Symbol,
    int? Limit = null,
    Cursor? Cursor = null);
