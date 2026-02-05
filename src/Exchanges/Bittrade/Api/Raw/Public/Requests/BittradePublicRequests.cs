
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Public.Requests;

public sealed record GetMergedTickerRequest(Symbol Symbol);

public sealed record GetDepthRequest(Symbol Symbol, FreeText? Type = null);

public sealed record GetTradesRequest(Symbol Symbol);

public sealed record GetSymbolsRequest;

public sealed record GetCurrenciesRequest;

public sealed record GetTimestampRequest;

public sealed record GetKlinesRequest(Symbol Symbol, Period Period, int? Size = null);

public sealed record GetTickersRequest;

public sealed record GetTradeHistoryRequest(Symbol Symbol);
