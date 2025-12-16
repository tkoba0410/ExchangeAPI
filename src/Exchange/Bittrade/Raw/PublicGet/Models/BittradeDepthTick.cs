using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace Exchange.Bittrade.Raw;

public sealed record BittradeDepthTick(
    [property: JsonPropertyName("bids")] IReadOnlyList<IReadOnlyList<decimal>>? Bids,
    [property: JsonPropertyName("asks")] IReadOnlyList<IReadOnlyList<decimal>>? Asks,
    [property: JsonPropertyName("ts")] long? Ts);
