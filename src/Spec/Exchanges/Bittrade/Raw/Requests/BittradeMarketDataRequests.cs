using ExchangeApi.Exchanges.Bittrade.Raw.Types;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Requests;

public sealed record GetTickerRequest(RawSymbol Symbol);

public sealed record GetOrderBookRequest(RawSymbol Symbol, string? Type = null);

public sealed record GetMarketTradesRequest(RawSymbol Symbol);
