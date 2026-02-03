using System.Collections.Generic;
using System.Text.Json;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Primitives.ValueCommon.Lossless;

public interface ILosslessNormalized
{
    JsonElement RawSnapshot { get; }
    IReadOnlyDictionary<FreeText, JsonElement> Extras { get; }
}
