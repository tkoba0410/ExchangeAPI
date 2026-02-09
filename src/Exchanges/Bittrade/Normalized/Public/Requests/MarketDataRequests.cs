using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Public.Requests;

public sealed record GetDetailMergedRequest(ProductCode ProductCode);

public sealed record GetDepthRequest(ProductCode ProductCode, string? DepthType = null);

public sealed record GetTradeRequest(ProductCode ProductCode);

public sealed record GetHistoryKlineRequest(ProductCode ProductCode, Period Period, int? Size = null);

public sealed record GetTickersRequest;

public sealed record GetHistoryTradeRequest(ProductCode ProductCode);
