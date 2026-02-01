using System;
using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Primitives.ValueCommon.Lossless;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Dtos;

public sealed record BittradeTickerNormalized(
    decimal LastTradedPrice,
    DateTimeOffset Timestamp,
    JsonElement RawSnapshot,
    IReadOnlyDictionary<string, JsonElement> Extras) : ILosslessNormalized;
