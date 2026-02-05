using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Public.Dtos;

public sealed record RawTradeTick(
    [property: JsonPropertyName("data")] IReadOnlyList<RawTradeEntry>? Data);
