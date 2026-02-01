
namespace ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Requests;

public sealed record GetTickerRequest(string ProductCode, bool UseAliasPath = false);

public sealed record GetBoardRequest(string ProductCode, bool UseAliasPath = false);

public sealed record GetExecutionsRequest(
    string ProductCode,
    int? Count = null,
    long? Before = null,
    long? After = null,
    bool UseAliasPath = false);

public sealed record GetMarketsRequest(bool UseAliasPath = false);

public sealed record GetChatsRequest(string? FromDate = null);

public sealed record GetHealthRequest(string ProductCode);

public sealed record GetBoardStateRequest(string ProductCode);

public sealed record GetCorporateLeverageRequest;

public sealed record GetFundingRateRequest(string ProductCode);
