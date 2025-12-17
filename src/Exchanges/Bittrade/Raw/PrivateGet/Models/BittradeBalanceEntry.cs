using System.Text.Json.Serialization;
namespace Exchange.Bittrade.Raw;

public sealed record BittradeBalanceEntry(
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("balance")] string Balance);
