using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExchangeApi.Adapter.Bittrade.RawApi;

public sealed record BittradeTradeTick(
    [property: JsonPropertyName("data")] IReadOnlyList<BittradeTradeEntry>? Data);
