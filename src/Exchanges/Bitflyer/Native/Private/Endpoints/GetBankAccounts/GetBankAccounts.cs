namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBankAccounts;

public static class GetBankAccounts
{
    public sealed class Item
    {
        public required long Id { get; init; }
        public required bool IsVerified { get; init; }
        public required string BankName { get; init; }
        public required string BranchName { get; init; }
        public required string AccountType { get; init; }
        public required string AccountNumber { get; init; }
        public required string AccountName { get; init; }
    }
}
