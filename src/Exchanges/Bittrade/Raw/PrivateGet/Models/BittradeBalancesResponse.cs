using System.Text.Json.Serialization;
namespace Exchange.Bittrade.Raw;

public sealed record BittradeBalancesResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] BittradeBalanceData? Data);
