using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Api.Normalized.Public.Requests;

public sealed record GetTickerRequest(ProductCode ProductCode);

public sealed record GetOrderBookRequest(ProductCode ProductCode);

public sealed record GetExecutionsRequest(
    ProductCode ProductCode,
    int? Count = null,
    long? Before = null,
    long? After = null);

public sealed record GetHealthRequest(ProductCode ProductCode);

public sealed record GetBoardStateRequest(ProductCode ProductCode);

public sealed record GetChatsRequest(FreeText? FromDate = null);

public sealed record GetMarketsRequest;

public sealed record GetCorporateLeverageRequest;

public sealed record GetFundingRateRequest(ProductCode ProductCode);
