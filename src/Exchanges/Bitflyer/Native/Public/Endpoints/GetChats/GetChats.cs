using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetChats;

public static class GetChats
{
    public sealed class Item
    {
        [JsonPropertyName("nickname")]
        public required string Nickname { get; init; }
        [JsonPropertyName("message")]
        public required string Message { get; init; }
        [JsonPropertyName("date")]
        public required DateTimeOffset Date { get; init; }
    }
}
