using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Facade.Requests;

public sealed record GetOpenOrdersRequest(Symbol Market);
