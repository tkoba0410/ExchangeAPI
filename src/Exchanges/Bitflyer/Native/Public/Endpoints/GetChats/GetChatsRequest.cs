using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetChats;

public sealed class GetChatsRequest
{
    [JsonPropertyName("from_date")]
    public string? FromDate { get; init; }
}
