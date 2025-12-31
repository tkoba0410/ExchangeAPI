using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace ExchangeApi.Exchanges.Bittrade.Raw;

[JsonConverter(typeof(OrderSideJsonConverter))]
public enum OrderSide
{
    [EnumMember(Value = "buy")]
    Buy,
    [EnumMember(Value = "sell")]
    Sell
}
