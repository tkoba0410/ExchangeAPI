using ExchangeApi.Common.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Requests;

public sealed record GetOpenPositionsRequest(Symbol Symbol);

public sealed record GetCollateralRequest;
