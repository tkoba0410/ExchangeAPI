using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record RawCancelOrdersRequest(
    [property: JsonPropertyName("order-ids")] IReadOnlyList<RawOrderId> OrderIds);
