using System;
using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Primitives.ValueCommon.Lossless;
using ExchangeApi.Exchanges.Bittrade.Api.Normalized.Internal.Types;

namespace ExchangeApi.Exchanges.Bittrade.Api.Normalized.Public.Dtos;

public sealed record BittradeExecutionNormalized(
    string Id,
    BittradeOrderSide Side,
    decimal Price,
    decimal Size,
    DateTimeOffset Timestamp,
    JsonElement RawSnapshot,
    IReadOnlyDictionary<string, JsonElement> Extras) : ILosslessNormalized;
