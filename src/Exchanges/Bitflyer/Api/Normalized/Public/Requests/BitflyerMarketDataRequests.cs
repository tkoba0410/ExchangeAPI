namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Requests;

public sealed record GetTickerRequest(string ProductCode);

public sealed record GetOrderBookRequest(string ProductCode);

public sealed record GetExecutionsRequest(
    string ProductCode,
    int? Count = null,
    long? Before = null,
    long? After = null);

public sealed record GetHealthRequest(string ProductCode);

public sealed record GetBoardStateRequest(string ProductCode);

public sealed record GetChatsRequest(string? FromDate = null);

public sealed record GetMarketsRequest;

public sealed record GetCorporateLeverageRequest;

public sealed record GetFundingRateRequest(string ProductCode);
