using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace Exchange.Bittrade.Raw;

public sealed record BittradeTradeTick(
    [property: JsonPropertyName("data")] IReadOnlyList<BittradeTradeEntry>? Data);
