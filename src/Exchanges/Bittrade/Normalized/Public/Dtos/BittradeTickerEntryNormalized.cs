using System;
using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Primitives.ValueCommon.Lossless;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;

public sealed record BittradeTickerEntryNormalized(
    string Symbol,
    decimal LastTradedPrice,
    DateTimeOffset Timestamp,
    JsonElement RawSnapshot,
    IReadOnlyDictionary<string, JsonElement> Extras) : ILosslessNormalized;
