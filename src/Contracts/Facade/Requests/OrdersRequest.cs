using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Facade.Requests;

public sealed record OrdersRequest(
    Symbol Market,
    int? Limit = null,
    Cursor? Cursor = null);
