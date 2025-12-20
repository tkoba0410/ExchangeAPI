using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bitflyer.Raw;

/// <summary>/v1/me/withdraw リクエスト DTO。</summary>
public sealed class CreateWithdrawalRequest
{
    [JsonPropertyName("currency_code")] public string CurrencyCode { get; init; } = string.Empty; // "JPY"
    [JsonPropertyName("bank_account_id")] public int BankAccountId { get; init; }
    [JsonPropertyName("amount")] public decimal Amount { get; init; }

    [JsonPropertyName("code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Code { get; init; }
}

public sealed class CreateWithdrawalResponse
{
    [JsonPropertyName("message_id")] public string MessageId { get; init; } = string.Empty;
}
