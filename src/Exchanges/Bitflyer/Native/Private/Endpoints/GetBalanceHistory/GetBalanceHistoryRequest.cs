using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalanceHistory;

public sealed class GetBalanceHistoryRequest
{
    [JsonPropertyName("currency_code")]
    public string? CurrencyCode { get; init; }
    [JsonPropertyName("count")]
    public int? Count { get; init; }
    [JsonPropertyName("before")]
    public long? Before { get; init; }
    [JsonPropertyName("after")]
    public long? After { get; init; }
}
