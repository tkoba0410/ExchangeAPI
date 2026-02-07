using ExchangeApi.Contracts.Common.Dtos;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Contracts.Facade.Requests;

public sealed record CandlesticksRequest(Symbol Symbol, PeriodDto Period, int? Size = null);
