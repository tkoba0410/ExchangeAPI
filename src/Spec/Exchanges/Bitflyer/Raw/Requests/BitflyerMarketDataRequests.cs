using ExchangeApi.Exchanges.Bitflyer.Raw.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Raw.Requests;

public sealed record GetTickerRequest(RawProductCode ProductCode, bool UseAliasPath = false);

public sealed record GetBoardRequest(RawProductCode ProductCode, bool UseAliasPath = false);

public sealed record GetExecutionsRequest(
    RawProductCode ProductCode,
    int? Count = null,
    long? Before = null,
    long? After = null,
    bool UseAliasPath = false);

public sealed record GetMarketsRequest(string? Region = null, bool UseAliasPath = false);

public sealed record GetChatsRequest(string? FromDate = null, string? Region = null);

public sealed record GetHealthRequest(RawProductCode ProductCode);

public sealed record GetBoardStateRequest(RawProductCode ProductCode);

public sealed record GetCorporateLeverageRequest;

public sealed record GetFundingRateRequest(RawProductCode ProductCode);
