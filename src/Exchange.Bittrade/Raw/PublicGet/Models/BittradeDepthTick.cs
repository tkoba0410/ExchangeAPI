using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExchangeApi.Adapter.Bittrade.RawApi;

public sealed record BittradeDepthTick(
    [property: JsonPropertyName("bids")] IReadOnlyList<IReadOnlyList<decimal>>? Bids,
    [property: JsonPropertyName("asks")] IReadOnlyList<IReadOnlyList<decimal>>? Asks,
    [property: JsonPropertyName("ts")] long? Ts);
