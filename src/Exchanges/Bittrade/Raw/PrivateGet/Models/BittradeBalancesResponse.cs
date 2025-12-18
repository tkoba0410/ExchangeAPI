using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record BittradeBalancesResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] BittradeBalanceData? Data);
