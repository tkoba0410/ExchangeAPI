
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Public.Requests;

public sealed record GetDetailMergedRequest(Symbol Symbol);

public sealed record GetDepthRequest(Symbol Symbol, FreeText? Type = null);

public sealed record GetTradeRequest(Symbol Symbol);

public sealed record GetSymbolsRequest;

public sealed record GetCurrencysRequest;

public sealed record GetTimestampRequest;

public sealed record GetHistoryKlineRequest(Symbol Symbol, Period Period, int? Size = null);

public sealed record GetTickersRequest;

public sealed record GetHistoryTradeRequest(Symbol Symbol);
