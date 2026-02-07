using System;
using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Exchanges.Bitflyer.Normalized.Internal.Types;
using ExchangeApi.Primitives.DomainCommon.Types;
using ExchangeApi.Primitives.ValueCommon.Lossless;

namespace ExchangeApi.Exchanges.Bitflyer.Normalized.Public.Dtos;

public sealed record BitflyerExecutionNormalized(
    long Id,
    BitflyerSide Side,
    decimal Price,
    decimal Size,
    DateTimeOffset ExecutedAt,
    AcceptanceId? ChildOrderAcceptanceId,
    JsonElement RawSnapshot,
    IReadOnlyDictionary<FreeText, JsonElement> Extras) : ILosslessNormalized;
