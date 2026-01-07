using ExchangeApi.Common.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Requests;

public sealed record GetBalancesRequest;

public sealed record GetAccountExecutionsRequest(Symbol Symbol);

public sealed record GetTradingCommissionRequest(Symbol Symbol);
