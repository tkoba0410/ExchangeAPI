using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ExchangeApi.Primitives.JsonCommon.Converters;

namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Dtos;

public sealed record RawWithdrawVirtualAddressesResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] IReadOnlyList<RawWithdrawVirtualAddress>? Data);

public sealed record RawWithdrawVirtualAddress(
    [property: JsonPropertyName("address")] string? Address,
    [property: JsonPropertyName("address-id")] long? AddressId,
    [property: JsonPropertyName("address-tag")] string? AddressTag,
    [property: JsonPropertyName("currency")] string? Currency,
    [property: JsonPropertyName("chain")] string? Chain,
    [property: JsonPropertyName("note")] string? Note,
    [property: JsonPropertyName("state")] string? State,
    [property: JsonPropertyName("created-at")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset? CreatedAt,
    [property: JsonPropertyName("updated-at")]
    [property: JsonConverter(typeof(UnixTimeMillisecondsDateTimeOffsetConverter))] DateTimeOffset? UpdatedAt);
