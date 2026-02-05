using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Facade.Requests;

public sealed record GetOrderRequest(
    Symbol Symbol,
    OrderKey OrderKey);
