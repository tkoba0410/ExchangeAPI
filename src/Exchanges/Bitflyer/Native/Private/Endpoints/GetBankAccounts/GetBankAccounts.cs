using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.GetBankAccounts;

public static class GetBankAccounts
{
    public sealed class Item
    {
        [JsonPropertyName("id")]
        public required long Id { get; init; }
        [JsonPropertyName("is_verified")]
        public required bool IsVerified { get; init; }
        [JsonPropertyName("bank_name")]
        public required string BankName { get; init; }
        [JsonPropertyName("branch_name")]
        public required string BranchName { get; init; }
        [JsonPropertyName("account_type")]
        public required string AccountType { get; init; }
        [JsonPropertyName("account_number")]
        public required string AccountNumber { get; init; }
        [JsonPropertyName("account_name")]
        public required string AccountName { get; init; }
    }
}
