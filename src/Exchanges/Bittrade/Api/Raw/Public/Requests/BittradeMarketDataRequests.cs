
namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Public.Requests;

public sealed record GetTickerRequest(string Symbol);

public sealed record GetOrderBookRequest(string Symbol, string? Type = null);

public sealed record GetMarketTradesRequest(string Symbol);
