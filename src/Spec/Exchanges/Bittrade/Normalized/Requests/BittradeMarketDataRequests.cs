using ExchangeApi.Exchanges.Bittrade.Normalize.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Requests;

public sealed record GetTickerRequest(BittradeSymbol Symbol);

public sealed record GetOrderBookRequest(BittradeSymbol Symbol, BittradeDepthType? DepthType = null);

public sealed record GetExecutionsRequest(BittradeSymbol Symbol);
