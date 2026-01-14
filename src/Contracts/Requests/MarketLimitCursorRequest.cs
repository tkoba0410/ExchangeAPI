using ExchangeApi.Contracts.Common.DomainCommon.Types;

namespace ExchangeApi.Contracts.Requests;

public sealed record MarketLimitCursorRequest(
    Symbol Market,
    int? Limit = null,
    string? Cursor = null);
