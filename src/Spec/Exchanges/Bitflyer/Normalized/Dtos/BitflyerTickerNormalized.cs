using System;
using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Spec.ValueCommon.Lossless;

namespace ExchangeApi.Exchanges.Bitflyer.Normalize.Dtos;

public sealed record BitflyerTickerNormalized(
    string ProductCode,
    decimal LastTradedPrice,
    DateTimeOffset Timestamp,
    JsonElement RawSnapshot,
    IReadOnlyDictionary<string, JsonElement> Extras) : ILosslessNormalized;
