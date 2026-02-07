using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Dtos;

public sealed record GetAccountsBalanceByAccountIdData(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("state")] string State,
    [property: JsonPropertyName("list")] IReadOnlyList<GetAccountsBalanceByAccountIdEntry> List);
