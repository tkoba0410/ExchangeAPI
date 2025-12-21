using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record CancelOrdersRequest(
    [property: JsonPropertyName("order-ids")] IReadOnlyList<OrderId> OrderIds);
