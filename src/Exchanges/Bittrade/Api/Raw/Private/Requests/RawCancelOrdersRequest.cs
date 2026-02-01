using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Requests;

public sealed record RawCancelOrdersRequest(
    [property: JsonPropertyName("order-ids")] IReadOnlyList<string> OrderIds);
