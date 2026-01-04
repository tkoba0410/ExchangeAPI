namespace ExchangeApi.Exchanges.Bittrade.Normalize.Requests;

public sealed record GetTickerRequest(string Symbol);

public sealed record GetOrderBookRequest(string Symbol);

public sealed record GetExecutionsRequest(string Symbol);
