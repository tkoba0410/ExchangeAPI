using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record BittradeTradeTick(
    [property: JsonPropertyName("data")] IReadOnlyList<BittradeTradeEntry>? Data);
