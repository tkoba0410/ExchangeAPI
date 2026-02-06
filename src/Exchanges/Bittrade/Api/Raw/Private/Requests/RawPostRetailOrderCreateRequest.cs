using System.Text.Json.Serialization;
using ExchangeApi.Primitives.DomainCommon.Types;
namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Requests;

public sealed record RawPostRetailOrderCreateRequest(
    [property: JsonPropertyName("symbol")]
    Symbol Symbol,
    [property: JsonPropertyName("type")] int Type,
    [property: JsonPropertyName("price")] FreeText? Price = null,
    [property: JsonPropertyName("amount")] FreeText? Amount = null,
    [property: JsonPropertyName("cash_amount")] FreeText? CashAmount = null);
