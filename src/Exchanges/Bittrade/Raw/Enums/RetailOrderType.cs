using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

[JsonConverter(typeof(RetailOrderTypeJsonConverter))]
public enum RetailOrderType
{
    Buy = 1,
    Sell = 2
}
