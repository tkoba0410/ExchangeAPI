using System;
using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Primitives.ValueCommon.Lossless;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;

public sealed record TickerNormalized(
    decimal LastTradedPrice,
    DateTimeOffset Timestamp,
    JsonElement RawSnapshot,
    IReadOnlyDictionary<FreeText, JsonElement> Extras) : ILosslessNormalized;
