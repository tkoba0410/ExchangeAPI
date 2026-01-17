using ExchangeApi.Exchanges.Bittrade.Normalized.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Requests;

public sealed record GetTickerRequest(string ProductCode);

public sealed record GetOrderBookRequest(string ProductCode, BittradeDepthType? DepthType = null);

public sealed record GetExecutionsRequest(string ProductCode);
