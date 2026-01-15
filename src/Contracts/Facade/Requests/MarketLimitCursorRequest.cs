using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Facade.Requests;

public sealed record MarketLimitCursorRequest(
    Symbol Market,
    int? Limit = null,
    string? Cursor = null);
