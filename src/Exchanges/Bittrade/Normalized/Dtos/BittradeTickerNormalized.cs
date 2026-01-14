using System;
using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Contracts.Common.ValueCommon.Lossless;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Dtos;

public sealed record BittradeTickerNormalized(
    decimal LastTradedPrice,
    DateTimeOffset Timestamp,
    JsonElement RawSnapshot,
    IReadOnlyDictionary<string, JsonElement> Extras) : ILosslessNormalized;
