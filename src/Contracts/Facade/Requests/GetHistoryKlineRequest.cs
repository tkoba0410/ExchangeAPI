using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Facade.Requests;

public sealed record GetHistoryKlineRequest(Symbol Symbol, string Period, int? Size = null);
