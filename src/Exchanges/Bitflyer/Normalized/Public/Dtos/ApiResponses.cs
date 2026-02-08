using System.Collections.Generic;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;

public sealed record GetMarketsItem(MarketNormalized Value);
public sealed record GetMarketsResponse(IReadOnlyList<GetMarketsItem> Items);

public sealed record GetBoardResponse(OrderBookNormalized Item);
public sealed record GetTickerResponse(TickerNormalized Item);

public sealed record GetExecutionsPublicItem(ExecutionNormalized Value);
public sealed record GetExecutionsPublicResponse(IReadOnlyList<GetExecutionsPublicItem> Items);

public sealed record GetBoardStateResponse(BoardStateNormalized Item);
public sealed record GetHealthResponse(HealthNormalized Item);
public sealed record GetFundingRateResponse(FundingRateNormalized Item);
public sealed record GetCorporateLeverageResponse(CorporateLeverageNormalized Item);

public sealed record GetChatsItem(ChatNormalized Value);
public sealed record GetChatsResponse(IReadOnlyList<GetChatsItem> Items);
