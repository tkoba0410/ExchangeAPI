using System;
using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Primitives.ValueCommon.Lossless;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;

public sealed record TickerNormalized(
    ProductCode ProductCode,
    DateTimeOffset Timestamp,
    long TickId,
    decimal BestBid,
    decimal BestAsk,
    decimal BestBidSize,
    decimal BestAskSize,
    decimal TotalBidDepth,
    decimal TotalAskDepth,
    decimal LastTradedPrice,
    decimal Volume,
    decimal VolumeByProduct,
    JsonElement RawSnapshot,
    IReadOnlyDictionary<FreeText, JsonElement> Extras) : ILosslessNormalized;
