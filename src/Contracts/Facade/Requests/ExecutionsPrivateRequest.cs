using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Facade.Requests;

public sealed record ExecutionsPrivateRequest(
    Symbol Symbol,
    int? Limit = null);
