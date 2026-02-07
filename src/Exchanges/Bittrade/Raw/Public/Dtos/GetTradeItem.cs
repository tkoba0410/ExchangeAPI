using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Public.Dtos;

public sealed record GetTradeItem(
    [property: JsonPropertyName("data")] IReadOnlyList<GetHistoryTradeEntry>? Data);
