using System;
using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Spec.ValueCommon.Lossless;

namespace ExchangeApi.Exchanges.Bittrade.Normalize.Dtos;

public sealed record BittradeExecutionNormalized(
    string Id,
    string Side,
    decimal Price,
    decimal Size,
    DateTimeOffset Timestamp,
    JsonElement RawSnapshot,
    IReadOnlyDictionary<string, JsonElement> Extras) : ILosslessNormalized;
