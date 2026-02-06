
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Raw.Public.Requests;

public sealed record GetTickerRequest(ProductCode ProductCode, bool UseAliasPath = false);

public sealed record GetBoardRequest(ProductCode ProductCode, bool UseAliasPath = false);

public sealed record GetExecutionsPublicRequest(
    ProductCode ProductCode,
    int? Count = null,
    long? Before = null,
    long? After = null,
    bool UseAliasPath = false);

public sealed record GetMarketsRequest(bool UseAliasPath = false);

public sealed record GetChatsRequest(FreeText? FromDate = null);

public sealed record GetHealthRequest(ProductCode ProductCode);

public sealed record GetBoardStateRequest(ProductCode ProductCode);

public sealed record GetCorporateLeverageRequest;

public sealed record GetFundingRateRequest(ProductCode ProductCode);
