using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Public.Models;

public sealed record RawTradeTick(
    [property: JsonPropertyName("data")] IReadOnlyList<RawTradeEntry>? Data);
