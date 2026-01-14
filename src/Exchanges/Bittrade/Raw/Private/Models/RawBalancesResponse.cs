using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;

public sealed record RawBalancesResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] RawBalanceData? Data);
