using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExchangeApi.Adapter.Bittrade.RawApi;

public sealed record BittradeMergedResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("tick")] BittradeMergedTick? Tick,
    [property: JsonPropertyName("ts")] long? Ts);

public sealed record BittradeMergedTick(
    [property: JsonPropertyName("close")] decimal Close,
    [property: JsonPropertyName("open")] decimal Open,
    [property: JsonPropertyName("low")] decimal Low,
    [property: JsonPropertyName("high")] decimal High,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("vol")] decimal Volume,
    [property: JsonPropertyName("ts")] long? Ts,
    [property: JsonPropertyName("bid")] decimal[] Bid,
    [property: JsonPropertyName("ask")] decimal[] Ask);

public sealed record BittradeDepthResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("tick")] BittradeDepthTick? Tick,
    [property: JsonPropertyName("ts")] long? Ts);

public sealed record BittradeDepthTick(
    [property: JsonPropertyName("bids")] IReadOnlyList<IReadOnlyList<decimal>>? Bids,
    [property: JsonPropertyName("asks")] IReadOnlyList<IReadOnlyList<decimal>>? Asks,
    [property: JsonPropertyName("ts")] long? Ts);

public sealed record BittradeTradeResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("tick")] BittradeTradeTick? Tick,
    [property: JsonPropertyName("ts")] long? Ts);

public sealed record BittradeTradeTick(
    [property: JsonPropertyName("data")] IReadOnlyList<BittradeTradeEntry>? Data);

public sealed record BittradeTradeEntry(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("price")] decimal Price,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("direction")] string Direction,
    [property: JsonPropertyName("ts")] long Ts);
