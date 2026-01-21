
namespace ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;

public sealed record GetMergedTickerRequest(string Symbol);

public sealed record GetDepthRequest(string Symbol, string? Type = null);

public sealed record GetTradesRequest(string Symbol);

public sealed record GetSymbolsRequest;

public sealed record GetCurrenciesRequest;

public sealed record GetTimestampRequest;

public sealed record GetKlinesRequest(string Symbol, string Period, int? Size = null);

public sealed record GetTickersRequest;

public sealed record GetTradeHistoryRequest(string Symbol);

public sealed record GetRetailMaintainTimeRequest;
