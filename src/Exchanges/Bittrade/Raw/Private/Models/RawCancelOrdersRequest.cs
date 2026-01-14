using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Models;

public sealed record RawCancelOrdersRequest(
    [property: JsonPropertyName("order-ids")] IReadOnlyList<string> OrderIds);
