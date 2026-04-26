using System.Text.Json.Serialization;

namespace ExchangeApi.Adapters.McpServer.Schema.Inspection;

public sealed class GetBalanceHistoryRequest : BitflyerPrivateReadRequestBase
{
    [JsonPropertyName("currencyCode")]
    public string? CurrencyCode { get; init; }

    [JsonPropertyName("count")]
    public int? Count { get; init; }

    [JsonPropertyName("before")]
    public long? Before { get; init; }

    [JsonPropertyName("after")]
    public long? After { get; init; }
}
