using System;
using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Primitives.ValueCommon.Lossless;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;

public sealed record TickerEntryNormalized(
    Symbol Symbol,
    decimal LastTradedPrice,
    DateTimeOffset? Timestamp,
    JsonElement RawSnapshot,
    IReadOnlyDictionary<FreeText, JsonElement> Extras) : ILosslessNormalized;
