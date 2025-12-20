using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record TradeTick(
    [property: JsonPropertyName("data")] IReadOnlyList<TradeEntry>? Data);
