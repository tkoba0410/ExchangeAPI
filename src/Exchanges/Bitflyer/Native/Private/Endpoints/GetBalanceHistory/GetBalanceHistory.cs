namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBalanceHistory;

public static class GetBalanceHistory
{
    public sealed class Item
    {
        public required long Id { get; init; }
        public required DateTimeOffset TradeDate { get; init; }
        public required DateTimeOffset EventDate { get; init; }
        public string? ProductCode { get; init; }
        public required string CurrencyCode { get; init; }
        public required string TradeType { get; init; }
        public required decimal Price { get; init; }
        public required decimal Amount { get; init; }
        public required decimal Quantity { get; init; }
        public required decimal Commission { get; init; }
        public required decimal Balance { get; init; }
        public string? OrderId { get; init; }
    }
}
