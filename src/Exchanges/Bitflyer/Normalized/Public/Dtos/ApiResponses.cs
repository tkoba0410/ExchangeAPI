using System;
using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;

public sealed record GetMarketsItem(MarketNormalized Value);
public sealed record GetMarketsResponse(IReadOnlyList<GetMarketsItem> Items);

public sealed record GetBoardResponse(
    IReadOnlyList<OrderBookLevelNormalized> Bids,
    IReadOnlyList<OrderBookLevelNormalized> Asks);
public sealed record GetTickerResponse(
    ProductCode ProductCode,
    decimal LastTradedPrice,
    DateTimeOffset Timestamp,
    JsonElement RawSnapshot,
    IReadOnlyDictionary<FreeText, JsonElement> Extras);

public sealed record GetExecutionsPublicItem(ExecutionNormalized Value);
public sealed record GetExecutionsPublicResponse(IReadOnlyList<GetExecutionsPublicItem> Items);

public sealed record GetBoardStateResponse(
    FreeText? Health,
    FreeText? State,
    FreeText? Data);
public sealed record GetHealthResponse(FreeText? Status);
public sealed record GetFundingRateResponse(
    decimal CurrentFundingRate,
    DateTimeOffset NextFundingRateSettleDate);
public sealed record GetCorporateLeverageResponse(
    decimal CurrentMax,
    DateTimeOffset CurrentStartDate,
    decimal? NextMax,
    DateTimeOffset? NextStartDate);

public sealed record GetChatsItem(ChatNormalized Value);
public sealed record GetChatsResponse(IReadOnlyList<GetChatsItem> Items);
