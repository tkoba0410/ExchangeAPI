using System.Text.Json.Serialization;
using ExchangeApi.Primitives.DomainCommon.Types;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Requests;

public sealed record RawPostOrdersPlaceRequest(
    [property: JsonPropertyName("account-id")] AccountId AccountId,
    [property: JsonPropertyName("symbol")]
    Symbol Symbol,
    [property: JsonPropertyName("type")] FreeText Type,
    [property: JsonPropertyName("amount")] FreeText Amount,
    [property: JsonPropertyName("price")] FreeText? Price = null,
    [property: JsonPropertyName("source")] FreeText? Source = null);
