using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bitflyer.Native.Private.Endpoints.Withdraw;

public sealed class WithdrawRequest
{
    [JsonPropertyName("currency_code")]
    public required string CurrencyCode { get; init; }

    [JsonPropertyName("bank_account_id")]
    public required long BankAccountId { get; init; }

    [JsonPropertyName("amount")]
    public required decimal Amount { get; init; }

    [JsonPropertyName("code")]
    public required string Code { get; init; }
}
