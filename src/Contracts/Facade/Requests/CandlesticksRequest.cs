using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Facade.Requests;

public sealed record CandlesticksRequest(Symbol Symbol, Period Period, int? Size = null);
