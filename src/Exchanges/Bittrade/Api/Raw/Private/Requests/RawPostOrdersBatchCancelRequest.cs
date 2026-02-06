using System.Collections.Generic;
using System.Text.Json.Serialization;
using ExchangeApi.Primitives.DomainCommon.Types;
namespace ExchangeApi.Exchanges.Bittrade.Api.Raw.Private.Requests;

public sealed record RawPostOrdersBatchCancelRequest(
    [property: JsonPropertyName("order-ids")] IReadOnlyList<OrderId> OrderIds);
