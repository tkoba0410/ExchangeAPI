using ExchangeApi.Contracts.Common.DomainCommon.Types;

namespace ExchangeApi.Contracts.Requests;

public sealed record GetMarketExecutionsRequest(Symbol Symbol);
