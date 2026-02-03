
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Public.Requests;

public sealed record GetTickerRequest(Symbol Symbol);

public sealed record GetOrderBookRequest(Symbol Symbol, FreeText? Type = null);

public sealed record GetMarketTradesRequest(Symbol Symbol);
