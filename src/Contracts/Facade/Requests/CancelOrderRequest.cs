using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Facade.Requests;

public sealed record CancelOrderRequest(
    Symbol Symbol,
    OrderKey OrderKey);
