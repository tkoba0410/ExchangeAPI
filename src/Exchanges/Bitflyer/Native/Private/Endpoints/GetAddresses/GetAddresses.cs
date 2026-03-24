namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetAddresses;

public static class GetAddresses
{
    public sealed class Item
    {
        public required string Type { get; init; }
        public required string CurrencyCode { get; init; }
        public required string Address { get; init; }
    }
}
