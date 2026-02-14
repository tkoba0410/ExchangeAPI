using System.Text.Json.Serialization;
using ExchangeApi.Primitives.DomainCommon.Types;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Requests;

public sealed record RawPostWithdrawApiCreateRequest(
    [property: JsonPropertyName("address")] FreeText Address,
    [property: JsonPropertyName("amount")] FreeText Amount,
    [property: JsonPropertyName("currency")] FreeText Currency,
    [property: JsonPropertyName("fee")] FreeText? Fee = null,
    [property: JsonPropertyName("addr-tag")] FreeText? AddressTag = null);
