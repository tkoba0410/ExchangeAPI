using System.Text.Json.Serialization;
using ExchangeApi.Primitives.DomainCommon.Types;
namespace ExchangeApi.Exchanges.Bitflyer.Raw.Private.Requests;

/// <summary>/v1/me/withdraw リクエスト DTO。</summary>
public sealed class WithdrawRequest
{
    [JsonPropertyName("currency_code")] public FreeText CurrencyCode { get; init; } = FreeText.Empty; // "JPY"
    [JsonPropertyName("bank_account_id")] public int BankAccountId { get; init; }
    [JsonPropertyName("amount")] public decimal Amount { get; init; }

    [JsonPropertyName("code")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public FreeText? Code { get; init; }
}
