using System;
using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Primitives.ValueCommon.Lossless;
using ExchangeApi.Exchanges.Bittrade.Normalized.Internal.Types;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Normalized.Public.Dtos;

public sealed record ExecutionNormalized(
    OrderId OrderId,
    OrderSide Side,
    decimal Price,
    decimal Size,
    DateTimeOffset Timestamp,
    JsonElement RawSnapshot,
    IReadOnlyDictionary<FreeText, JsonElement> Extras) : ILosslessNormalized;
