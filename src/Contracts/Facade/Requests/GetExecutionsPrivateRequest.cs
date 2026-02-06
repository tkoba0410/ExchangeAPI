using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Facade.Requests;

public sealed record GetExecutionsPrivateRequest(
    Symbol Market,
    int? Limit = null,
    Cursor? Cursor = null);
