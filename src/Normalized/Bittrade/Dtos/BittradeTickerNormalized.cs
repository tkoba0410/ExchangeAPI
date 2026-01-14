using System;
using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Spec.ValueCommon.Lossless;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Dtos;

public sealed record BittradeTickerNormalized(
    decimal LastTradedPrice,
    DateTimeOffset Timestamp,
    JsonElement RawSnapshot,
    IReadOnlyDictionary<string, JsonElement> Extras) : ILosslessNormalized;
