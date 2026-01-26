using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Private.Requests;

public sealed record GetBalancesRequest;

public sealed record GetAccountExecutionsRequest(Symbol Symbol);

public sealed record GetTradingCommissionRequest(Symbol Symbol);
