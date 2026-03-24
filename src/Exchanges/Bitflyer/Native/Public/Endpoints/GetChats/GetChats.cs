namespace ExchangeApi.Exchanges.Bitflyer.Native.Public.Endpoints.GetChats;

public static class GetChats
{
    public sealed class Item
    {
        public required string Nickname { get; init; }
        public required string Message { get; init; }
        public required DateTimeOffset Date { get; init; }
    }
}
