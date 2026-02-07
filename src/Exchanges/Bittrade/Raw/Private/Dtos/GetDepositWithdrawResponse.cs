using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ExchangeApi.Primitives.JsonCommon.Converters;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Dtos;

public sealed record GetDepositWithdrawResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] IReadOnlyList<GetDepositWithdrawEntry>? Data);

public sealed record GetDepositWithdrawEntry(
    [property: JsonPropertyName("id")]
    [property: JsonConverter(typeof(StringOrNumberToStringConverter))] string Id,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("currency")] string Currency,
    [property: JsonPropertyName("amount")] decimal Amount,
    [property: JsonPropertyName("address")] string? Address,
    [property: JsonPropertyName("tx-hash")] string? TxHash,
    [property: JsonPropertyName("state")] string? State,
    [property: JsonPropertyName("created-at")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset? CreatedAt);
