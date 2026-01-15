using System.Collections.Generic;
using System.Text.Json;

namespace ExchangeApi.Primitives.ValueCommon.Lossless;

public interface ILosslessNormalized
{
    JsonElement RawSnapshot { get; }
    IReadOnlyDictionary<string, JsonElement> Extras { get; }
}
