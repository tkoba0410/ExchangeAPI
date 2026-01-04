using ExchangeApi.Exchanges.Bittrade.Raw.Types;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Requests;

public sealed record GetMergedTickerRequest(RawSymbol Symbol);

public sealed record GetDepthRequest(RawSymbol Symbol, string? Type = null);

public sealed record GetTradesRequest(RawSymbol Symbol);

public sealed record GetSymbolsRequest;

public sealed record GetCurrenciesRequest;

public sealed record GetTimestampRequest;

public sealed record GetKlinesRequest(RawSymbol Symbol, string Period, int? Size = null);

public sealed record GetTickersRequest;

public sealed record GetTradeHistoryRequest(RawSymbol Symbol);

public sealed record GetRetailMaintainTimeRequest;
