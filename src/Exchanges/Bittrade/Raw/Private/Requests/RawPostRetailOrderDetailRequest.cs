using System.Text.Json.Serialization;
using ExchangeApi.Primitives.DomainCommon.Types;

namespace ExchangeApi.Exchanges.Bittrade.Raw.Private.Requests;

public sealed record RawPostRetailOrderDetailRequest(
    [property: JsonPropertyName("orderId")] OrderId OrderId);
