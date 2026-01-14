using ExchangeApi.Contracts.Common.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Requests;

public sealed record GetBalancesRequest;

public sealed record GetAccountExecutionsRequest(Symbol Symbol);

public sealed record GetTradingCommissionRequest(Symbol Symbol);
