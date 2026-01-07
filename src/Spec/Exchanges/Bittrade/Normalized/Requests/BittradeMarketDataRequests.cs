using ExchangeApi.Exchanges.Bittrade.Normalize.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Requests;

public sealed record GetTickerRequest(string Symbol);

public sealed record GetOrderBookRequest(string Symbol, BittradeDepthType? DepthType = null);

public sealed record GetExecutionsRequest(string Symbol);
