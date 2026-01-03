using System.Collections.Generic;
using System.Text.Json.Serialization;
using ExchangeApi.Exchanges.Bittrade.Raw.Types;
namespace ExchangeApi.Exchanges.Bittrade.Raw;

public sealed record RawCancelOrdersRequest(
    [property: JsonPropertyName("order-ids")] IReadOnlyList<RawOrderId> OrderIds);
