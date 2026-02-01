using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Types;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Requests;

public sealed record GetTickerRequest(string ProductCode);

public sealed record GetOrderBookRequest(string ProductCode, BittradeDepthType? DepthType = null);

public sealed record GetExecutionsRequest(string ProductCode);

public sealed record GetHistoryKlineRequest(string ProductCode, string Period, int? Size = null);

public sealed record GetTickersRequest;

public sealed record GetHistoryTradeRequest(string ProductCode);
