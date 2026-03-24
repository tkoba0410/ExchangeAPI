namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetWithdrawals;

public static class GetWithdrawals
{
    public sealed class Item
    {
        public required long Id { get; init; }
        public required string OrderId { get; init; }
        public required string CurrencyCode { get; init; }
        public required decimal Amount { get; init; }
        public required string Status { get; init; }
        public required DateTimeOffset EventDate { get; init; }
    }
}
