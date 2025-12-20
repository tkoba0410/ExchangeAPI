using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record DepositWithdrawsResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] IReadOnlyList<DepositWithdrawEntry>? Data);

public sealed record DepositWithdrawEntry(
    [property: JsonPropertyName("id")] long Id,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("address")] string? Address,
    [property: JsonPropertyName("tx-hash")] string? TxHash,
    [property: JsonPropertyName("state")] string? State,
    [property: JsonPropertyName("created-at")] long? CreatedAt);
