using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Application.Trading;

public sealed record GetOrderQuery(Symbol Symbol, OrderKey OrderKey);
