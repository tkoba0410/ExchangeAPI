
namespace ExchangeApi.Exchanges.Bittrade.Raw.Requests;

public sealed record GetTickerRequest(string Symbol);

public sealed record GetOrderBookRequest(string Symbol, string? Type = null);

public sealed record GetMarketTradesRequest(string Symbol);
