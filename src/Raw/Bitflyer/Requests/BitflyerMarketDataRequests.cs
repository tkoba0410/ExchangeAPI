
namespace ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

public sealed record GetTickerRequest(string ProductCode, bool UseAliasPath = false);

public sealed record GetBoardRequest(string ProductCode, bool UseAliasPath = false);

public sealed record GetExecutionsRequest(
    string ProductCode,
    int? Count = null,
    long? Before = null,
    long? After = null,
    bool UseAliasPath = false);

public sealed record GetMarketsRequest(string? Region = null, bool UseAliasPath = false);

public sealed record GetChatsRequest(string? FromDate = null, string? Region = null);

public sealed record GetHealthRequest(string ProductCode);

public sealed record GetBoardStateRequest(string ProductCode);

public sealed record GetCorporateLeverageRequest;

public sealed record GetFundingRateRequest(string ProductCode);
