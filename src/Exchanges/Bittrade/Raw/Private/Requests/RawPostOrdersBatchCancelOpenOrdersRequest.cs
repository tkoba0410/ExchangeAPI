using System;
using System.Text.Json.Serialization;
using ExchangeApi.Primitives.DomainCommon.Types;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Requests;

public sealed record RawPostOrdersBatchCancelOpenOrdersRequest(
    [property: JsonPropertyName("account-id")] AccountId? AccountId = null,
    [property: JsonPropertyName("symbol")]
    Symbol? Symbol = null,
    [property: JsonPropertyName("side")] FreeText? Side = null,
    [property: JsonPropertyName("size")] FreeText? Size = null,
    [property: JsonPropertyName("price")] FreeText? Price = null,
    [property: JsonPropertyName("created-at")]
    DateTimeOffset? CreatedAt = null);
