using ExchangeApi.Exchanges.Bittrade.Normalized.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Requests;

public sealed record GetTickerRequest(BittradeSymbol Symbol);

public sealed record GetOrderBookRequest(BittradeSymbol Symbol, BittradeDepthType? DepthType = null);

public sealed record GetExecutionsRequest(BittradeSymbol Symbol);
